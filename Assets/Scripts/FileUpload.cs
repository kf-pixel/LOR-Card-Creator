using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Runtime.InteropServices;
using System.IO;
using SFB;
using System.Collections.Generic;

public class FileUpload : MonoBehaviour
{
	[DllImport("__Internal")]
	private static extern void ImageUploaderCaptureClick();

	[SerializeField] private Sprite sprite;
	[SerializeField] private float pixelsPerUnit = 100;

	[Header("Events")]
	[SerializeField] private UnityEvent uploadedEvent;
	[SerializeField] private UnityEvent lowResEvent;
	[SerializeField] private UnityEvent highResEvent;

	[Header("Set Image Locations")]
	[SerializeField] private Image img;
	[SerializeField] private Image[] tileImgs;
	[SerializeField] private Image spellImg;
	[SerializeField] private Image spellTextImage;
	[SerializeField] private StringVariable artworkPath;

#if UNITY_STANDALONE_WIN
	private void OnEnable()
	{
		B83.Win32.UnityDragAndDropHook.InstallHook();
		B83.Win32.UnityDragAndDropHook.OnDroppedFiles += OnFiles;
	}

	private void OnDisable()
	{
		B83.Win32.UnityDragAndDropHook.UninstallHook();
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
		if (!Directory.Exists(Application.persistentDataPath + "/artwork"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/artwork");
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
				// Reset image transform
				img.rectTransform.localPosition = Vector3.zero;
				spellImg.rectTransform.localPosition = Vector3.zero;
				spellTextImage.rectTransform.localPosition = Vector3.zero;

				FileSelected("file:///" + paths[0]);
			}
		}
	}

	public void FileSelected(string path)
	{
		// Check if the requested filed has the persistentdatapath tag
		if (path.Contains("&local&"))
		{
			path = path.Replace("&local&", Application.persistentDataPath);
		}

		//print("beginning upload...");

		StartCoroutine(LoadTexture(path));
		uploadedEvent.Invoke();
	}

	IEnumerator LoadTexture(string url)
	{
		WWW image = new WWW(url);
		yield return image;
		Texture2D texture = new Texture2D(1, 1);
		image.LoadImageIntoTexture(texture);

		texture.filterMode = FilterMode.Bilinear;

		// Create Sprite
		if (sprite != null) Destroy(sprite);
		sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2), pixelsPerUnit, 1, SpriteMeshType.FullRect);
		sprite.name = "URL_IMAGE";

		// Assign sprite
		img.sprite = sprite;
		spellImg.sprite = sprite;
		spellTextImage.sprite = sprite;

		foreach(Image im in tileImgs)
		{
			im.sprite = sprite;
		}

		// Set Image width x height
		float yAspectRatio = (float)texture.height / (float)texture.width;

		// Set size of image equal to aspect ratio
		img.rectTransform.sizeDelta = new Vector2(img.rectTransform.sizeDelta.x, img.rectTransform.sizeDelta.x * yAspectRatio);
		spellImg.rectTransform.sizeDelta = new Vector2(spellImg.rectTransform.sizeDelta.x, spellImg.rectTransform.sizeDelta.x * yAspectRatio);

		// Save artwork path
		artworkPath.value = url.Replace(Application.persistentDataPath.Replace("/", "\\"), "&local&");

		// Send low or high res events
		if (texture.height < 200 || texture.width < 200)
		{
			lowResEvent.Invoke();
		}
		else
		{
			highResEvent.Invoke();
		}

		//print("complete!");
	}

}