using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using SFB;

public class FileSave : MonoBehaviour
{
	[SerializeField] private Canvas canvas;
	[SerializeField] private CanvasScaler canvasScaler;
	[SerializeField] private Vector2 resolution;
	[SerializeField] private string disallowedFileNameChars;
	[SerializeField] private Texture2D tex;
	[SerializeField] private Texture2D loadedTex2d;
	[SerializeField] private Texture2D placeholderTexture;
	[SerializeField] private RectTransform source;
	[SerializeField] private StringVariable titleName;
	[SerializeField] private IntVariable cardType;
	[SerializeField] private ListManager setManager;

	[Header("Tooltip Saving")]
	[SerializeField] private Texture2D texTooltip;
	[SerializeField] private GameObject tooltipCanvas;
	[SerializeField] private RectTransform tooltipSource;
	[SerializeField] private CustomKeywordData[] KWData;
	[SerializeField] private IntVariable KWTabIndex;
	[SerializeField] private BoolVariable tooltipTransparent;

	[Header("Events")]
	[SerializeField] private UnityEvent setupSaveEvent;
	[SerializeField] private UnityEvent finishedSaveEvent;
	[SerializeField] private UnityEvent clipboardEvent;
	[SerializeField] private UnityEvent tooltipSetupEvent;
	[SerializeField] private UnityEvent tooltipFinishEvent;
	[SerializeField] private UnityEvent exportSetSetupEvent, exportSetFinisheEvent;

	private int currentResolutionWidth = Screen.width;
	private int currentResolutionHeight = Screen.height;

	private bool artLoaded = true;

	[DllImport("__Internal")]
	private static extern void ImageDownloader(string str, string fn);

	private void DownloadScreenshot(byte[] image_data, string filename)
	{
		if (image_data != null)
		{
			//Debug.Log("Downloading..." + filename);
			ImageDownloader(System.Convert.ToBase64String(image_data), filename);
		}
	}

	public void CardArtLoadedFlag() // via game event
	{
		artLoaded = true;
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

	private bool ResolutionSetup()
	{
		bool resolutionChanged = false;
#if UNITY_STANDALONE
		currentResolutionWidth = Screen.width;
		currentResolutionHeight = Screen.height;

		// Set Resolution to 1920x1080 if it's below it
		if (Screen.resolutions.Length > 0)
		{
			// Check if the monitor supports it & is currently below it
			if (Screen.resolutions[Screen.resolutions.Length - 1].width > 1900 && Screen.resolutions[Screen.resolutions.Length - 1].height > 1050 && (Screen.width < 1900 || Screen.height < 1050))
			{
				Screen.SetResolution(1920, 1080, Screen.fullScreen);
				resolutionChanged = true;
			}
		}
#endif
		return resolutionChanged;
	}

	private void ResolutionReturn()
	{
		Screen.SetResolution(currentResolutionWidth, currentResolutionHeight, Screen.fullScreen);
	}

	private void EncodeSetup(bool fixTransparency)
	{
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

		if (fixTransparency)
		{
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
		}

		// Apply to texture
		tex.Apply();
	}

	public void EncodeMultiple(List<ListItem> listItemSet)
	{
		StartCoroutine(EncodeMultiplePNG(listItemSet));
	}
	private IEnumerator EncodeMultiplePNG(List<ListItem> set)
	{
		// make a copy of the set
		List<ListItem> setCopy = new List<ListItem>(set);

		// Get the folder path via dialog
		string exportPath = "";
#if UNITY_STANDALONE
		exportPath = GetExportSetPath() + "/";
		if (exportPath == null)
		{
			Debug.Log("Cancelled");
			yield break;
		}
		if (exportPath.Length < 5)
		{
			Debug.Log("Cancelled, too short?");
			yield break;
		}
#endif

		exportSetSetupEvent.Invoke();

		setupSaveEvent.Invoke();

		bool resChanged = ResolutionSetup();

		// Loop through each card, wait for the art to load, then encode
		for (int i = setCopy.Count - 1; i >= 0 ; i--)
		{
			artLoaded = false;

			// Change the card data
			setManager.SetNewActiveItem(setCopy[i], false);

			// Wait for art to load via game event
			yield return new WaitUntil(() => artLoaded == true);
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();

			// Reads the screen pixels and adds to texture
			EncodeSetup(true);

			// Encode texture into PNG
			byte[] bytes = tex.EncodeToPNG();

			// DETERMINE NAME
			string fileName = (setCopy.Count - i) + " " + setCopy[i].cardData.cardName;
			fileName = FixFileName(fileName);

#if UNITY_WEBGL
			DownloadScreenshot(bytes, fileName + ".png");
			continue;
#endif
			// Write file
			File.WriteAllBytes(exportPath + fileName + ".png", bytes);
		}

		// Return Res
		if (resChanged)
		{
			yield return new WaitForEndOfFrame();
			ResolutionReturn();
		}

		exportSetFinisheEvent.Invoke();
		finishedSaveEvent.Invoke();
	}

	private IEnumerator EncodePNG(bool asClipBoard = false)
	{
		setupSaveEvent.Invoke();

		bool resChanged = ResolutionSetup();

		// We should only read the screen buffer after rendering is complete
		yield return new WaitForEndOfFrame();

		// Reads the screen pixels and adds to texture
		EncodeSetup(!asClipBoard);

		// Return Res
		if (resChanged)
		{
			yield return new WaitForEndOfFrame();
			ResolutionReturn();
		}

		// Add to Clipboard and return if stated
		if (asClipBoard == true)
		{
			finishedSaveEvent.Invoke();
			MemoryStreamToClipboard(tex);
			yield break;
		}

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

		fileName = FixFileName(fileName);

		// Platform dependent file writing
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

	public string GetExportSetPath(string fileName = "Set")
	{
		// Open Save as Panel
		var path = StandaloneFileBrowser.OpenFolderPanel("Set Folder for Export", Application.persistentDataPath, false);

		// Save
		if (path.Length > 0)
		{
			return path[0];
		}
		else return null;
	}

	public void SaveDialog(string fileName, byte[] bytes)
	{
		var extensionList = new[] {
			new ExtensionFilter("PNG", "png"),
		};

		// Discard illegal characters in fileName
		fileName = Regex.Replace(fileName, "[^A-za-z0-9 ]", "");

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
		// Set up res change
		bool resChanged = ResolutionSetup();

		// Set-up scene, turn off unneeded canvases
		tooltipCanvas.SetActive(true);
		tooltipSetupEvent.Invoke();

		// We should only read the screen buffer after rendering is complete
		yield return new WaitForEndOfFrame();

		// Destroy existing image
		if (texTooltip != null) Destroy(texTooltip);

		// Read Source Template
		float canvasLocalScale = canvas.transform.localScale.x;
		Vector2 sourceScreen = tooltipSource.position;
		Vector2 sourceSizeScreen = tooltipSource.rect.size * canvasLocalScale;

		// Create a texture the size of the screen, RGB24 format
		int width = (int)(sourceSizeScreen.x);
		int height = (int)(sourceSizeScreen.y);
		texTooltip = tooltipTransparent.value ?
			new Texture2D(width, height, TextureFormat.ARGB32, false, true) :
			new Texture2D(width, height, TextureFormat.ARGB32, false, true);

		// Read screen contents into the texture
		texTooltip.ReadPixels(new Rect(sourceScreen.x, sourceScreen.y, sourceSizeScreen.x, sourceSizeScreen.y), 0, 0);
		texTooltip.Apply();

		// fix tooltip transparency
		if (tooltipTransparent.value == false)
		{
			for (int y = 15; y < texTooltip.height - 15; y++)
			{
				for (int x = 15; x < texTooltip.width - 15; x++)
				{
					Color color = new Color(texTooltip.GetPixel(x, y).r, texTooltip.GetPixel(x, y).g, texTooltip.GetPixel(x, y).b, 1);
					texTooltip.SetPixel(x, y, color);
				}
			}
			texTooltip.Apply();
		}

		// Return Res
		if (resChanged)
		{
			yield return new WaitForEndOfFrame();
			ResolutionReturn();
		}

		// Reset Canvases
		tooltipCanvas.SetActive(false);
		tooltipFinishEvent.Invoke();

		// Copy into clipboard and return if stated
		if (asClipboard)
		{
			MemoryStreamToClipboard(texTooltip);
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

		fileName = FixFileName(fileName);

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

	private string FixFileName(string file)
	{
		string fixedFileName = file;
		char[] disallowd = disallowedFileNameChars.ToCharArray();
		foreach (char c in disallowd)
		{
			fixedFileName = fixedFileName.Replace(c.ToString(), "");
		}
		return fixedFileName;
	}
}