using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using TMPro;
using SFB;
using System.Linq;

public class FileSave : MonoBehaviour
{
	[Header("Components")]
	[SerializeField] private Canvas rootCanvas;
	[SerializeField] private RectTransform source;
	[SerializeField] private GameObject tooltipCanvas;
	[SerializeField] private RectTransform tooltipSource;
	[SerializeField] private ListManager setManager;
	[SerializeField] private TMP_InputField creditField;

	[SerializeField] private GradientColourGrab gradientSpell;

	[Header("Data")]
	[SerializeField] private Texture2D placeholderTexture;
	[SerializeField] private StringVariable titleName;
	[SerializeField] private IntVariable cardType;
	[SerializeField] private IntVariable cardResolution;
	[SerializeField] private string disallowedFileNameChars;
	[SerializeField] private RenderTexture[] screenRTs;
	public Vector2Int[] compLayouts;
	private Texture2D tex;
	private Texture2D texTooltip;
	private Texture2D comp;
	private bool artLoaded = true;
	private List<Texture2D> compImages = new List<Texture2D>();

	// Resolution Data
	private RenderTexture currentScreenRT;
	private Vector2Int cardSize = new Vector2Int(340, 512);
	private Vector2Int tooltipSize = new Vector2Int(240, 512);
	private Vector2Int screenSize = new Vector2Int(1280, 720);
	private float sizeMultiplier = 1f;
	private bool limitlessLayoutSize = false;
	private Color bgColor = new Color(0.06666667f, 0.08235294f, 0.1098039f, 0f);

	[Header("Tooltip Data")]
	[SerializeField] private CustomKeywordData[] KWData;
	[SerializeField] private IntVariable KWTabIndex;
	[SerializeField] private BoolVariable tooltipTransparent;

	[Header("Events")]
	[SerializeField] private UnityEvent setupSaveEvent;
	[SerializeField] private UnityEvent finishedSaveEvent;
	[SerializeField] private UnityEvent clipboardEvent;
	[SerializeField] private UnityEvent tooltipSetupEvent;
	[SerializeField] private UnityEvent tooltipFinishEvent;
	[SerializeField] private UnityEvent multipleExportSetupEvent, multipleExportFinishedEvent;


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
		Destroy(texture);
		bits = null;
	}

	private void PlatformDependentDialog(string fileName, byte[] bytes)
	{
#if UNITY_EDITOR
		SaveDialog(fileName, bytes);
		return;
#endif

#if UNITY_STANDALONE
		SaveDialog(fileName, bytes);
		return;
#endif

#if UNITY_WEBGL
		DownloadScreenshot(bytes, fileName + ".png");
		return;
#endif
	}
	private void GetResolution()
	{
		if (cardResolution.value == 2)
		{
			sizeMultiplier = 2f;
			cardSize = new Vector2Int(680, 1024);
			tooltipSize = new Vector2Int(1360, 832);
			screenSize = new Vector2Int(2560, 1440);
		}
		else if (cardResolution.value == 1)
		{
			sizeMultiplier = 1.5f;
			cardSize = new Vector2Int(510, 768);
			tooltipSize = new Vector2Int(1024, 624);
			screenSize = new Vector2Int(1920, 1080);
		}
		else
		{
			sizeMultiplier = 1f;
			cardSize = new Vector2Int(340, 512);
			tooltipSize = new Vector2Int(680, 416);
			screenSize = new Vector2Int(1280, 720);
		}
	}

	private void RenderTextureSetup()
	{
		GetResolution();
		//if (screenRenderTexture != null) Destroy(screenRenderTexture);
		//screenRenderTexture = new RenderTexture(screenSize.x, screenSize.y, 24, UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat);
		currentScreenRT = screenRTs[cardResolution.value];
		Camera.main.targetTexture = currentScreenRT;
	}

	private Texture2D ScreenGrab()
	{
		Texture2D screenTexture = new Texture2D(currentScreenRT.width, currentScreenRT.height, TextureFormat.ARGB32, false);
		RenderTexture.active = currentScreenRT;
		screenTexture.ReadPixels(new Rect(0, 0, currentScreenRT.width, currentScreenRT.height), 0, 0);
		screenTexture.Apply();

		Destroy(screenTexture);
		Camera.main.targetTexture = null;
		RenderTexture.active = null;

		return screenTexture;
	}

	private void EncodeSetup(bool clipboard)
	{
		// Clear Memory
		Destroy(tex);

		// grab screen
		Texture2D screenRender = ScreenGrab();

		// get pixels from screen rendertexture
		tex = new Texture2D(cardSize.x, cardSize.y, TextureFormat.ARGB32, false, true);
		tex.SetPixels(0, 0, cardSize.x, cardSize.y, screenRender.GetPixels((int)(source.anchoredPosition.x * sizeMultiplier), (int)(source.anchoredPosition.y * sizeMultiplier), cardSize.x, cardSize.y));

		// fix transparency
		if (!clipboard)
		{
			for (int tx = 0; tx < tex.width; tx++)
			{
				for (int ty = 0; ty < tex.height; ty++)
				{
					if (tex.GetPixel(tx, ty).a > 0.1f)
					{
						tex.SetPixel(tx, ty, new Color(tex.GetPixel(tx, ty).r, tex.GetPixel(tx, ty).g, tex.GetPixel(tx, ty).b));
					}
				}
			}
		}
		tex.Apply();

		// Clean up
		Destroy(screenRender);

		// Reenable the credit field
		creditField.gameObject.SetActive(true);
	}

	private IEnumerator EncodePNG(bool clipboard = false)
	{
		setupSaveEvent.Invoke();
		RenderTextureSetup();

		// Disable the credit field if it has a dd URL
		ClearCreditField();

		// We should only read the screen buffer after rendering is complete
		yield return new WaitForEndOfFrame();

		// Reads the screen pixels and adds to texture
		EncodeSetup(clipboard);

		finishedSaveEvent.Invoke();

		// Add to Clipboard and return if stated
		if (clipboard == true)
		{
			MemoryStreamToClipboard(tex);
			Destroy(tex);
			yield break;
		}

		// Encode texture into PNG
		byte[] bytes = tex.EncodeToPNG();
		Destroy(tex);

		// DETERMINE NAME
		string fileName = "card_download";
		if (titleName.value != null)
		{
			if (titleName.value.Length > 0)
			{
				fileName = titleName.value;
				if (cardType.value == 2)
				{
					fileName = $"{titleName.value} LVL2";
				}
				else if (cardType.value == 8)
				{
					fileName = $"{titleName.value} LVL3";
				}
			}
		}
		fileName = GetUseableFileName(fileName);

		// Save to file
		PlatformDependentDialog(fileName, bytes);
	}

	private IEnumerator EncodeTooltipPNG(bool asClipboard = false)
	{
		// Clear Memory
		if (texTooltip != null) Destroy(texTooltip);

		// Set-up scene, turn off unneeded canvases
		tooltipCanvas.SetActive(true);
		tooltipSetupEvent.Invoke();
		RenderTextureSetup();

		// We should only read the screen buffer after rendering is complete
		yield return new WaitForEndOfFrame();

		// grab screen
		Texture2D screenRender = ScreenGrab();

		// get pixels from screen rendertexture
		texTooltip = new Texture2D(tooltipSize.x, tooltipSize.y, TextureFormat.ARGB32, false, true);
		texTooltip.SetPixels(0, 0, tooltipSize.x, tooltipSize.y, screenRender.GetPixels((int)(tooltipSource.anchoredPosition.x * sizeMultiplier), (int)(tooltipSource.anchoredPosition.y * sizeMultiplier), tooltipSize.x, tooltipSize.y));

		/* fix tooltip transparency
		if (tooltipTransparent.value == false && !asClipboard)
		{
			for (int y = 4; y < texTooltip.height - 4; y++)
			{
				for (int x = 3; x < texTooltip.width - 3; x++)
				{
					Color color = new Color(texTooltip.GetPixel(x, y).r, texTooltip.GetPixel(x, y).g, texTooltip.GetPixel(x, y).b, 1);
					texTooltip.SetPixel(x, y, color);
				}
			}
		}
		*/

		// fix transparency
		//if (!tooltipTransparent.value && !asClipboard)
		{
			for (int tx = 0; tx < texTooltip.width; tx++)
			{
				for (int ty = 0; ty < texTooltip.height; ty++)
				{
					if (texTooltip.GetPixel(tx, ty).a > 0.1f)
					{
						texTooltip.SetPixel(tx, ty, new Color(texTooltip.GetPixel(tx, ty).r, texTooltip.GetPixel(tx, ty).g, texTooltip.GetPixel(tx, ty).b));
					}
				}
			}
		}
		texTooltip.Apply();

		// Reset Canvases
		tooltipCanvas.SetActive(false);
		tooltipFinishEvent.Invoke();

		// Clean up
		Destroy(screenRender);

		// Copy and return if clipboard
		if (asClipboard)
		{
			MemoryStreamToClipboard(texTooltip);
			Destroy(texTooltip);
			yield break;
		}

		// Encode texture into PNG
		byte[] bytes = texTooltip.EncodeToPNG();
		Destroy(texTooltip);

		// DETERMINE NAME
		CustomKeywordData kw = KWData[KWTabIndex.value];
		string fileName = "tooltip_download";
		if (!string.IsNullOrEmpty(kw.label))
		{
			fileName = $"Keyword Tooltip {kw.label}";
		}
		fileName = GetUseableFileName(fileName);

		// Save to file
		PlatformDependentDialog(fileName, bytes);
	}


	public void EncodeMultiple()
	{
		StartCoroutine(EncodeMultipleIE(setManager.activeItem));
	}
	private IEnumerator EncodeMultipleIE(List<ListItem> set)
	{
		FrameRateManager.Instance.DisableFRManager();

		// make a copy
		List<ListItem> setCopy = new List<ListItem>(set);

		// Get the folder path via dialog
		string exportPath = "";

#if UNITY_STANDALONE
		exportPath = GetExportSetPath() + "/";
		if (exportPath == null)
		{
			//Debug.Log("Cancelled");
			yield break;
		}
		if (exportPath.Length < 5)
		{
			//Debug.Log("Cancelled, too short?");
			yield break;
		}
#endif

		multipleExportSetupEvent.Invoke();
		setupSaveEvent.Invoke();
		RenderTextureSetup();
		yield return new WaitForEndOfFrame();

		// Loop through each card, wait for the art to load, then encode
		for (int i = setCopy.Count - 1; i >= 0 ; i--)
		{
			artLoaded = false;

			// Change the card data
			setManager.SetNewActiveItem(setCopy[i], false);

			// Disable the credit field if it has a dd URL
			ClearCreditField();

			// Wait for art to load via game event
			yield return new WaitUntil(() => artLoaded == true);
			yield return new WaitForEndOfFrame();

			Camera.main.targetTexture = currentScreenRT;
			yield return new WaitForEndOfFrame();

			// Reads the screen pixels and adds to texture
			EncodeSetup(false);

			// Reenable the credit field
			creditField.gameObject.SetActive(true);

			// Encode texture into PNG
			byte[] bytes = tex.EncodeToPNG();
			Destroy(tex);

			// DETERMINE NAME
			string fileName = (setCopy.Count - i) + " " + setCopy[i].cardData.cardName;
			fileName = GetUseableFileName(fileName);

#if UNITY_WEBGL
			DownloadScreenshot(bytes, fileName + ".png");
			continue;
#endif
			// Write file
			File.WriteAllBytes(exportPath + fileName + ".png", bytes);
		}

		multipleExportFinishedEvent.Invoke();
		finishedSaveEvent.Invoke();
		FrameRateManager.Instance.EnableFRManager();
	}

	public void EncodeCompImages(bool clipboard)
	{
		StartCoroutine(EncodeCompImagesIE(setManager.activeItem, clipboard));
	}

	private IEnumerator EncodeCompImagesIE(List<ListItem> set, bool clipboard = false)
	{
		FrameRateManager.Instance.DisableFRManager();

		// make a copy
		List<ListItem> setCopy = set.OrderBy(x => x.listOrderIndex).ToList();

		multipleExportSetupEvent.Invoke();
		setupSaveEvent.Invoke();

		// Create a composite texture size according to the number of images
		RenderTextureSetup();
		Vector2Int gridLayout = EncodeCompLayout(setCopy.Count);

		// Fill texture with bg color
		Destroy(comp);
		comp = new Texture2D(cardSize.x * gridLayout.x, cardSize.y * gridLayout.y, TextureFormat.ARGB32, false);
		comp.name = "comp";
		for (int compx = 0; compx < comp.width; compx++)
		{
			for (int compy = 0; compy < comp.height; compy++)
			{
				comp.SetPixel(compx, compy, bgColor);
			}
		}

		// Calculate last row position offsets
		int emptyCardSlots = gridLayout.x * gridLayout.y - setCopy.Count;
		int x_offset = (int)emptyCardSlots * (cardSize.x / 2);

		// Loop through each card, wait for the art to load, then encode
		for (int i = 0; i < setCopy.Count && i < compLayouts.Length - 1; i++)
		{
			artLoaded = false;

			// Change the card data
			setManager.SetNewActiveItem(setCopy[i], false);

			// Disable the credit field if it has a dd URL
			ClearCreditField();

			// Wait for art to load via game event
			yield return new WaitUntil(() => artLoaded == true);
			yield return new WaitForEndOfFrame();

			Camera.main.targetTexture = currentScreenRT;
			yield return new WaitForEndOfFrame();

			// Reads the screen pixels and adds to texture
			EncodeSetup(false);

			// Reenable the credit field
			creditField.gameObject.SetActive(true);

			// Encode texture into PNG
			int rowInverse = gridLayout.y - 1 - (Mathf.FloorToInt(i / gridLayout.x));
			int column = i - ((Mathf.FloorToInt(i / gridLayout.x) * gridLayout.x));
			//Debug.Log($"{rowInverse}, {column}");

			// Set pixels onto comp, add x pos offset if last row
			if (rowInverse == 0)
			{
				comp.SetPixels(column * cardSize.x + x_offset, rowInverse * cardSize.y, cardSize.x, cardSize.y, tex.GetPixels(0, 0, cardSize.x, cardSize.y));
			}
			else
			{
				comp.SetPixels(column * cardSize.x, rowInverse * cardSize.y, cardSize.x, cardSize.y, tex.GetPixels(0, 0, cardSize.x, cardSize.y));
			}
			Destroy(tex);
		}

		multipleExportFinishedEvent.Invoke();
		finishedSaveEvent.Invoke();
		FrameRateManager.Instance.EnableFRManager();

		// clipboard route
		if (clipboard)
		{
			MemoryStreamToClipboard(comp);
			Destroy(comp);

			yield break;
		}

		// Encode texture into PNG
		byte[] bytes = comp.EncodeToPNG();
		Destroy(comp);

		// Save dialog
		string fileName = $"{setCopy[0].cardData.cardName} Comp";
		PlatformDependentDialog(fileName, bytes);
		bytes = null;
	}

	public Vector2Int EncodeCompLayout(int count)
	{
		if (count < compLayouts.Length)
		{
			return compLayouts[count];
		}

		if (limitlessLayoutSize == true)
		{
			float fourcount = count / 4f;
			float fourcountfloor = Mathf.Floor(fourcount);
			bool hangingfour = fourcount - fourcountfloor == 0.25f ? true : false;

			if ((float)count / 5 == Mathf.Floor(count / 5) || hangingfour == true) // 5 column
			{
				return new Vector2Int(5, Mathf.CeilToInt((float)count / 5));
			}
			else // 4 column
			{
				return new Vector2Int(4, Mathf.CeilToInt((float)count / 4));
			}
		}
		else return compLayouts[compLayouts.Length - 1];
	}


	public string GetExportSetPath()
	{
		// Open Save as Panel
		var path = StandaloneFileBrowser.OpenFolderPanel("Set Folder for Export", Application.persistentDataPath, false);
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

	private string GetUseableFileName(string file)
	{
		string fixedFileName = file;
		char[] disallowd = disallowedFileNameChars.ToCharArray();
		foreach (char c in disallowd)
		{
			fixedFileName = fixedFileName.Replace(c.ToString(), "");
		}
		return fixedFileName;
	}

	private void ClearCreditField()
	{
		if (creditField.text.StartsWith("http://dd.b.pvp.net"))
		{
			creditField.gameObject.SetActive(false);
		}
		else
		{
			creditField.gameObject.SetActive(true);
		}
	}
}