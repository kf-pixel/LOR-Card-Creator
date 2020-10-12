using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using SFB;

public class ListManager : MonoBehaviour
{
	[Header("Prefab GameObjects")]
	[SerializeField] private GameObject listItemPrefab;
	[SerializeField] private GameObject folderPrefab;

	[Header("Region/CardType data to add on newly created cards.")]
	[SerializeField] private IntVariable regionIndex;
	public string[] regionNames;
	public string[] cardTypeNames;

	[Header("Spawned List GameObjects")]
	[SerializeField] private List<ListItem> listItemInstances = new List<ListItem>();

	[Header("UI")]
	[SerializeField] private TextMeshProUGUI uiSaveNameDisplay;
	[SerializeField] private TMP_InputField inputFieldCardData;
	[SerializeField] private CardCode cardCode;
	[SerializeField] private Transform content;

	[Header("CustomKeyword UI")]
	[SerializeField] private TMP_InputField[] KWLabels;
	[SerializeField] private TMP_InputField[] KWDescriptions;
	[SerializeField] private TMP_Dropdown[] KWIndexDropdowns;
	[SerializeField] private ColourHexInputter[] KWHexColourInputs;

	[Header("Custom Keyword Save Data")]
	public CustomKeywordData[] KWData;
	private int KWCount = 5;

	[Header("Autosaving")]
	public float autoSaveTime = 60f;
	private float currentAutoSaveTimer = 0f;
	private int currentAutoSaveCount = 0;

	[Header("Saved Data")]
	public string defaultSaveName = "Untitled Set";
	public int maxCards = 50;
	public ListItem activeItem;
	public ListItem activeHovering;
	public ListItem activeDragging;
	public string activeSaveDataPath;
	public SaveData activeSaveData;

	[Header("Events")]
	[SerializeField] private UnityEvent onLoad;
	[SerializeField] private UnityEvent onSave;
	[SerializeField] private UnityEvent restart;
	[SerializeField] private UnityEvent noSaveEvent;


	private void Start()
	{
		if (activeSaveData.cardData.Count == 0)
		{
			AddCard();
		}
		UpdateCustomKeywordSaveData();

#if UNITY_STANDALONE
		// Create Autosave folder
		if (!Directory.Exists(Application.persistentDataPath + "/autosaves"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/autosaves");
		}
#endif
	}

#if UNITY_STANDALONE
	private void Update()
	{
		if (Application.isFocused)
		{
			if (currentAutoSaveTimer < autoSaveTime)
			{
				currentAutoSaveTimer += Time.deltaTime;
				if (currentAutoSaveTimer > autoSaveTime)
				{
					currentAutoSaveCount++;
					AutoSave();

					currentAutoSaveTimer = 0f;
					if (currentAutoSaveCount > 3)
					{
						currentAutoSaveCount = 0;
					}
				}
			}
		}
	}
#endif

	private void AutoSave()
	{
		if (activeSaveData != null)
		{
			// Create Autosave folder
			if (!Directory.Exists(Application.persistentDataPath + "/autosaves"))
			{
				Directory.CreateDirectory(Application.persistentDataPath + "/autosaves");
			}

			string autoSaveName = "autosave" + currentAutoSaveCount + ".json";
			SerializationManager.JSONSave(Application.persistentDataPath + "/autosaves/" + autoSaveName, activeSaveData);
		}
	}

	public void QuickSave()
	{
		if (activeSaveDataPath != null)
		{
			if (File.Exists(activeSaveDataPath))
			{
				SerializationManager.JSONSave(activeSaveDataPath, activeSaveData);
			}
		}
	}

	public void SaveAndNew()
	{
		if (string.IsNullOrEmpty(activeSaveDataPath))
		{
			noSaveEvent.Invoke();
			return;
		}
		else if (!File.Exists(activeSaveDataPath))
		{
			noSaveEvent.Invoke();
			return;
		}
		StartCoroutine(SaveAndNewCoroutine());
	}

	public IEnumerator SaveAndNewCoroutine()
	{
		QuickSave();
		yield return new WaitForSeconds(0.4f);
		restart.Invoke();
	}

	private void InitializeKWList()
	{
		// Make List equal to 3 capacity if not already
		if (activeSaveData.customKeywordSaveData.Count != KWCount)
		{
			activeSaveData.customKeywordSaveData = new List<CustomKeywordSaveData>();
			for (int i = 0; i < KWCount; i++)
			{
				activeSaveData.customKeywordSaveData.Add(new CustomKeywordSaveData());
			}
		}
	}

	public void UpdateCustomKeywordSaveData()
	{
		InitializeKWList();

		// Copy keyword Data
		for (int i = 0; i < activeSaveData.customKeywordSaveData.Count; i++)
		{
			activeSaveData.customKeywordSaveData[i].label = KWData[i].label;
			activeSaveData.customKeywordSaveData[i].description = KWData[i].description;
			activeSaveData.customKeywordSaveData[i].hexColor = KWData[i].hexColor;

			activeSaveData.customKeywordSaveData[i].spriteIndex = KWData[i].spriteIndex;

			activeSaveData.customKeywordSaveData[i].colorR = KWData[i].colorR;
			activeSaveData.customKeywordSaveData[i].colorG = KWData[i].colorG;
			activeSaveData.customKeywordSaveData[i].colorB = KWData[i].colorB;
		}
	}

	public void LoadCustomKeywordSaveData()
	{
		InitializeKWList();

		// Copy keyword Data
		for (int i = 0; i < activeSaveData.customKeywordSaveData.Count; i++)
		{
			KWData[i].label = activeSaveData.customKeywordSaveData[i].label;
			KWData[i].description = activeSaveData.customKeywordSaveData[i].description;
			KWData[i].hexColor = activeSaveData.customKeywordSaveData[i].hexColor;

			KWData[i].spriteIndex = activeSaveData.customKeywordSaveData[i].spriteIndex;

			KWData[i].colorR = activeSaveData.customKeywordSaveData[i].colorR;
			KWData[i].colorG = activeSaveData.customKeywordSaveData[i].colorG;
			KWData[i].colorB = activeSaveData.customKeywordSaveData[i].colorB;

			print(activeSaveData.customKeywordSaveData[i].label);

			// Modify to UI Panels
			KWLabels[i].SetTextWithoutNotify(KWData[i].label);
			KWDescriptions[i].SetTextWithoutNotify(KWData[i].description);
			KWIndexDropdowns[i].SetValueWithoutNotify(KWData[i].spriteIndex);
			KWHexColourInputs[i].HexDataToRGB();
		}
	}

	public void WriteSaveData()
	{
		var extensions = new[]
		{
			new ExtensionFilter("JSON Save File", "json" ),
		};

		// Create the save location if it doesn't exist
		if (!Directory.Exists(Application.persistentDataPath + "/saves"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/saves");
		}
		string fileName = defaultSaveName;
		if (!string.IsNullOrEmpty(activeSaveData.saveName))
		{
			fileName = activeSaveData.saveName;
		}
		string path = StandaloneFileBrowser.SaveFilePanel("Save JSON File Location", Application.persistentDataPath + "/saves", fileName, extensions);

		if (!string.IsNullOrEmpty(path))
		{
			WriteSaveData(path);
		}
	}

	public void WriteSaveData(string path)
	{
		// Call the serialization manager to save
		if (string.IsNullOrEmpty(activeSaveData.saveName))
		{
			activeSaveData.saveName = defaultSaveName;
		}

		SerializationManager.JSONSave(path, activeSaveData);

		// Update path
		activeSaveDataPath = path;

		// Update Set Name Display with path name
		string saveSubfolder = "/saves/";
		if (path.IndexOf(saveSubfolder) < 0)
		{
			saveSubfolder = "\\saves\\";
		}
		activeSaveData.saveName = path.Substring(path.IndexOf(saveSubfolder) + 7).Replace(".json", "");
		uiSaveNameDisplay.text = activeSaveData.saveName;

		// Event
		onSave.Invoke();
	}

	public void LoadSaveData()
	{
		var extensions = new[]
		{
			new ExtensionFilter("JSON Save File", "json" ),
		};

		// Create the save location if it doesn't exist
		if (!Directory.Exists(Application.persistentDataPath + "/saves"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/saves");
		}
		string[] paths = StandaloneFileBrowser.OpenFilePanel("Open JSON Save File", Application.persistentDataPath + "/saves", extensions, false);

		if (paths != null)
		{
			if (paths.Length > 0)
			{
				if (paths[0].Length > 1)
				{
					LoadSaveData(paths[0]);
				}
			}
		}
	}

    public void LoadSaveData(string path)
	{
		// Clear Current GameObjects
		foreach(ListItem oldInstance in listItemInstances)
		{
			Destroy(oldInstance.gameObject);
		}
		listItemInstances.Clear();

		// Clear UI
		onLoad.Invoke();

		// Call the Serialization Manager to load
		SaveData loadedSave = (SaveData)SerializationManager.JSONLoad(path);
		activeSaveDataPath = path;

		if (loadedSave != null)
		{
			activeSaveData = loadedSave;

			// Load Keyword data
			LoadCustomKeywordSaveData();

			// Load Set Name Display with path name
			string saveSubfolder = "/saves/";
			if (path.IndexOf(saveSubfolder) < 0)
			{
				saveSubfolder = "\\saves\\";
			}
			activeSaveData.saveName = path.Substring(path.IndexOf(saveSubfolder) + 7).Replace(".json", "");
			uiSaveNameDisplay.text = activeSaveData.saveName;

			// Start loading and building the card list
			activeItem = null;
			StartCoroutine(LoadAllCards());
		}

		// Clear Actives
		activeItem = null;
		activeHovering = null;
		activeDragging = null;
	}

	private IEnumerator LoadAllCards()
	{
		int number = 0;
		bool setNewActive = false;
		foreach(CardDataObject card in activeSaveData.cardData)
		{
			// Set Card name if empty
			if (string.IsNullOrEmpty(card.cardName))
			{
				card.SetName();
			}

			// Set Card ID if empty
			if (string.IsNullOrEmpty(card.id))
			{
				card.GenerateID(number.ToString());
			}

			// Spawn Prefab & set data
			GameObject newItem = Instantiate(listItemPrefab, content);
			ListItem it = newItem.GetComponent<ListItem>();
			it.cardData = card;
			it.listManager = this;
			it.listOrderIndex = number;

			// Add to spawned instances
			listItemInstances.Add(it);

			// Set as active if none
			if (setNewActive == false)
			{
				setNewActive = true;
				StartCoroutine(SetFirstAsActiveOnLoad(it));
			}
			number++;
		}

		yield break;
	}

	private IEnumerator SetFirstAsActiveOnLoad(ListItem item)
	{
		yield return new WaitForSeconds(0.1f);

		Button bt = item.GetComponent<Button>();
		if (bt != null)
		{
			bt.onClick.Invoke();
		}
	}

	public void SetNewActiveItem(ListItem newItem)
	{
		if (activeItem == newItem) return;

		activeItem = newItem;

		//inputFieldCardData.text = activeItem.cardData.cardCode;
		cardCode.WriteInput(activeItem.cardData.cardCode);
	}

	public void SetNewHoveringItem(ListItem newHoveringItem)
	{
		if (activeHovering == newHoveringItem) return;

		activeHovering = newHoveringItem;
	}

	public void SetActiveDragging(ListItem newDragging)
	{
		activeDragging = newDragging;
	}

	public void ResetActiveDragging()
	{
		activeDragging = null;
	}

	public void UpdateActiveCardData()
	{
		if (activeItem == null) return;

		activeItem.cardData.cardCode = inputFieldCardData.text;
		activeItem.cardData.SetName();
		activeItem.UpdateLabel();
	}

	public void AddCard()
	{
		// Check if we are at the max card limit
		if (listItemInstances.Count >= maxCards) return;

		// Spawn Prefab & set data
		CardDataObject card = new CardDataObject(regionNames[regionIndex.value]);
		card.SetName();
		card.GenerateID();

		GameObject newItem = Instantiate(listItemPrefab, content);
		ListItem it = newItem.GetComponent<ListItem>();
		it.cardData = card;
		it.listManager = this;
		it.listOrderIndex = activeSaveData.cardData.Count;

		// Add to spawned instances
		listItemInstances.Add(it);

		// Clear actives
		activeItem = null;
		activeDragging = null;
		activeHovering = null;

		// Set as active item if none is active
		StartCoroutine(SetFirstAsActiveOnLoad(it));

		activeSaveData.cardData.Add(card);
	}

	public void RemoveCard()
	{
		if (!activeDragging) return;

		// Return if only 1 listItems are left
		if (listItemInstances.Count < 2) return;

		ListItem toBeRemoved = activeDragging;
		activeSaveData.cardData.Remove(toBeRemoved.cardData);
		listItemInstances.Remove(toBeRemoved);

		Destroy(toBeRemoved.gameObject);

		// Update their ListItem indexes
		int number = 0;
		foreach (ListItem cd in listItemInstances)
		{
			cd.listOrderIndex = number;
			number++;
		}

		// Clear actives
		activeItem = null;
		activeDragging = null;
		activeHovering = null;

		// Reset to first active item
		if (listItemInstances.Count > 0)
		{
			StartCoroutine(SetFirstAsActiveOnLoad(listItemInstances[0]));
		}

	}

	public void RepositionOnDrag(ListItem draggedItem, Vector3 mousePosition)
	{
		if (activeHovering == null) return;

		// Return if only 1 listItems are left
		if (listItemInstances.Count < 2) return;

		// Get index of position to drag the item
		bool positionAboveHovering = false;
		if (mousePosition.y > activeHovering.transform.position.y)
		{
			positionAboveHovering = true;
		}
		int newDraggedIndex = positionAboveHovering ? activeHovering.listOrderIndex - 1 : activeHovering.listOrderIndex;
		if (newDraggedIndex < 0)
		{
			newDraggedIndex = 0;
		}

		// Remove and Insert at dragged position in the data list
		activeSaveData.cardData.Remove(draggedItem.cardData);
		activeSaveData.cardData.Insert(newDraggedIndex, draggedItem.cardData);

		listItemInstances.Remove(draggedItem);
		listItemInstances.Insert(newDraggedIndex, draggedItem);

		// Reorder transforms
		draggedItem.transform.SetSiblingIndex(Mathf.Clamp(newDraggedIndex, 0, listItemInstances.Count));

		// Update their ListItem indexes
		int number = 0;
		foreach(ListItem cd in listItemInstances)
		{
			cd.listOrderIndex = number;
			cd.UpdateLabel();
			number++;
		}
	}
}
