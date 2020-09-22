using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Keiwando.NFSO;
using System.IO;
using System.Runtime.InteropServices;

public class FileOpenSave : MonoBehaviour
{
	[SerializeField] private bool webGLBuild = false;
	[SerializeField] private Canvas canvas;
	[SerializeField] private CanvasScaler canvasScaler;
	[SerializeField] private Vector2 resolution;
	[SerializeField] private Texture2D tex;
	[SerializeField] private Texture2D loadedTex2d;
	[SerializeField] private RectTransform source;
	[SerializeField] private StringVariable titleName;
	[SerializeField] private IntVariable cardType;
	[SerializeField] private StringVariable keywordName;

	[Header("Tooltip Saving")]
	[SerializeField] private Texture2D texTooltip;
	[SerializeField] private GameObject tooltipCanvas;
	[SerializeField] private RectTransform tooltipSource;

	[Header("Events")]
	[SerializeField] private UnityEvent setupSaveEvent;
	[SerializeField] private UnityEvent finishedSaveEvent;

	[DllImport("__Internal")]
	private static extern void ImageDownloader(string str, string fn);

	private void DownloadScreenshot(byte[] image_data, string filename)
	{
		if (image_data != null)
		{
			Debug.Log("Downloading..." + filename);
			ImageDownloader(System.Convert.ToBase64String(image_data), filename);
		}
	}

	void Start()
	{
		NativeFileSOMobile.shared.FilesWereOpened += delegate (OpenedFile[] files)
		{
			// Process the opened files
		};
	}

	private IEnumerator EncodePNG()
	{
		setupSaveEvent.Invoke();

		// We should only read the screen buffer after rendering is complete
		yield return new WaitForEndOfFrame();

		// Destroy existing image
		if (tex != null) Destroy(tex);

		// Read Source Template
		float canvasLocalScale = canvas.transform.localScale.x;
		Vector2 sourceScreen = source.position;
		Vector2 sourceSizeScreen = source.rect.size * canvasLocalScale;

		// Create a texture the size of the screen, RGB24 format
		int width = (int)(sourceSizeScreen.x);
		int height = (int)(sourceSizeScreen.y);
		tex = new Texture2D(width, height, TextureFormat.ARGB32, false, true);

		// Read screen contents into the texture
		tex.ReadPixels(new Rect(sourceScreen.x, sourceScreen.y, sourceSizeScreen.x, sourceSizeScreen.y), 0, 0);

		// Fix transparency
		if (cardType.value < 3)
		{
			for (int y = 0; y < tex.height; y++)
			{
				for (int x = 0; x < tex.width; x++)
				{
					if (y > (int)(30f * canvasLocalScale) && y < tex.height - (int)(23f * canvasLocalScale) && x > (int)(15f * canvasLocalScale) && x < tex.width - (int)(16f * canvasLocalScale))
					{
						Color color = new Color(tex.GetPixel(x, y).r, tex.GetPixel(x, y).g, tex.GetPixel(x, y).b, 1);
						tex.SetPixel(x, y, color);

					}
				}
			}
		}
		else
		{
			for (int y = 0; y < tex.height; y++)
			{
				for (int x = 0; x < tex.width; x++)
				{
					if (tex.GetPixel(x,y).a > 0.3f)
					{
						Color color = new Color(tex.GetPixel(x, y).r, tex.GetPixel(x, y).g, tex.GetPixel(x, y).b, 1);
						tex.SetPixel(x, y, color);
					}
				}
			}
		}

		tex.Apply();

		finishedSaveEvent.Invoke();

		// Encode texture into PNG
		byte[] bytes = tex.EncodeToPNG();

		// DETERMINE NAME
		string fileName = "card_download";
		if (titleName.value != null)
		{
			if (titleName.value.Length > 0)
			{
				fileName = titleName.value;
				if (cardType.value == 2)
				{
					fileName = titleName.value + " levelUp";
				}
			}
		}

		// Platform dependent file saving
#if UNITY_EDITOR || UNITY_STANDALONE
		File.WriteAllBytes(Application.dataPath + "/../" + fileName + ".png", bytes);
#else
		DownloadScreenshot(bytes, fileName + ".png");
#endif
	}

	public void SavePNG()
	{
		StartCoroutine(EncodePNG());

		// Non webGL Save
		if (!webGLBuild)
		{
			StartCoroutine(SaveNewPNG());
		}
	}

	public void SaveTooltipPNG()
	{
		StartCoroutine(EncodeTooltipPNG());
	}

	private IEnumerator EncodeTooltipPNG()
	{
		setupSaveEvent.Invoke();

		// We should only read the screen buffer after rendering is complete
		tooltipCanvas.SetActive(true);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		finishedSaveEvent.Invoke();

		// Destroy existing image
		if (texTooltip != null) Destroy(texTooltip);

		// Read Source Template
		float canvasLocalScale = canvas.transform.localScale.x;
		Vector2 sourceScreen = tooltipSource.position;
		Vector2 sourceSizeScreen = tooltipSource.rect.size * canvasLocalScale;

		// Create a texture the size of the screen, RGB24 format
		int width = (int)(sourceSizeScreen.x) - 8;
		int height = (int)(sourceSizeScreen.y) - 8;
		texTooltip = new Texture2D(width, height, TextureFormat.RGB24, false, true);

		// Read screen contents into the texture
		texTooltip.ReadPixels(new Rect(sourceScreen.x, sourceScreen.y, sourceSizeScreen.x, sourceSizeScreen.y), 0, 0);

		texTooltip.Apply();

		// Encode texture into PNG
		byte[] bytes = texTooltip.EncodeToPNG();

		// DETERMINE NAME
		string fileName = "tooltip_download";
		if (keywordName.value != null)
		{
			if (keywordName.value.Length > 1)
			{
				fileName = "tooltip_" + keywordName.value; 
			}
		}

		yield return new WaitForEndOfFrame();

		// Turn canvas off again
		tooltipCanvas.SetActive(false);

		// Platform dependent file saving
#if UNITY_EDITOR || UNITY_STANDALONE
		File.WriteAllBytes(Application.dataPath + "/../" + fileName + ".png", bytes);
#else
		DownloadScreenshot(bytes, fileName + ".png");
#endif
	}

	private IEnumerator SaveNewPNG()
	{
		//yield return new WaitUntil(() => Directory.Exists(Application.dataPath + "/../" + titleName.value + ".png") == true);

		yield return new WaitForSeconds(1.25f);

		string path = Application.dataPath + "/../" + titleName.value + ".png";

		FileToSave file = new FileToSave(path, titleName.value, SupportedFileType.PNG);

		// Allows the user to choose a save location and saves the 
		// file to that location
		NativeFileSO.shared.SaveFile(file);
	}
}