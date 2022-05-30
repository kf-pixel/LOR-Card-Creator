using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using SFB;

public class ListManager : MonoBehaviour
{
	[Header("Active Input")]
	public List<ListItem> activeItem = new List<ListItem>();
	[HideInInspector] public ListItem activeHovering;
	[HideInInspector] public ListItem activeDragging;
	[SerializeField] private BoolVariable shiftDown;
	[SerializeField] private BoolVariable ctrlDown;

	[Header("Prefab GameObjects")]
	[SerializeField] private GameObject listItemPrefab;
	[SerializeField] private GameObject folderPrefab;

	[Header("Data Used for Cards")]
	[SerializeField] private string initialPlaceHolderText;
	[SerializeField] private IntVariable regionIndex, regionIndex2;
	[SerializeField] private StringListVariable cardNamesList;
	[SerializeField] private GameEvent updateTextEvent;
	public string[] regionNames;
	public string[] cardTypeNames;
	public string[] rarityNames;

	[Header("Spawned List GameObjects")]
	[SerializeField] private List<ListItem> listItemInstances = new List<ListItem>();

	[Header("UI")]
	[SerializeField] private TextMeshProUGUI uiSaveNameDisplay;
	[SerializeField] private TMP_InputField inputFieldCardData;
	[SerializeField] private CardCode cardCode;
	[SerializeField] private Transform content;
	[SerializeField] private Tooltip quickCompTooltip;

	[Header("CustomKeyword UI")]
	[SerializeField] private TMP_InputField[] KWLabels;
	[SerializeField] private TMP_InputField[] KWDescriptions;
	[SerializeField] private TMP_Dropdown[] KWIndexDropdowns;
	[SerializeField] private KeywordColourHexInput[] KWHexColourInputs;

	[Header("Custom Keyword Save Data")]
	public CustomKeywordData[] KWData;
	private int KWCount = 5;

	[Header("Autosaving")]
	public float autoSaveTime = 60f;
	private float currentAutoSaveTimer = 0f;
	private int currentAutoSaveCount = 0;
	private bool appQuitSave = true;

	[Header("Saved Data")]
	public string defaultSaveName = "Untitled Set";
	private int softMaxCards = 100;
	public int maxCards = 300;
	public string activeSaveDataPath;
	public SaveData activeSaveData;
	[SerializeField] private TextMeshProUGUI tmpLastSave;
	private float lastSaveTime;

	[Header("Set Export")]
	[SerializeField] private FileSave fileSaver;

	[Header("Events")]
	[SerializeField] private UnityEvent onLoad;
	[SerializeField] private UnityEvent onSave;
	[SerializeField] private UnityEvent restart;
	[SerializeField] private UnityEvent noSaveEvent;
	[SerializeField] private UnityEvent loadSetConfirmEvent;
	[SerializeField] private UnityEvent cardLimitEvent;
	[SerializeField] private UnityEvent nonCardLimitEvent;
	[SerializeField] private UnityEvent softCardLimitEvent;
	[SerializeField] private UnityEvent multiCardExportEvent, nonMultiCardExportEvent;

	private void Start()
	{
		if (activeSaveData.cardData.Count == 0)
		{
			AddCard();
			listItemInstances[0].cardData.cardCode = initialPlaceHolderText;
			//cardCode.WriteInput(initialPlaceHolderText);
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
		if (!Application.isFocused) return;

		// Auto save countdown
		if (currentAutoSaveTimer < autoSaveTime)
		{
			currentAutoSaveTimer += Time.deltaTime;
			if (currentAutoSaveTimer > autoSaveTime)
			{
				currentAutoSaveCount++;
				AutoSave();

				currentAutoSaveTimer = 0f;
				if (currentAutoSaveCount > 5)
				{
					currentAutoSaveCount = 0;
				}
			}
		}

		// Time since last save calculate
		if (activeSaveData == null) return;
		if (Time.timeSinceLevelLoad - lastSaveTime < 2f)
		{
			tmpLastSave.text = "Quick Save<i><alpha=#88>  just saved!";
			return;
		}
		int minutesSinceLastSave = Mathf.FloorToInt((Time.timeSinceLevelLoad - lastSaveTime) / 60);
		tmpLastSave.text = minutesSinceLastSave == 0 ? "Quick Save<i><alpha=#88>  last saved <1 min. ago" : "Quick Save<i><alpha=#88>  last saved " + minutesSinceLastSave.ToString() + " min. ago";
	}
#endif

	private void OnApplicationQuit()
	{
		if (appQuitSave)
		{
			QuickSave();
		}
	}

	public void SetApplicationQuitSave(bool b)
	{
		appQuitSave = b;
	}

	public bool HasMultipleItems()
	{
		if (activeItem == null) return false;
		if (activeItem.Count < 2) return false;

		return true;
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

			// Modify to UI Panels
			KWLabels[i].SetTextWithoutNotify(KWData[i].label);
			KWDescriptions[i].SetTextWithoutNotify(KWData[i].description);
			KWIndexDropdowns[i].SetValueWithoutNotify(KWData[i].spriteIndex);
			KWHexColourInputs[i].HexDataToRGB();
		}
	}

	private void SerializationSave(string path)
	{
		SerializationManager.JSONSave(path, activeSaveData);
		lastSaveTime = Time.timeSinceLevelLoad;
	}

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
			SerializationSave(Application.persistentDataPath + "/autosaves/" + autoSaveName);
		}
	}

	public void QuickSave()
	{
		if (activeSaveDataPath != null)
		{
			if (File.Exists(activeSaveDataPath))
			{
				SerializationSave(activeSaveDataPath);
			}
		}
	}

	public void OnSaveAndNew() // on new set button
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

		SerializationSave(path);

		// Update path
		activeSaveDataPath = path;

		// Update Set Name Display with path name
		int pathFileIndex = path.LastIndexOf("\\") + 1;
		if (pathFileIndex > 0)
		{
			activeSaveData.saveName = path.Substring(pathFileIndex).Replace(".json", "");
			uiSaveNameDisplay.text = "- " + activeSaveData.saveName + " -";
		}
		else uiSaveNameDisplay.text = "-";

		// Event
		onSave.Invoke();
	}

	public void OnLoadSet(bool forceLoad)
	{
		if (string.IsNullOrEmpty(activeSaveDataPath) || forceLoad == true)
		{
			LoadSaveData();
		}
		else
		{
			loadSetConfirmEvent.Invoke();
		}
	}

	private void LoadSaveData() // opens window on ^ 'load set' button
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

    private void LoadSaveData(string path)
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
		lastSaveTime = Time.timeSinceLevelLoad;

		if (loadedSave != null)
		{
			activeSaveData = loadedSave;

			// Load Keyword data
			LoadCustomKeywordSaveData();

			// Update Set Name Display with path name
			int pathFileIndex = path.LastIndexOf("\\") + 1;
			if (pathFileIndex > 0)
			{
				activeSaveData.saveName = path.Substring(pathFileIndex).Replace(".json", "");
				uiSaveNameDisplay.text = "- " + activeSaveData.saveName + " -";
			}
			else uiSaveNameDisplay.text = "-";

			// Start loading and building the card list
			ClearActiveCards();
			StartCoroutine(LoadAllCards());
		}

		// Clear Actives
		
	}

	private IEnumerator LoadAllCards()
	{
		int number = 0;
		bool setNewActive = false;
		foreach(CardDataObject card in activeSaveData.cardData)
		{
			// Set Card name
			card.SetName();

			// Set Card ID if empty
			if (string.IsNullOrEmpty(card.id))
			{
				card.GenerateID();
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
				StartCoroutine(SetAsActive(it));
			}
			number++;
		}

		// Update Card Names List
		cardNamesList.values.Clear();
		foreach (ListItem it in listItemInstances)
		{
			AddCardName(it);
		}

		yield break;
	}

	private IEnumerator SetAsActive(ListItem item)
	{
		yield return new WaitForEndOfFrame();

		item.SetAsActiveItem();
	}

	public void SetNewActiveItem(ListItem newItem, bool captureInput = true)
	{
		// null checking
		if (activeItem == null)
		{
			activeItem = new List<ListItem>();
		}

		// Regular click; clear list then add the new item as active
		if (!shiftDown.value && !ctrlDown.value || activeItem.Count == 0 || captureInput == false)
		{
			foreach (ListItem item in activeItem)
			{
				item.Dehighlight();
			}

			// Set new active
			activeItem.Clear();
			activeItem.Add(newItem);
			cardCode.WriteInput(newItem.cardData.cardCode);
			newItem.Highlight();
		}
		// Ctrl click, for multi export and quickcomp
		else if (ctrlDown.value && !shiftDown.value) // CTRL input, add if not in list, remove if in list
		{
			if (!activeItem.Contains(newItem)) // add item 
			{
				activeItem.Insert(0, newItem);
				newItem.Highlight();
				cardCode.WriteInput(newItem.cardData.cardCode);
			}
			else if (activeItem.Count > 1) // remove item
			{
				newItem.Dehighlight();
				activeItem.Remove(newItem);
				cardCode.WriteInput(activeItem[0].cardData.cardCode);
			}
		}
		else if (shiftDown.value) // SHIFT input, gather all in between orderIndex
		{
			int startIndex = Mathf.Min(activeItem[0].listOrderIndex, newItem.listOrderIndex);
			int endIndex = Mathf.Max(activeItem[0].listOrderIndex, newItem.listOrderIndex);

			if (startIndex != endIndex)
			{
				activeItem[0].Highlight();
				for (int i = startIndex; i < endIndex; i++)
				{
					ListItem n = listItemInstances[i];
					if (!activeItem.Contains(n))
					{
						activeItem.Insert(0, n);
						n.Highlight();
					}
				}

				// Set new active
				activeItem.Remove(newItem);
				activeItem.Insert(0, newItem);
				newItem.Highlight();
				cardCode.WriteInput(newItem.cardData.cardCode);
			}
		}

		// Check if there are multiple cards selected
		if (activeItem.Count > 1)
		{
			multiCardExportEvent.Invoke();
			int quickCompLayoutCount = Mathf.Min(activeItem.Count, fileSaver.compLayouts.Length - 1);
			quickCompTooltip.NewContentAppend($"<br><b>{quickCompLayoutCount}</b> cards in a <b>[{fileSaver.compLayouts[quickCompLayoutCount].x},{fileSaver.compLayouts[quickCompLayoutCount].y}]</b> grid.");

			foreach (ListItem item in activeItem)
			{
				item.DisableDuplicateGameObject();
			}
		}
		else
		{
			nonMultiCardExportEvent.Invoke();
			foreach (ListItem item in activeItem)
			{
				item.EnableDuplicateGameObject();
			}
		}
	}

	public void UpdateActiveCardData()
	{
		if (activeItem == null)
		{
			activeItem = new List<ListItem>();
			return;
		}
		if (activeItem.Count == 0) return;
		if (activeItem[0] == null) return;

		activeItem[0].cardData.cardCode = inputFieldCardData.text;
		activeItem[0].cardData.SetName();
		activeItem[0].UpdateLabel();
		UpdateActiveCardName();
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

	public void AddCard()
	{
		// Check if we are at the max card limit
		if (listItemInstances.Count >= maxCards - 1)
		{
			cardLimitEvent.Invoke();
		}
		else if (listItemInstances.Count >= softMaxCards)
		{
			softCardLimitEvent.Invoke();
		}
		else
		{
			nonCardLimitEvent.Invoke();
		}
		if (listItemInstances.Count >= maxCards) return;


		// Spawn Prefab & set data
		CardDataObject card = new CardDataObject(regionNames[regionIndex.value]);
		if (regionIndex2.value != 13)
		{
			card.cardCode += " ++" + regionNames[regionIndex2.value];
		}
		card.SetName();
		card.GenerateID();

		GameObject newItem = Instantiate(listItemPrefab, content);
		ListItem it = newItem.GetComponent<ListItem>();
		it.cardData = card;
		it.listManager = this;
		it.listOrderIndex = activeSaveData.cardData.Count;

		// Add to spawned instances
		listItemInstances.Add(it);
		AddCardName(it);

		// Clear actives
		ClearActiveCards();

		// Set as active item if none is active
		StartCoroutine(SetAsActive(it));
		activeSaveData.cardData.Add(card);
	}

	public void RemoveCard()
	{
		if (!activeDragging) return;

		// Return if only 1 listItems are left
		if (listItemInstances.Count < 2) return;

		int newIndex = 0;
		if (activeDragging != null) newIndex = activeDragging.listOrderIndex;

		ListItem toBeRemoved = activeDragging;
		activeSaveData.cardData.Remove(toBeRemoved.cardData);
		listItemInstances.Remove(toBeRemoved);
		RemoveCardName(toBeRemoved.listOrderIndex);

		Destroy(toBeRemoved.gameObject);

		// Update their ListItem indexes
		UpdateIndexOrders();

		// Clear actives
		ClearActiveCards();

		// Set new active
		if (listItemInstances.Count > 0)
		{
			StartCoroutine(SetAsActive(listItemInstances[Mathf.Clamp(newIndex - 1, 0, listItemInstances.Count - 1)]));
		}

		// Update index orders
		UpdateIndexOrders();

		// Check if we are at the max card limit
		if (listItemInstances.Count >= maxCards)
		{
			cardLimitEvent.Invoke();
		}
		else if (listItemInstances.Count >= softMaxCards)
		{
			softCardLimitEvent.Invoke();
		}
		else
		{
			nonCardLimitEvent.Invoke();
		}
	}

	public void RemoveAllSelected()
	{
		List<int> cardNamesToBeRemoved = new List<int>();
		for (int i = activeItem.Count - 1; i >= 0 ; i--)
		{
			activeSaveData.cardData.Remove(activeItem[i].cardData);
			listItemInstances.Remove(activeItem[i]);
			cardNamesToBeRemoved.Add(activeItem[i].listOrderIndex);

			Destroy(activeItem[i].gameObject);
			activeItem.RemoveAt(i);

			if (listItemInstances.Count < 2) break;
		}

		// Sort and remove CardNamesList
		cardNamesToBeRemoved.Sort();
		for (int i = cardNamesToBeRemoved.Count - 1; i >= 0 ; i--)
		{
			RemoveCardName(cardNamesToBeRemoved[i]);
		}

		// Update their ListItem indexes
		UpdateIndexOrders();

		// Clear actives
		ClearActiveCards();

		// Set new active
		if (listItemInstances.Count > 0)
		{
			StartCoroutine(SetAsActive(listItemInstances[0]));
		}

		// Update index orders
		UpdateIndexOrders();

		// Check if we are at the max card limit
		if (listItemInstances.Count >= maxCards)
		{
			cardLimitEvent.Invoke();
		}
		else if (listItemInstances.Count >= softMaxCards)
		{
			softCardLimitEvent.Invoke();
		}
		else
		{
			nonCardLimitEvent.Invoke();
		}
	}

	public void DuplicateCard()
	{
		if (activeItem == null || listItemInstances.Count >= maxCards) return;
		if (activeItem.Count < 1) return;

		// Spawn Prefab & set data
		CardDataObject card = new CardDataObject(activeItem[0].cardData);
		card.SetName();
		card.GenerateID();

		GameObject newItem = Instantiate(listItemPrefab, content);
		ListItem it = newItem.GetComponent<ListItem>();
		it.cardData = card;
		it.listManager = this;
		it.listOrderIndex = listItemInstances.Count;

		// Add to spawned instances
		listItemInstances.Add(it);
		AddCardName(it);

		// Position duplicate item
		RepositionItemIndex(it, activeItem[0].listOrderIndex);

		// Clear actives
		ClearActiveCards();

		// Set as active item if none is active
		StartCoroutine(SetAsActive(it));
		//activeSaveData.cardData.Add(card);

		// Update index orders
		UpdateIndexOrders();

		// Check if we are at the max card limit
		if (listItemInstances.Count >= maxCards)
		{
			cardLimitEvent.Invoke();
		}
		else if (listItemInstances.Count >= softMaxCards)
		{
			softCardLimitEvent.Invoke();
		}
		else
		{
			nonCardLimitEvent.Invoke();
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
		int newItemIndex = positionAboveHovering ? activeHovering.listOrderIndex - 1 : activeHovering.listOrderIndex;
		if (newItemIndex < 0)
		{
			newItemIndex = 0;
		}

		RepositionItemIndex(draggedItem, newItemIndex);
	}

	private void RepositionItemIndex(ListItem draggedItem, int newItemIndex)
	{
		newItemIndex = Mathf.Min(newItemIndex, listItemInstances.Count);

		// Remove and Insert at dragged position in the data list
		activeSaveData.cardData.Remove(draggedItem.cardData);
		activeSaveData.cardData.Insert(newItemIndex, draggedItem.cardData);

		listItemInstances.Remove(draggedItem);
		listItemInstances.Insert(newItemIndex, draggedItem);

		RemoveCardName(draggedItem.listOrderIndex);
		InsertCardName(draggedItem, newItemIndex);

		// Reorder transforms
		draggedItem.transform.SetSiblingIndex(Mathf.Clamp(newItemIndex, 0, listItemInstances.Count));

		// Update index orders
		UpdateIndexOrders();
	}

	private void UpdateIndexOrders()
	{
		// Update their ListItem indexes
		for (int i = 0; i < listItemInstances.Count; i++)
		{
			listItemInstances[i].listOrderIndex = i;
			listItemInstances[i].UpdateOrderIndexOnly();
		}

	}





	// Helper Functions

	public void SetMaxCardsLimit(int count)
	{
		maxCards = count;
	}

	private void ClearActiveCards()
	{
		foreach (ListItem item in activeItem)
		{
			item?.Dehighlight();
		}

		activeItem.Clear();
		activeDragging = null;
		activeHovering = null;
	}

	// Card Names List Quick functions

	private void AddCardName(ListItem item)
	{
		cardNamesList.values.Add(item.cardData.cardName);
	}

	private void RemoveCardName(int id)
	{
		cardNamesList.values.RemoveAt(id);
	}

	private void InsertCardName(ListItem item, int id)
	{
		cardNamesList.values.Insert(id, item.cardData.cardName);
	}

	public void UpdateActiveCardName()
	{
		if (activeItem.Count > 0)
		{
			if (activeItem[0].listOrderIndex < cardNamesList.values.Count)
			{
				cardNamesList.values[activeItem[0].listOrderIndex] = activeItem[0].cardData.cardName;
				updateTextEvent.Raise();
			}
		}
	}

	public int GetListItemCount()
	{
		return listItemInstances.Count;
	}
}
