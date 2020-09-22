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
	[SerializeField] private string url;
	[SerializeField] private Image img;
	[SerializeField] private Sprite sprite;
	[SerializeField] private float pixelsPerUnit = 100;

	// Get Image URL
	public async void LoadImageFromURL(string url)
	{
		Texture2D url_image = await GetURLImage(url);

		// Create Sprite
		if (sprite != null) Destroy(sprite);
		sprite = Sprite.Create(url_image, new Rect(0.0f, 0.0f, url_image.width, url_image.height), new Vector2(url_image.width / 2, url_image.height / 2), pixelsPerUnit, 1, SpriteMeshType.FullRect);
		sprite.name = "URL_IMAGE";
		img.sprite = sprite;
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
				Debug.Log($"{ www.error }, URL:{ www.url }");

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
