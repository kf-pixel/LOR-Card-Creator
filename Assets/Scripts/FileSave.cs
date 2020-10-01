using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.IO;
using System.Runtime.InteropServices;
using SFB;

public class FileSave : MonoBehaviour
{
	[SerializeField] private Canvas canvas;
	[SerializeField] private CanvasScaler canvasScaler;
	[SerializeField] private Vector2 resolution;
	[SerializeField] private Texture2D tex;
	[SerializeField] private Texture2D loadedTex2d;
	[SerializeField] private Texture2D placeholderTexture;
	[SerializeField] private RectTransform source;
	[SerializeField] private StringVariable titleName;
	[SerializeField] private IntVariable cardType;

	[Header("Tooltip Saving")]
	[SerializeField] private Texture2D texTooltip;
	[SerializeField] private GameObject tooltipCanvas;
	[SerializeField] private RectTransform tooltipSource;
	[SerializeField] private CustomKeywordData[] KWData;
	[SerializeField] private IntVariable KWTabIndex;

	[Header("Events")]
	[SerializeField] private UnityEvent setupSaveEvent;
	[SerializeField] private UnityEvent finishedSaveEvent;
	[SerializeField] private UnityEvent clipboardEvent;

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

#if UNITY_STANDALONE
	private void Start()
	{
		// Add placeholder image if one doesn't exist
		if (!Directory.Exists(Application.persistentDataPath))
		{
			Directory.CreateDirectory(Application.persistentDataPath);
		}
		if (!File.Exists(Application.persistentDataPath + "/placeholder.png") && placeholderTexture != null)
		{
			byte[] bits = placeholderTexture.EncodeToPNG();
			File.WriteAllBytes(Application.persistentDataPath + "/placeholder.png", bits);
			print("placeheld!");
		}
	}
#endif

	private void MemoryStreamToClipboard(Texture2D texture)
	{
		System.IO.Stream s = new System.IO.MemoryStream(texture.width * texture.height);
		byte[] bits = texture.EncodeToJPG(100);
		s.Write(bits, 0, bits.Length);

#if UNITY_STANDALONE
		System.Drawing.Image image = System.Drawing.Image.FromStream(s);
		System.Windows.Forms.Clipboard.SetImage(image);
		clipboardEvent.Invoke();
#endif
		s.Close();
		s.Dispose();
	}

	private void FileCopyToClipboard(Texture2D texture)
	{
		byte[] bits = texture.EncodeToPNG();
		if (!Directory.Exists(Application.persistentDataPath))
		{
			Directory.CreateDirectory(Application.persistentDataPath);
		}
		string path = Application.persistentDataPath + "/clipboard.png";
		File.WriteAllBytes(path, bits);

#if UNITY_STANDALONE
		System.Drawing.Image drawingImag = System.Drawing.Image.FromFile(path);
		System.Windows.Forms.Clipboard.SetImage(drawingImag);
		clipboardEvent.Invoke();
#endif

	}

	private IEnumerator EncodePNG(bool asClipBoard = false)
	{
		setupSaveEvent.Invoke();

#if UNITY_STANDALONE
		int currentResolutionWidth = Screen.width;
		int currentResolutionHeight = Screen.height;
		bool resolutionChanged = false;

		// Set Resolution to 1920x1080 if it's below it
		if (Screen.resolutions.Length > 0)
		{
			// Check if the monitor supports it & is currently below it
			if (Screen.resolutions[Screen.resolutions.Length - 1].width > 1900 && Screen.resolutions[Screen.resolutions.Length - 1].height > 1050 && (Screen.width < 1900 || Screen.height < 1050))
			{
				Screen.SetResolution(1920, 1080, Screen.fullScreen);
				resolutionChanged = true;
				yield return new WaitForEndOfFrame();
			}
		}
#endif
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

		// Add to Clipboard and return if stated
		if (asClipBoard == true)
		{
			tex.Apply();
			finishedSaveEvent.Invoke();
			MemoryStreamToClipboard(tex);

#if UNITY_STANDALONE
			// Return resolution
			if (resolutionChanged == true)
			{
				yield return new WaitForEndOfFrame();
				Screen.SetResolution(currentResolutionWidth, currentResolutionHeight, Screen.fullScreen);
			}
#endif
			yield break;
		}

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
					if (tex.GetPixel(x, y).a > 0.3f)
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

#if UNITY_STANDALONE
		// Return resolution
		if (resolutionChanged == true)
		{
			yield return new WaitForEndOfFrame();
			Screen.SetResolution(currentResolutionWidth, currentResolutionHeight, Screen.fullScreen);
		}
#endif

		// Platform dependent file saving
#if UNITY_EDITOR
		SaveDialog(fileName, bytes);
		yield break;
#endif

#if UNITY_STANDALONE
		SaveDialog(fileName, bytes);
		yield break;
#endif

#if UNITY_WEBGL
		DownloadScreenshot(bytes, fileName + ".png");
		yield break;
#endif
	}

	public void SaveDialog(string fileName, byte[] bytes)
	{
		var extensionList = new[] {
			new ExtensionFilter("PNG", "png"),
		};

		// Open Save as Panel
		var path = StandaloneFileBrowser.SaveFilePanel("Save File", "", fileName, extensionList);

		// Save
		if (path.Length > 1)
		{
			File.WriteAllBytes(path, bytes);
		}
	}

	public void SavePNG(bool asClipBoard = false)
	{
		StartCoroutine(EncodePNG(asClipBoard));
	}

	public void SaveTooltipPNG(bool asClipBoard = false)
	{
		StartCoroutine(EncodeTooltipPNG(asClipBoard));
	}

	private IEnumerator EncodeTooltipPNG(bool asClipboard = false)
	{

#if UNITY_STANDALONE
		int currentResolutionWidth = Screen.width;
		int currentResolutionHeight = Screen.height;
		bool resolutionChanged = false;

		// Set Resolution to 1920x1080 if it's below it
		if (Screen.resolutions.Length > 0)
		{
			// Check if the monitor supports it & is currently below it
			if (Screen.resolutions[Screen.resolutions.Length - 1].width > 1900 && Screen.resolutions[Screen.resolutions.Length - 1].height > 1050 && (Screen.width < 1900 || Screen.height < 1050))
			{
				Screen.SetResolution(1920, 1080, Screen.fullScreen);
				resolutionChanged = true;
				yield return new WaitForEndOfFrame();
			}
		}
#endif
		// We should only read the screen buffer after rendering is complete
		tooltipCanvas.SetActive(true);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

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

		// Copy into clipboard and return if stated
		if (asClipboard)
		{
			tooltipCanvas.SetActive(false);
			MemoryStreamToClipboard(texTooltip);

#if UNITY_STANDALONE
			// Return resolution
			if (resolutionChanged == true)
			{
				yield return new WaitForEndOfFrame();
				Screen.SetResolution(currentResolutionWidth, currentResolutionHeight, Screen.fullScreen);
			}
#endif
			yield break;
		}

		// Encode texture into PNG
		byte[] bytes = texTooltip.EncodeToPNG();

		// Get Active KW Data
		CustomKeywordData kw = KWData[KWTabIndex.value];

		// DETERMINE NAME
		string fileName = "tooltip_download";
		if (!string.IsNullOrEmpty(kw.label))
		{
			fileName = "tooltip_" + kw.label;
		}

		// Turn canvas off again
		tooltipCanvas.SetActive(false);

		// Platform dependent file saving
#if UNITY_EDITOR
		SaveDialog(fileName, bytes);
		yield break;
#endif

#if UNITY_STANDALONE
		SaveDialog(fileName, bytes);
		yield break;
#endif

#if UNITY_WEBGL
		DownloadScreenshot(bytes, fileName + ".png");
		yield break;
#endif
	}

}