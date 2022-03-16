using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Threading.Tasks;
using TMPro;
public class VersionChecker : MonoBehaviour
{
	private string url = "https://raw.githubusercontent.com/kf-pixel/LOR-Card-Creator/master/version.txt";
	private string api_url = "https://raw.githubusercontent.com/kf-pixel/LOR-Card-Creator/master/apienabled";
	[SerializeField] private Tooltip tooltip;
	[SerializeField] private Outline outline;
	[SerializeField] private TextMeshProUGUI textfield;
	[SerializeField] private UnityEvent apiEventEnabled, apiEventDisabled;

	private void Start()
    {
		CheckVersion();
		CheckAPIEnabled();
	}

	public async void CheckAPIEnabled()
	{
		string api_value = await GetWebText(api_url);

		if (api_value != null)
		{
			if (api_value.Contains("true"))
			{
				apiEventEnabled.Invoke();
			}
			else
			{
				apiEventDisabled.Invoke();
			}
		}
	}

	public async void CheckVersion()
	{
		string versionWeb = await GetWebText(url);
		string versionApplication = Application.version;

		if (versionWeb != null)
		{
            // Convert to floats
			float versionWebFloat = 0f;
            float.TryParse(versionWeb, out versionWebFloat);

			float versionApplicationFloat = 0f;
			float.TryParse(versionApplication, out versionApplicationFloat);

            // Set Text Prompts
			if (versionApplicationFloat > 0 && versionWebFloat > 0)
			{
				bool isLatestVersion = versionApplicationFloat < versionWebFloat ? false : true;
				if (isLatestVersion)
				{
					tooltip.NewContentAppend($"Current version: <b><style=Keyword>v{versionApplicationFloat}</b></style>\nUp to date.");
				}
				else // oudated
				{
					tooltip.NewContentAppend($"Current version: <b><style=Keyword>v{versionApplicationFloat}</b></style>\nLatest version: <b><style=Keyword>v{versionWebFloat}</b></style>\nDownload latest at\n<b><style=Keyword>kf-pixel.itch.io/lor-card");
					outline.enabled = true;
					textfield.color = outline.effectColor;
				}
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
