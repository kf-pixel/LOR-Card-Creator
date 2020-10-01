using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
	public string saveName = "Untitled Set";

	public List<CardDataObject> cardData;

	public List<CustomKeywordSaveData> customKeywordSaveData;
}
