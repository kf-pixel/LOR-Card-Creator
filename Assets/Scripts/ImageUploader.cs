using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Runtime.InteropServices;

public class ImageUploader : MonoBehaviour
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

	public void OnButtonPointerDown()
	{
#if UNITY_EDITOR || UNITY_STANDALONE
		string path = UnityEditor.EditorUtility.OpenFilePanel("Open image", "", "jpg,png,bmp");
		if (!System.String.IsNullOrEmpty(path))
		{
			FileSelected("file:///" + path);
		}
#else
        ImageUploaderCaptureClick ();
#endif
	}

	public void FileSelected(string path)
	{
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
		img.rectTransform.sizeDelta = new Vector2(img.rectTransform.sizeDelta.x, img.rectTransform.sizeDelta.x * yAspectRatio);

		// Save artwork path
		artworkPath.value = url;

		Debug.Log("Loaded image size: " + texture.width + "x" + texture.height);

		// Send low or high res events
		if (texture.height < 200 || texture.width < 200)
		{
			lowResEvent.Invoke();
		}
		else
		{
			highResEvent.Invoke();
		}
	}

}