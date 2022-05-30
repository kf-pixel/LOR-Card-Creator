using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
	public string saveName = "Untitled Set";

	public List<CardDataObject> cardData;

	public List<CustomKeywordSaveData> customKeywordSaveData;
}
