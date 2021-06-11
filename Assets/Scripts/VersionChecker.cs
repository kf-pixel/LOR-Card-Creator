using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;
using TMPro;
public class VersionChecker : MonoBehaviour
{
	[SerializeField] private string url = "https://raw.githubusercontent.com/kf-pixel/LOR-Card-Creator/master/version.txt";
	[SerializeField] private TextMeshProUGUI textField;

    private void Start()
    {
		CheckVersion();
	}

	public async void CheckVersion()
	{
		string versionWeb = await GetWebText(url);
		string versionApplication = Application.version;

		if (versionWeb != null)
		{
            textField.text = versionWeb;

            // Convert to floats
			float versionWebFloat = 0f;
            float.TryParse(versionWeb, out versionWebFloat);

			float versionApplicationFloat = 0f;
			float.TryParse(versionApplication, out versionApplicationFloat);

            // Set Text Prompts
			if (versionApplicationFloat > 0 && versionWebFloat > 0)
			{
				string displayText = versionApplicationFloat < versionWebFloat ?
					"Current version: " + versionApplicationFloat + "; Latest version: " + versionWebFloat + "\nNew version available!  Download latest on kf-pixel.itch.io/lor-card-creator" :
					"<alpha=#44>Current version: " + versionApplicationFloat + ". Up to date.";

				textField.text = displayText;
			}
		}
	}

	public async Task<string> GetWebText(string url)
	{
		using (UnityWebRequest www = UnityWebRequest.Get(url))
		{
			// Send URL Request
			var asyncOp = www.SendWebRequest();

			// Await until it's done: 
			while (asyncOp.isDone == false)
			{
				await Task.Delay(1000 / 60);
			}

			// Read Results:
			if (www.isNetworkError || www.isHttpError)
			{
				// Log Error:

				// Exit
				return null;
			}
			else
			{
				// Else Return if Valid:
				return www.downloadHandler.text;
			}
		}
	}
}
