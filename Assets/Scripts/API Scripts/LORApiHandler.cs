using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using LORAPI;
using TMPro;
using System.Text.RegularExpressions;
using System.Linq;

public class LORAPIHandler : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private TextAsset[] setsJSON;
	[SerializeField] private bool webVersion;
	[SerializeField] private bool getImage = true;
	[SerializeField] private bool getText = true;
	private bool apiEnabled;

	[Header("Components")]
	[SerializeField] private CardCode code;
	[SerializeField] private FileUpload imageUploader;
	[SerializeField] private ListManager listManager;
	[SerializeField] private TMP_InputField submitField;
	[SerializeField] private TMP_InputField webURLField;
	[SerializeField] private GameObject urlWarningGameObject;
	[SerializeField] private GameObject loadSetWarningGameObject;
	private UnityAction<string> submitAction;
	[HideInInspector] public LORCardSet allCards = new LORCardSet();

	[Header("Input Fields")]
	[SerializeField] private TMP_InputField manaF;
	[SerializeField] private TMP_InputField attackF;
	[SerializeField] private TMP_InputField healthF;
	[SerializeField] private TMP_InputField titleF;
	[SerializeField] private TMP_InputField groupF;
	[SerializeField] private TMP_InputField cardTextF;
	[SerializeField] private TMP_InputField levelUpTextF;
	[SerializeField] private Slider artworkScaleSlider;
	[SerializeField] private RectTransform[] artworkRectTransforms;

	[Header("Scriptable Object Data")]
	[SerializeField] private IntVariable cardType;
	[SerializeField] private IntVariable region, region2;
	[SerializeField] private IntVariable rarity;
	[SerializeField] private StringVariable mana;
	[SerializeField] private StringVariable attack;
	[SerializeField] private StringVariable health;
	[SerializeField] private StringVariable title;
	[SerializeField] private StringVariable group;
	[SerializeField] private StringVariable cardText;
	[SerializeField] private StringVariable levelUp;

	[Header("Translation Data")]
	[SerializeField] private SpritesVariable regionsList;
	[SerializeField] private GameObjectVariableList keywords;
	[SerializeField] private IntEvent effectIntAdd;
	[SerializeField] private GameEvent[] updateEvents;
	private List<string> keywordNames = new List<string>();

	// internal
	private string bluePattern = "(?<=<style=AssociatedCard>)[^<>]*(?=</style>)";
	private bool cardArtLoaded = false;

	public void EnableAPI()
	{
		apiEnabled = true;
	}

	public void SetWebVersion(bool b)
	{
		webVersion = b;
	}

	public void ToggleGetImage(bool b)
	{
		getImage = b;
	}

	public void ToggleGetText(bool b)
	{
		getText = b;
	}

	public void CardArtLoaded()
	{
		cardArtLoaded = true;
	}

	private void Start()
    {
		allCards.cards = new List<LORCard>();
		StartCoroutine(DeserializeCardSetsIE());

		// Generate keyword names from GameObject prefabs
		foreach (GameObject keywordPrefab in keywords.value)
		{
			keywordNames.Add(keywordPrefab.name);
		}

		// Attach FindCard to submit input field
		submitAction += FindCard;
		submitField.onSubmit.AddListener(submitAction);
	}

	private IEnumerator DeserializeCardSetsIE()
	{
		FrameRateManager.Instance.RequestFullFrameRate();
		foreach (TextAsset setAsset in setsJSON)
		{
			string setString = setAsset.text;
			object data = JsonUtility.FromJson<LORCardSet>(setString);

			// Load valid data
			if (data != null)
			{
				LORCardSet set = (LORCardSet)data;
				allCards.cards.AddRange(set.cards);
			}
			yield return new WaitForEndOfFrame();
		}
	}

	public void FindCard(string input)
	{
		// get set bool
		bool loadSet = input.EndsWith(" set");
		if (loadSet)
		{
			input = input.Remove(input.Length - 4, 4);
		}

		// get digit on end
		int token = 0;
		Match match = Regex.Match(input, "\\d*$");
		if (match.Length > 0)
		{
			int.TryParse(match.Value, out token);
			input = input.Replace(match.Value, "");
			token--;
		}
		input = input.Trim(' ');

		// clean input
		string inputLower = input.ToLower();
		List<LORCard> cards = new List<LORCard>();

		// load set redirect
		if (loadSet)
		{
			// Find the first matching card
			LORCard firstCard = null;
			int firstCardIndex = 0;

			for (int i = 0; i < allCards.cards.Count; i++)
			{
				if (allCards.cards[i].name.ToLower() == inputLower)
				{
					firstCard = allCards.cards[i];
					firstCardIndex = i;
					break;
				}
			}

			if (firstCard != null)
			{
				// Trim the token tag if it has one
				string cardID = firstCard.cardCode;
				Match tokenID = Regex.Match(cardID, "(?<=\\d)T\\d*$");
				if (tokenID.Success)
				{
					cardID = cardID.Replace(tokenID.Value, "");
				}

				cards.Add(firstCard);

				// Look at the next 30 cards after the first card, add them
				for (int i = firstCardIndex + 1; i < firstCardIndex + 30 && i < allCards.cards.Count; i++)
				{
					if (i > allCards.cards.Count) break;

					if (allCards.cards[i].cardCode.StartsWith(cardID))
					{
						cards.Add(allCards.cards[i]);
					}
				}
			}

			//Debug.Log($"cards found: {cards.Count} cards");
		}
		else
		{
			cards = allCards.cards.FindAll(x => x.name.ToLower() == inputLower);
		}

		// single card result
		if (cards.Count == 1)
		{
			PopulateFields(cards[0]);
			submitField.SetTextWithoutNotify("");
			return;
		}
		// multi card result
		else if (cards.Count > 1)
		{
			// card list sorting
			cards = cards.OrderBy(x => (x.type == "Unit" ? 0 : 100) + (x.type == "Unit" && x.supertype == "Champion" ? -100 : 0) + x.health + x.attack + x.cost).ToList();

			if (loadSet)
			{
				StartCoroutine(LoadCardSet(cards));
			}
			else // load single card by token index
			{
				token = Mathf.Clamp(token, 0, cards.Count - 1);
				PopulateFields(cards[token], token);
			}
			submitField.SetTextWithoutNotify("");
			return;
		}
			// else no card result
	}

	public IEnumerator LoadCardSet(List<LORCard> cardList)
	{
		loadSetWarningGameObject.SetActive(true);
		FrameRateManager.Instance.DisableFRManager();

		int champNumber = -1;
		for (int i = 0; i < cardList.Count; i++)
		{

			listManager.AddCard();
			yield return new WaitForEndOfFrame();

			if (cardList[i].supertype == "Champion" && cardList[i].type == "Unit")
			{
				champNumber++;
			}
			cardArtLoaded = false;
			PopulateFields(cardList[i], champNumber);

			if (!webVersion) yield return new WaitUntil(() => cardArtLoaded);
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();

			foreach (RectTransform rt in artworkRectTransforms)
			{
				rt.anchoredPosition = Vector3.zero;
			}

			yield return new WaitForEndOfFrame();
		}

		loadSetWarningGameObject.SetActive(false);
		FrameRateManager.Instance.EnableFRManager();
	}

	private void PopulateFields(LORCard cardData, int championTokenNumber = 0)
	{
		// reset card
		if (getText)
		{
			code.WriteInput("Untitled: 1 Mana 0/1 Runeterra Follower");
		}

		// Upload artwork
		if (getImage && (apiEnabled || webVersion))
		{
			// give artwork URL on web
			if (webVersion)
			{
				webURLField.text = cardData.assets[0].fullAbsolutePath;
				urlWarningGameObject.SetActive(false);
				urlWarningGameObject.SetActive(true);
			}
			else // get image normally
			{
				foreach (RectTransform rt in artworkRectTransforms)
				{
					rt.anchoredPosition = Vector3.zero;
				}
				imageUploader.GetWebImage(cardData.assets[0].fullAbsolutePath, cardData.cardCode);
				artworkScaleSlider.value = 850;
			}
		}
		else
		{
			cardArtLoaded = true;
		}

		// stop here if we dont need card text
		if (!getText)
		{
			return;
		}

		// common card fields
		manaF.text = cardData.cost.ToString();
		titleF.text = cardData.name;

		// get blue text from description, add to rawdescription
		MatchCollection blueTexts = Regex.Matches(cardData.description, bluePattern);
		string cardDesc = cardData.descriptionRaw;
		if (blueTexts.Count > 0)
		{
			foreach (Match m in blueTexts)
			{
				cardDesc = cardDesc.Replace(m.Value, $"[{m.Value}]");
			}
		}
		cardTextF.text = cardDesc;

		// get blue level up text
		if (!string.IsNullOrEmpty(cardData.levelupDescriptionRaw))
		{
			MatchCollection blueLevelUpTexts = Regex.Matches(cardData.levelupDescription, bluePattern);
			string cardLevelUpDesc = cardData.levelupDescriptionRaw;
			if (blueLevelUpTexts.Count > 0)
			{
				foreach (Match ml in blueLevelUpTexts)
				{
					cardLevelUpDesc = cardLevelUpDesc.Replace(ml.Value, $"[{ml.Value}]");
				}
			}
			levelUpTextF.text = cardLevelUpDesc;
		}

		// subtype finding
		if (string.IsNullOrEmpty(cardData.subtype))
		{
			groupF.text = cardData.subtype;
		}
		else if (cardData.subtypes.Count > 0)
		{
			groupF.text = cardData.subtypes[0];
		}

		// region
		if (cardData.regions.Count > 0)
		{
			string cardRegion1 = cardData.regionRefs[0];
			for (int i = 0; i < regionsList.values.Count; i++)
			{
				if (regionsList.values[i].name.EndsWith(cardRegion1))
				{
					region.value = i;
					break;
				}
			}

			// dual region
			if (cardData.regions.Count > 1)
			{
				string cardRegion2 = cardData.regionRefs[1];
				for (int i2 = 0; i2 < regionsList.values.Count; i2++)
				{
					if (regionsList.values[i2].name.EndsWith(cardRegion2))
					{
						region2.value = i2;
						break;
					}
				}
			}
		}

		// rarity sorting
		string cardRarity = cardData.rarity;
		if (cardData.collectible == false || cardRarity == "Champion")
		{
			rarity.value = 0;
		}
		else
		{
			if (cardRarity == "COMMON") rarity.value = 1;
			else if (cardRarity == "RARE") rarity.value = 2;
			else rarity.value = 3;
		}

		// card type sorting
		if (cardData.type == "Unit")
		{
			healthF.text = cardData.health.ToString();
			attackF.text = cardData.attack.ToString();

			// champion data
			if (cardData.supertype == "Champion")
			{
				if (championTokenNumber == 0) cardType.value = 1;
				else if (championTokenNumber == 1) cardType.value = 2;
				else cardType.value = 8;
			}
			else cardType.value = 0;
		}
		else if (cardData.type == "Spell" || cardData.type == "Ability")
		{
			if (cardData.keywords.Count > 0)
			{
				if (cardData.keywords.Contains("Slow")) cardType.value = 3;
				else if (cardData.keywords.Contains("Fast")) cardType.value = 4;
				else if (cardData.keywords.Contains("Burst")) cardType.value = 5;
				else if (cardData.keywords.Contains("Skill")) cardType.value = 6;
				else if (cardData.keywords.Contains("Focus")) cardType.value = 9;
			}

			artworkScaleSlider.value = 250;
		}
		else if (cardData.type == "Landmark")
		{
			cardType.value = 7;
			if (getImage && (apiEnabled || webVersion)) artworkScaleSlider.value = 1200;
		}

		// Keywords adding
		foreach (string key in cardData.keywords)
		{
			string keyNS = key.Replace(" ", "");
			for (int i = 0; i < keywords.value.Count; i++)
			{
				if (keyNS == keywords.value[i].name.Replace(" ", ""))
				{
					effectIntAdd.Raise(i);
					break;
				}
			}
		}

		// Skill mark adding (on string start only)
		if (cardData.description.StartsWith("<link=keyword.AttackSkillMark>") || cardData.description.StartsWith("<link=keyword.PlaySkillMark>"))
		{
			cardTextF.text = $"@{cardTextF.text}";
		}

		StartCoroutine(RefreshEvents());
	}

	private IEnumerator RefreshEvents()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		foreach (GameEvent ge in updateEvents)
		{
			ge.Raise();
		}
	}
}
