using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using System.Threading.Tasks;

public class LoadURL : MonoBehaviour
{
	[SerializeField] private Image img;
	[SerializeField] private Texture2D webImage;
	[SerializeField] private float pixelsPerUnit = 100;
	private string artworkPath = Application.persistentDataPath + "/artwork";

	public bool CheckIfWebImageDownloaded(string url)
	{

		return false;
	}
	public async void GetWebImage(string url)
	{
		if (webImage != null) Destroy(webImage);
		webImage = await GetURLImage(url);

		// Download into artwork folder, return directory
		
	}

	public async Task<Texture2D> GetURLImage(string url)
	{
		using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
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
				return DownloadHandlerTexture.GetContent(www);
			}
		}
	}
}
