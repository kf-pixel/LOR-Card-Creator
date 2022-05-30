using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManager : MonoBehaviour
{

	public static void JSONSave(string path, object saveData)
	{
		// Create the save location if it doesn't exist
		if (!Directory.Exists(Application.persistentDataPath + "/saves"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/saves");
		}

		string json = JsonUtility.ToJson(saveData, true);

		File.WriteAllText(path, json);
	}

	public static object JSONLoad(string path)
	{
		if (!File.Exists(path))
		{
			Debug.Log("Cannot find file!");
			return null;
		}

		string json = File.ReadAllText(path);

		// Try open file
		try
		{
			object fileSave = JsonUtility.FromJson<SaveData>(json);
			return fileSave;
		}
		catch
		{
			Debug.Log("Failed to Load File");
			return null;
		}
	}





	// Unused Binary Formatter

	public static bool Save(string path, object saveData)
	{
		BinaryFormatter formatter = GetBinaryFormatter();

		// Create the save location if it doesn't exist
		if (!Directory.Exists(Application.persistentDataPath + "/saves"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/saves");
		}

		// Create file at the save data path
		FileStream file = File.Create(path);

		formatter.Serialize(file, saveData);
		file.Close();

		return true;
	}

	public static object Load(string path)
	{
		if (!File.Exists(path))
		{
			Debug.Log("Cannot find file!");
			return null;
		}

		// Get file
		BinaryFormatter formatter = GetBinaryFormatter();
		FileStream file = File.Open(path, FileMode.Open);

		// Try open file
		try
		{
			object fileSave = formatter.Deserialize(file);
			file.Close();
			return fileSave;
		}
		catch
		{
			Debug.Log("Failed to Load File");
			file.Close();
			return null;
		}
	}

	public static BinaryFormatter GetBinaryFormatter()
	{
		BinaryFormatter formatter = new BinaryFormatter();

		return formatter;
	}
}
