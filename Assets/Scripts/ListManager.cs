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
	[Header("Active Input")]
	public List<ListItem> activeItem = new List<ListItem>();
	[HideInInspector] public ListItem activeHovering;
	[HideInInspector] public ListItem activeDragging;
	[SerializeField] private BoolVariable shiftDown;
	[SerializeField] private BoolVariable ctrlDown;

	[Header("Prefab GameObjects")]
	[SerializeField] private GameObject listItemPrefab;
	[SerializeField] private GameObject folderPrefab;

	[Header("Region/CardType data to add on newly created cards.")]
	[SerializeField] private string initialPlaceHolderText;
	[SerializeField] private IntVariable regionIndex, regionIndex2;
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
	[SerializeField] private UnityEvent multiCardExportEvent, nonMultiCardExportEvent;

	private void Start()
	{
		if (activeSaveData.cardData.Count == 0)
		{
			AddCard();
			listItemInstances[0].cardData.cardCode = initialPlaceHolderText;
			cardCode.WriteInput(initialPlaceHolderText);
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
				if (currentAutoSaveCount > 3)
				{
					currentAutoSaveCount = 0;
				}
			}
		}

		// Time since last save calculate
		if (activeSaveData == null) return;
		if (Time.timeSinceLevelLoad - lastSaveTime < 2f)
		{
			tmpLastSave.text = "just saved!";
			return;
		}
		int minutesSinceLastSave = Mathf.FloorToInt((Time.timeSinceLevelLoad - lastSaveTime) / 60);
		tmpLastSave.text = minutesSinceLastSave == 0 ? "saved <1 minute ago" : "saved " + minutesSinceLastSave.ToString() + " minutes ago";
	}
#endif

	private void OnApplicationQuit()
	{
		QuickSave();
	}

	public void SetExport()
	{
		if (activeItem == null) return;
		if (activeItem.Count < 2) return;

		fileSaver.EncodeMultiple(activeItem);
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
				StartCoroutine(SetAsActive(it));
			}
			number++;
		}

		yield break;
	}

	private IEnumerator SetAsActive(ListItem item)
	{
		yield return new WaitForEndOfFrame();

		PointerDownEvent pt = item.GetComponent<PointerDownEvent>();
		if (pt != null)
		{
			pt.onPointerDown.Invoke();
		}
		else
		{
			Debug.Log("cannot find pointer script!");
		}
	}

	public void SetNewActiveItem(ListItem newItem, bool captureInput = true)
	{
		// null checking
		if (activeItem == null)
		{
			activeItem = new List<ListItem>();
		}
		if (activeItem.Count > 0)
		{
			if (activeItem[0] != null)
			{
				if (activeItem[0] == newItem && activeItem.Count == 0) return;
			}
		}

		// Regular click; clear list then add the new item as active
		if (!shiftDown.value && !ctrlDown.value || activeItem.Count == 0 || captureInput == false)
		{
			activeItem.Clear();
			activeItem.Add(newItem);
			cardCode.WriteInput(newItem.cardData.cardCode);

			// Dehighlight other items
			foreach (ListItem item in listItemInstances)
			{
				if (item != newItem) item.Dehighlight();
			}
		}
		// Multi-select cLICK, for export
		else if (ctrlDown.value && !shiftDown.value) // CTRL input, add if not in list, remove if in list
		{
			activeItem[0].Highlight();
			if (!activeItem.Contains(newItem))
			{
				activeItem.Insert(0, newItem);
				newItem.Highlight();
				cardCode.WriteInput(newItem.cardData.cardCode);
			}
			else if (activeItem.Count > 1)
			{
				// logic for to select last item if the removed item is the active item
				if (activeItem[0] == newItem)
				{
					ListItem lastActive = activeItem[activeItem.Count - 2];
					foreach (ListItem li in activeItem)
					{
						if (li.listOrderIndex > lastActive.listOrderIndex && li != newItem)
						{
							lastActive = li;
						}
					}

					// add item
					activeItem.Remove(lastActive);
					activeItem.Insert(0, lastActive);
					cardCode.WriteInput(lastActive.cardData.cardCode);
				}

				// remove item
				newItem.Dehighlight();
				activeItem.Remove(newItem);
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
		}
		else
		{
			nonMultiCardExportEvent.Invoke();
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

		// Clear actives
		activeItem = null;
		activeDragging = null;
		activeHovering = null;

		// Set as active item if none is active
		StartCoroutine(SetAsActive(it));
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
			StartCoroutine(SetAsActive(listItemInstances[0]));
		}

		// Check if we are at the max card limit
		if (listItemInstances.Count >= maxCards)
		{
			cardLimitEvent.Invoke();
		}
		else
		{
			nonCardLimitEvent.Invoke();
		}
	}

	public void DuplicateCard()
	{
		if (activeItem == null || listItemInstances.Count >= maxCards) return;

		// Spawn Prefab & set data
		CardDataObject card = new CardDataObject(activeItem[0].cardData);
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
		StartCoroutine(SetAsActive(it));
		activeSaveData.cardData.Add(card);

		// Check if we are at the max card limit
		if (listItemInstances.Count >= maxCards)
		{
			cardLimitEvent.Invoke();
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

	public void SetMaxCardsLimit(int count)
	{
		maxCards = count;
	}
}
