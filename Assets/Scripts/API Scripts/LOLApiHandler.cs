using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RiotAPI;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.UI;

[System.Serializable] public class UnityStringEvent : UnityEvent<string> { }

public class LOLApiHandler : MonoBehaviour
{
	public LOLChampionNamesAssets champList;
	[SerializeField] string activeChampion;
	public string p, q, w, e, r;
	public List<string> skins;
	public List<int> skinID;

	[Header("Scene Input Modifying")]
	[SerializeField] private string cardCodeDefaultText = "Untitled: 1 Mana Runeterra Follower";
	[SerializeField] private TextMeshProUGUI cardCodeText;

	[Header("Skin Dropdown Buttons")]
	[SerializeField] private Transform skinContent;
	[SerializeField] private GameObject skinButtonPrefab, spellButtonPrefab;
	[SerializeField] private List<GameObject> skinButtons = new List<GameObject>();

	// RIOT API URLs
	[Header("Image Push")]
	public FileUpload uploader;
	private string splashURL = "http://ddragon.leagueoflegends.com/cdn/img/champion/splash/";
	private string passiveURL = "http://ddragon.leagueoflegends.com/cdn/11.4.1/img/passive/";
	private string spellURL = "http://ddragon.leagueoflegends.com/cdn/11.4.1/img/spell/";

	[Header("Events")]
	[SerializeField] private UnityEvent spellImageEvent, blankChampPushEvent, blankSpellPushEvent;

	public void LoadChampionAsset(TextAsset asset)
	{
	    StartCoroutine(GetChampionImageData(asset));
	}

	public void LoadChampionAsset(string input)
	{
		foreach (ListNamesAssets champ in champList.championNamesAssets)
		{
			foreach (string s in champ.names)
			{
				if (Regex.IsMatch(input, "(^" + s + " |" + s + "$|" + s + " Prestige Edition$)", RegexOptions.IgnoreCase) == true)
				{
					skins.Clear();
					skinID.Clear();
					
					LoadChampionAsset(champ.asset);
					FindImageQuery(input, champ);

					ClearButtons();
					GenerateSkinButtons();
					GenerateSpellButtons();

					return;
				}
			}
		}
	}

	private void FindImageQuery(string input, ListNamesAssets champ)
	{
		string inputlower = input.ToLower();
		bool blankCard = cardCodeText.text.StartsWith(cardCodeDefaultText);

		// Passive IMG
		if (inputlower.EndsWith(" p") || inputlower.EndsWith("passive"))
		{
			UploadSpell(p, passiveURL);
			return;
		}

        // Spell IMGs
		if (inputlower.EndsWith(" q"))
		{
			UploadSpell(q, spellURL);
			return;
		}
		if (inputlower.EndsWith(" w"))
		{
			UploadSpell(w, spellURL);
			return;
		}
		if (inputlower.EndsWith(" e"))
		{
			UploadSpell(e, spellURL);
			return;
		}
		if (inputlower.EndsWith(" r"))
		{
			UploadSpell(r, spellURL);
			return;
		}

		// Skin Images
		for (int i = skins.Count - 1; i >= 1 ; i--)
		{
			string skinName = skins[i];
			foreach (string s in champ.names)
			{
				skinName = skinName.Replace(" " + s, "");
				skinName = skinName.Replace(s + " ", "");
			}

			if (inputlower.Replace(" ", "").Contains(skinName.ToLower().Replace(" ", "")))
			{
				UploadSkin(i);
				return;
			}
		}
		// Else load default skin
		string defaultSkin = activeChampion + "_0.jpg";
		uploader.GetWebImage(splashURL + defaultSkin, defaultSkin);

		// Push champ into title
		if (blankCard) blankChampPushEvent.Invoke();
	}

	public void UploadSpell(string s, string urlType)
	{
		bool blankCard = cardCodeText.text.StartsWith(cardCodeDefaultText);

		uploader.GetWebImage(urlType + s, s);
		if (blankCard) blankSpellPushEvent.Invoke();
		spellImageEvent.Invoke();
		return;
	}

	public void UploadSkin(int i)
	{
		string skinFileName = activeChampion + "_" + skinID[i] + ".jpg";
		uploader.GetWebImage(splashURL + skinFileName, skinFileName);
	}

	private IEnumerator GetChampionImageData(TextAsset champ)
	{
		activeChampion = champ.name;

		// add default skin
		skinID.Add(0);

		// Get List of Skin Names
		int i = champ.text.IndexOf("\"skins\"");
		int lorei = champ.text.IndexOf("\"lore\"");

		int loops = 0;
		while (i < lorei && loops < 30)
		{
			i = champ.text.IndexOf("\"name\"", i) + 8;
			if (i > lorei) break;
            
			int commai = champ.text.IndexOf(",", i);
			skins.Add(champ.text.Substring(i, commai - i - 1));

		
            // Add skins
			int numi = champ.text.IndexOf("\"num\":", i - 11);
			if (numi > -1)
			{
				int numcommai = champ.text.IndexOf(",", numi);
				int skini = 0;
				string num = (champ.text.Substring(numi + 6, numcommai - numi - 6));
				int.TryParse(num, out skini);
				skinID.Add(skini);
			}

			loops++;
		}

		// Passives & Spells
		i = champ.text.IndexOf("\"passive\"");
		int pngi = 0;

		i = champ.text.IndexOf("\"full\"", i);
		pngi = champ.text.IndexOf(".png", i);
		p = champ.text.Substring(i + 8, pngi - i - 4);

		i = champ.text.IndexOf("\"spells\"");
		i = champ.text.IndexOf("\"full\"", i);
		pngi = champ.text.IndexOf(".png", i);
		q = champ.text.Substring(i + 8, pngi - i - 4);

		i = champ.text.IndexOf("\"full\"", i + 10);
		pngi = champ.text.IndexOf(".png", i);
		w = champ.text.Substring(i + 8, pngi - i - 4);

		i = champ.text.IndexOf("\"full\"", i + 10);
		pngi = champ.text.IndexOf(".png", i);
		e = champ.text.Substring(i + 8, pngi - i - 4);

		i = champ.text.IndexOf("\"full\"", i + 10);
		pngi = champ.text.IndexOf(".png", i);
		r = champ.text.Substring(i + 8, pngi - i - 4);

		yield break;
	}


	public void ClearButtons()
	{
		for (int i = skinButtons.Count - 1; i >= 0 ; i--)
		{
			GameObject g = skinButtons[i];
			skinButtons.Remove(g);
			Destroy(g);
		}
	}

	public void GenerateSkinButtons()
	{
		for (int i = 0; i < skins.Count; i++)
		{
			GameObject b = Instantiate(skinButtonPrefab, skinContent);
			skinButtons.Add(b);
			b.name = skinID[i].ToString();
			TextMeshProUGUI tmp = b.GetComponentInChildren<TextMeshProUGUI>();
			tmp.text = i == 0 ? "Classic" : skins[i];

			int locali = i;
			Button btn = b.GetComponentInChildren<Button>();
			btn.onClick.AddListener(() => UploadSkin(locali));
		}
	}

	public void GenerateSpellButtons()
	{
		InstantiateSpellButton(p, "'s Passive Icon", passiveURL);
		InstantiateSpellButton(q, "'s Q Icon", spellURL);
		InstantiateSpellButton(w, "'s W Icon", spellURL);
		InstantiateSpellButton(e, "'s E Icon", spellURL);
		InstantiateSpellButton(r, "'s R Icon", spellURL);
	}

	public void InstantiateSpellButton(string s, string desc, string urlType)
	{
		GameObject g = Instantiate(spellButtonPrefab, skinContent);
		skinButtons.Add(g);
		g.name = s;
		TextMeshProUGUI tmpP = g.GetComponentInChildren<TextMeshProUGUI>();
		tmpP.text = activeChampion + desc;

		Button btnP = g.GetComponentInChildren<Button>();
		btnP.onClick.AddListener(() => UploadSpell(s, urlType));
	}
}