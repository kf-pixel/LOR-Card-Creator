using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Runtime.InteropServices;
using System.IO;
using SFB;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public class FileUpload : MonoBehaviour
{
	[DllImport("__Internal")]
	private static extern void ImageUploaderCaptureClick();

	private Texture2D texture;

	[SerializeField] Texture2D webGLPlaceholderTexure;
	private List<Texture2D> texturesInMemory = new List<Texture2D>();
	[SerializeField] private int maxTexturesInMemory = 10;

	[Header("Events")]
	[SerializeField] private UnityEvent uploadedEvent;
	[SerializeField] private UnityEvent lowResEvent;
	[SerializeField] private UnityEvent highResEvent;
	[SerializeField] private UnityEvent cardArtLoadedEvent;
	[SerializeField] private UnityEvent onLeagueImageEvent, onWebImageEvent;
	[SerializeField] private UnityEvent webImageLoading, webImageComplete, webImageFailed, invalidWebURL;

	[Header("Artwork RawImages")]
	[SerializeField] private RawImage[] artworkImages;
	[SerializeField] private RawImage[] artworkTiledImages;
	[SerializeField] private StringVariable artworkPath;

	[Header("Anime4k Upscale")]
	[SerializeField] private bool useUpscale = true;
	[SerializeField] private int upscaleSize = 300;
	[SerializeField] private int upscaleCycles = 8;
	[SerializeField] private RenderTexture _output = null;

	// web img loading
	private string artworkFilePath;
	private Texture2D webImage;
	[Header("Web Image Load")]
	[SerializeField] private string urlPattern;

#if UNITY_STANDALONE_WIN
	private void OnEnable()
	{
		B83.Win32.UnityDragAndDropHook.InstallHook();
		B83.Win32.UnityDragAndDropHook.OnDroppedFiles += OnFiles;
	}

	private void OnDisable()
	{
		B83.Win32.UnityDragAndDropHook.UninstallHook();
		ClearTexturesFromMemory();
	}

	void OnFiles(List<string> aFiles, B83.Win32.POINT aPos)
	{
		string filePath = "";
		// scan through dropped files and filter out supported image types
		foreach (var f in aFiles)
		{
			var fi = new System.IO.FileInfo(f);
			var ext = fi.Extension.ToLower();
			if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
			{
				filePath = f;
				break;
			}
		}

		FileSelected(filePath);
	}
#endif

	public void OnButtonPointerDown()
	{

#if UNITY_EDITOR
		FileOpenDialog();
		return;
#endif

#if UNITY_STANDALONE
		FileOpenDialog();
		return;
#endif

#if UNITY_WEBGL
		ImageUploaderCaptureClick();
		return;
#endif

	}

	private void Start()
	{
		// Create artworks folder on standalone client
#if UNITY_STANDALONE
		artworkFilePath = Application.persistentDataPath + "/artwork";
		if (!Directory.Exists(artworkFilePath))
		{
			Directory.CreateDirectory(artworkFilePath);
		}
#endif
	}

	public void OpenApplicationFolder()
	{
		Application.OpenURL(Application.persistentDataPath);
	}

	// Standalone file dialog
	public void FileOpenDialog()
	{
		var extensions = new[]
		{
			new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
			new ExtensionFilter("All Files", "*" ),
		};

		string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

		if (paths.Length > 0)
		{
			if (paths[0].Length > 1)
			{
				foreach (RawImage ri in artworkImages) ri.rectTransform.localPosition = Vector3.zero;
				FileSelected("file:///" + paths[0]);
			}
		}
	}

	public void FileSelected(string path)
	{
		// Check if we are on WebGL
#if UNITY_WEBGL
		if (path.Contains("&local&/placeholder.png"))
		{
			texture = webGLPlaceholderTexure;
			AssignTextures();
			cardArtLoadedEvent.Invoke();
			return;
		}
#endif
		// Check if the requested filed has the persistentdatapath tag
		path = path.Replace("&local&", Application.persistentDataPath);

		// Check if texture is in memory
		foreach(Texture2D tex2d in texturesInMemory)
		{
			if (tex2d.name == path)
			{
				LoadTextureFromMemory(tex2d);
				return;
			}
		}

		// Load via coroutine
		StartCoroutine(LoadNewTexture(path));
	}

	private void LoadTextureFromMemory(Texture2D tex)
	{
		texture = tex;

		// Reset low res warning
		highResEvent.Invoke();

		// Save artwork path
		artworkPath.value = tex.name.Replace(Application.persistentDataPath.Replace("/", "\\"), "&local&");

		AssignTextures();

		// Re-enqueue used texture
		texturesInMemory.Remove(tex);
		texturesInMemory.Add(tex);

		// Send the upload complete event
		cardArtLoadedEvent.Invoke();
		uploadedEvent.Invoke();
	}

	IEnumerator LoadNewTexture(string url)
	{
		WWW image = new WWW(url);
		yield return image;

		// Create Texture
		texture = new Texture2D(1, 1);
		image.LoadImageIntoTexture(texture);

		// Texture settings
		texture.filterMode = FilterMode.Trilinear;

		// Save artwork path
		artworkPath.value = url.Replace(Application.persistentDataPath.Replace("/", "\\"), "&local&");

		// Check texture res
		if (texture.height < 256 || texture.width < 256)
		{
			lowResEvent.Invoke();
		}
		else
		{
			highResEvent.Invoke();
		}

		// Anime4k Upscale
#if UNITY_STANDALONE
		if (texture.width * texture.height < (256 * 256) && useUpscale)
		{
			// Determine scale factor
			float textureAspectRatio = (float)texture.width / (float)texture.height;

			// Create Render Texture
			if (_output != null) Destroy(_output);

			_output = textureAspectRatio > 1 ?
				new RenderTexture(Mathf.RoundToInt(upscaleSize * textureAspectRatio), upscaleSize, 16, RenderTextureFormat.ARGB32) :
				new RenderTexture(upscaleSize, Mathf.RoundToInt(upscaleSize / textureAspectRatio), 16, RenderTextureFormat.ARGB32);


			yield return new WaitForEndOfFrame();
			Anime4K.ImageFilter.Upscale(texture, _output, 0.5f);

			yield return new WaitForEndOfFrame();
			texture = FromRenderTexture(_output);

			// Repeat upscaling
			for (int i = 0; i < upscaleCycles; i++)
			{
				yield return new WaitForEndOfFrame();
				_output = new RenderTexture(texture.width, texture.height, 16, RenderTextureFormat.ARGB32);
				Anime4K.ImageFilter.Upscale(texture, _output, 0.5f);
				texture = FromRenderTexture(_output);

				AssignTextures();
			}
		}
#endif
		texture.name = url;
		AssignTextures();

		// Add texture to memory
		if (!texturesInMemory.Contains(texture))
		{
			texturesInMemory.Add(texture);

			// Remove textures from memory
			if (texturesInMemory.Count > maxTexturesInMemory)
			{
				Texture2D texToRemove = texturesInMemory[0];
				texturesInMemory.RemoveAt(0);
				Destroy(texToRemove);
			}
		}

		// Send the upload complete event
		cardArtLoadedEvent.Invoke();
		uploadedEvent.Invoke();
	}

	private void AssignTextures()
	{
		// Assign Textures
		foreach (RawImage ri in artworkImages) ri.texture = texture;
		foreach (RawImage rit in artworkTiledImages) rit.texture = texture;

		// Set Image width x height
		float yAspectRatio = (float)texture.height / (float)texture.width;

		// Set size of image equal to aspect ratio
		foreach (RawImage ri in artworkImages)
			ri.rectTransform.sizeDelta = new Vector2(ri.rectTransform.sizeDelta.x, ri.rectTransform.sizeDelta.x * yAspectRatio);
	}

	public void Anime4kUpscale()
	{
		StartCoroutine(Anime4kUpscaleCoroutine());
	}

	private IEnumerator Anime4kUpscaleCoroutine()
	{
		// Create Render Texture
		if (_output != null) Destroy(_output);
		_output = new RenderTexture(texture.width, texture.height, 16, RenderTextureFormat.ARGB32);
		yield return new WaitForEndOfFrame();
		Anime4K.ImageFilter.Upscale(texture, _output, 0.5f);
		yield return new WaitForEndOfFrame();
		texture = FromRenderTexture(_output);

		AssignTextures();
	}

	private Texture2D FromRenderTexture(RenderTexture rTex)
	{
		Texture2D dest = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBA32, false);
		dest.Apply(false);
		Graphics.CopyTexture(rTex, dest);
		return dest;
	}

	public void ClearTexturesFromMemory()
	{
		for (int i = texturesInMemory.Count - 1; i >= 0 ; i--)
		{
			Texture2D texToRemove = texturesInMemory[texturesInMemory.Count - 1];
			texturesInMemory.Remove(texToRemove);
			Destroy(texToRemove);
		}
	}

	public void SetTextureMemoryCapacity(int count)
	{
		maxTexturesInMemory = count;
	}



	/// Web Image loading
	public bool CheckIfWebImageDownloaded(string fileName)
	{
		if (File.Exists(artworkFilePath + "/" + fileName))
		{
			return true;
		}
		else return false;
	}

	public async void GetWebImage(string url, string fileName) // league images
	{
		if (CheckIfWebImageDownloaded(fileName) == true)
		{
			FileSelected(artworkFilePath + "/" + fileName);
			onLeagueImageEvent.Invoke();
			return;
		}

		webImageLoading.Invoke();

		if (webImage != null) Destroy(webImage);
		webImage = await GetURLImage(url);
		if (webImage == null) return;

		// Download into artwork folder, return directory
		SaveTextureAsPNG(webImage, artworkFilePath, fileName);
		FileSelected(artworkFilePath + "/" + fileName);
		onLeagueImageEvent.Invoke();

		webImageComplete.Invoke();
	}

	public async void GetWebImage(string url) // web images
	{
		if (IsValidURL(url) == false)
		{
			invalidWebURL.Invoke();
			return;
		}

		string urlLastName = url.Substring(url.LastIndexOf("/") + 1);
		string fileName = Regex.Replace(urlLastName, "[\\W]+", "");

		if (CheckIfWebImageDownloaded(fileName + ".jpg") == true)
		{
			FileSelected(artworkFilePath + "/" + fileName + ".jpg");
			onWebImageEvent.Invoke();
			// successfully found art in folder
			return;
		}

		webImageLoading.Invoke();

		if (webImage != null) Destroy(webImage);
		webImage = await GetURLImage(url);
		if (webImage == null) return;

		// Download into artwork folder, return directory
		SaveTextureAsPNG(webImage, artworkFilePath, fileName);
		FileSelected(artworkFilePath + "/" + fileName + ".jpg");
		onWebImageEvent.Invoke();

		webImageComplete.Invoke();
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

				webImageFailed.Invoke();

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

	private bool IsValidURL(string url)
	{
		if (Regex.IsMatch(url, urlPattern))
		{
			if (url.Contains(".jpg") || url.Contains(".jpeg") || url.Contains(".png"))
			{
				return true;
			}
		}
		return false;
	}

	public void SaveTextureAsPNG(Texture2D _texture, string _fullPath, string _fileName)
	{
#if UNITY_WEBGL
		Debug.Log("Can't save web images on WEBGL! Returning");
		return;
#endif

		byte[] _bytes = _texture.EncodeToJPG(85);
		string extension = (_fileName.EndsWith(".jpg") || _fileName.EndsWith(".png") || _fileName.EndsWith(".jpeg"))  ? "" : ".jpg";
		System.IO.File.WriteAllBytes(_fullPath + "/" + _fileName + extension, _bytes);
	}
}