using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using System.Globalization;

[System.Serializable] public class UnityStringEvent : UnityEvent<string> { }

public class LOLAPIHandler : MonoBehaviour
{
	[SerializeField] private StringPairVariable championNameReplacer;
	private TextAsset championJSONAsset;
	private string activeChampion;
	public string p, q, w, e, r;
	public List<string> skins;
	public List<int> skinID;

	[Header("Skin Dropdown Buttons")]
	[SerializeField] private Transform skinContent;
	[SerializeField] private GameObject skinButtonPrefab, spellButtonPrefab;
	[SerializeField] private List<GameObject> skinButtons = new List<GameObject>();
	private bool apiEnabled = false;

	// RIOT API URLs
	[Header("Image Push")]
	public FileUpload uploader;
	private string lolapiversionurl = "https://raw.githubusercontent.com/kf-pixel/LOR-Card-Creator/master/lolapiversion.txt";
	private string patchVersion = "11.4.1";
	private string champURL = "http://ddragon.leagueoflegends.com/cdn/PATCH_VERSION/data/en_US/champion/";
	private string splashURL = "http://ddragon.leagueoflegends.com/cdn/img/champion/splash/";
	private string passiveURL = "http://ddragon.leagueoflegends.com/cdn/PATCH_VERSION/img/passive/";
	private string spellURL = "http://ddragon.leagueoflegends.com/cdn/PATCH_VERSION/img/spell/";

	[Header("Events")]
	[SerializeField] private UnityEvent spellImageEvent;

	public void EnableAPI()
	{
		apiEnabled = true;
	}

	public async void CheckLOLAPIVersion()
	{
		string versionWeb = await GetWebText(lolapiversionurl);

		if (versionWeb != null)
		{
			patchVersion = versionWeb.Replace(" ", "").Replace("\n","");
		}
	}

	public async Task<string> GetWebText(string url)
	{
		using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
		{
			// Send URL Request
			var asyncOp = webRequest.SendWebRequest();

			// Await until it's done: 
			while (asyncOp.isDone == false)
			{
				await Task.Delay(1000 / 60);
			}

			// Read Results:
			if (webRequest.isNetworkError || webRequest.isHttpError)
			{
				// Log Error && Exit:

				return null;
			}
			else
			{
				// Else Return if Valid:
				return webRequest.downloadHandler.text;
			}
		}
	}

	public void LoadChampionAsset(string input)
	{
		// return if the api isnt enabled via github flag
		if (!apiEnabled)
		{
			return;
		}

		// replace string and get champ
		foreach (StringPair sp in championNameReplacer.values)
		{
			if (input.ToLower() == sp.inputString.ToLower())
			{
				FindChampionJSON(sp.replacedString);
				return;
			}
		}

		// or clean up string and get champ
		TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
		input = textInfo.ToTitleCase(input).Replace(" ", "");

		FindChampionJSON(input);
	}

	private async void FindChampionJSON(string directChampName)
	{
		string url = champURL.Replace("PATCH_VERSION", patchVersion) + directChampName + ".json";
		string champJSON = await GetWebText(url);

		if (champJSON != null)
		{
			skins.Clear();
			skinID.Clear();

			championJSONAsset = new TextAsset(champJSON);
			championJSONAsset.name = directChampName;
			GetChampionJSON(championJSONAsset);
			
			GenerateSkinButtons();
			GenerateSpellButtons();
		}
	}

	private void GetChampionJSON(TextAsset asset)
	{
		StartCoroutine(GetChampionURLs(asset));
	}

	private IEnumerator GetChampionURLs(TextAsset json)
	{
		activeChampion = json.name;

		// add default skin
		skinID.Add(0);

		// Get List of Skin Names
		int i = json.text.IndexOf("\"skins\"");
		int lorei = json.text.IndexOf("\"lore\"");

		int loops = 0;
		while (i < lorei && loops < 30)
		{
			i = json.text.IndexOf("\"name\"", i) + 8;
			if (i > lorei) break;

			int commai = json.text.IndexOf(",", i);
			skins.Add(json.text.Substring(i, commai - i - 1));


			// Add skins
			int numi = json.text.IndexOf("\"num\":", i - 11);
			if (numi > -1)
			{
				int numcommai = json.text.IndexOf(",", numi);
				int skini = 0;
				string num = (json.text.Substring(numi + 6, numcommai - numi - 6));
				int.TryParse(num, out skini);
				skinID.Add(skini);
			}

			loops++;
		}

		// Passives & Spells
		i = json.text.IndexOf("\"passive\"");
		int pngi = 0;

		i = json.text.IndexOf("\"full\"", i);
		pngi = json.text.IndexOf(".png", i);
		p = json.text.Substring(i + 8, pngi - i - 4);

		i = json.text.IndexOf("\"spells\"");
		i = json.text.IndexOf("\"full\"", i);
		pngi = json.text.IndexOf(".png", i);
		q = json.text.Substring(i + 8, pngi - i - 4);

		i = json.text.IndexOf("\"full\"", i + 10);
		pngi = json.text.IndexOf(".png", i);
		w = json.text.Substring(i + 8, pngi - i - 4);

		i = json.text.IndexOf("\"full\"", i + 10);
		pngi = json.text.IndexOf(".png", i);
		e = json.text.Substring(i + 8, pngi - i - 4);

		i = json.text.IndexOf("\"full\"", i + 10);
		pngi = json.text.IndexOf(".png", i);
		r = json.text.Substring(i + 8, pngi - i - 4);

		yield break;
	}

	public void GetWebSpell(string s, string urlType)
	{
		uploader.GetWebImage(urlType + s, s);
		spellImageEvent.Invoke();
		return;
	}

	public void GetWebSkin(int i)
	{
		string skinFileName = activeChampion + "_" + skinID[i] + ".jpg";
		uploader.GetWebImage(splashURL + skinFileName, skinFileName);
	}

	private void GenerateSkinButtons()
	{
		// Clear buttons
		for (int i = skinButtons.Count - 1; i >= 0; i--)
		{
			GameObject g = skinButtons[i];
			skinButtons.Remove(g);
			Destroy(g);
		}

		// Generate buttons
		for (int i = 0; i < skins.Count; i++)
		{
			GameObject b = Instantiate(skinButtonPrefab, skinContent);
			skinButtons.Add(b);
			b.name = skinID[i].ToString();
			TextMeshProUGUI tmp = b.GetComponentInChildren<TextMeshProUGUI>();
			tmp.text = i == 0 ? "Classic" : skins[i];

			int locali = i;
			Button btn = b.GetComponentInChildren<Button>();
			btn.onClick.AddListener(() => GetWebSkin(locali));
		}
	}

	private void GenerateSpellButtons()
	{
		GenerateSpellButton(p, "'s Passive Icon", passiveURL.Replace("PATCH_VERSION", patchVersion));
		GenerateSpellButton(q, "'s Q Icon", spellURL.Replace("PATCH_VERSION", patchVersion));
		GenerateSpellButton(w, "'s W Icon", spellURL.Replace("PATCH_VERSION", patchVersion));
		GenerateSpellButton(e, "'s E Icon", spellURL.Replace("PATCH_VERSION", patchVersion));
		GenerateSpellButton(r, "'s R Icon", spellURL.Replace("PATCH_VERSION", patchVersion));
	}

	private void GenerateSpellButton(string s, string desc, string urlType)
	{
		GameObject g = Instantiate(spellButtonPrefab, skinContent);
		skinButtons.Add(g);
		g.name = s;
		TextMeshProUGUI tmpP = g.GetComponentInChildren<TextMeshProUGUI>();
		tmpP.text = activeChampion + desc;

		Button btnP = g.GetComponentInChildren<Button>();
		btnP.onClick.AddListener(() => GetWebSpell(s, urlType));
	}
}