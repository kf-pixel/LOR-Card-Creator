using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class UserContentLoader : MonoBehaviour
{
	[Header("User Keyword Sprites")]
	[SerializeField] private TMP_SpriteAsset spriteAsset;
	[SerializeField] private Texture2D placeholderFileSprite;
	[SerializeField] private Texture2D placeholderUserSprites;
	[SerializeField] private Texture2D stex;
	[SerializeField] private TMP_Dropdown[] dropdowns;
	private int maxUserSprites = 10;

	[Header("User Regions")]
	[SerializeField] private Sprite placeholderRegion;
	[SerializeField] private SpritesVariable regions;
	[SerializeField] private RegionToggle[] regionToggles;
	private int maxUserRegions = 7;

	private void Start()
    {
		StartCoroutine(LoadUserSpritesIE());
		StartCoroutine(LoadUserRegions());
	}

	private IEnumerator LoadUserRegions()
	{
		yield return new WaitForSeconds(0.2f);

		// create placeholder
		if (!File.Exists(Application.persistentDataPath + "/user_region1.png"))
		{
			byte[] pfile = placeholderRegion.texture.EncodeToPNG();
			File.WriteAllBytes(Application.persistentDataPath + "/user_region1.png", pfile);
		}

		// loop through user region names
		for (int i = 0; i < maxUserRegions; i++)
		{
			if (File.Exists(Application.persistentDataPath + "/user_region" + (i + 1) + ".png"))
			{
				//Debug.Log("Region " + (i + 1) + " exists");
				WWW image = new WWW(Application.persistentDataPath + "/user_region" + (i + 1) + ".png");
				yield return image;

				// Create Texture
				Texture2D rtex = new Texture2D(1, 1);
				image.LoadImageIntoTexture(rtex);

				// adjust texture settings
				rtex.name = "user_region" + (i + 1);
				rtex.Apply();

				// link to scriptable object
				Sprite rsprite = Sprite.Create(rtex, new Rect(0f, 0f, rtex.width, rtex.height), new Vector2(0.5f, 0.5f), 100);
				regions.values[i + 14] = rsprite;

				// Update region toggle
				regionToggles[i].gameObject.SetActive(true);
				regionToggles[i].UpdateSprites(rsprite);
			}
			else
			{
				//Debug.Log("Region " + (i + 1) + " doesn't exist.");
				regions.values[i + 14] = placeholderRegion;
			}
			FrameRateManager.Instance.RequestFullFrameRate();
			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator LoadUserSpritesIE()
	{
		yield return new WaitForSeconds(0.4f);

		if (!File.Exists(Application.persistentDataPath + "/user_sprite1.png"))
		{
			byte[] pfile = placeholderFileSprite.EncodeToPNG();
			File.WriteAllBytes(Application.persistentDataPath + "/user_sprite1.png", pfile);
		}

		Texture2D blankTexture = new Texture2D(150, 150);
		List<Texture2D> userSprites = new List<Texture2D>();

		// loop through user sprite names
		for (int i = 0; i < maxUserSprites; i++)
		{
			if (File.Exists(Application.persistentDataPath + "/user_sprite" + (i + 1) + ".png"))
			{
				WWW image = new WWW(Application.persistentDataPath + "/user_sprite" + (i + 1) + ".png");
				yield return image;

				// Create Texture
				Texture2D spriteTex = new Texture2D(1, 1);
				image.LoadImageIntoTexture(spriteTex);

				// adjust texture settings
				if (spriteTex.width != 150 || spriteTex.height != 150) spriteTex = ScaleTexture(spriteTex, 150, 150);
				spriteTex.name = "user_region" + (i + 1);

				userSprites.Add(spriteTex);

				// Add to dropdowns
				TMP_Dropdown.OptionData newDropdownOption = new TMP_Dropdown.OptionData($"<sprite name=\"user{i + 1}\" tint><color=white>U{i + 1}", null);
				foreach (TMP_Dropdown drop in dropdowns)
				{
					drop.options.Add(newDropdownOption);
				}
			}
			else
			{
				userSprites.Add(blankTexture);
			}
			FrameRateManager.Instance.RequestFullFrameRate();
			yield return new WaitForEndOfFrame();
		}

		// Pack sprites together
		for (int i = 0; i < userSprites.Count; i++)
		{
			stex.SetPixels(i * 150, 0, 150, 150, userSprites[i].GetPixels());
		}
		stex.Apply();
		spriteAsset.UpdateLookupTables();

	
	}

    private void OnDisable()
    {
#if UNITY_EDITOR
		stex.Resize(placeholderUserSprites.width, placeholderUserSprites.height);
		stex.SetPixels(placeholderUserSprites.GetPixels());
		stex.Apply();
#endif
	}

	// jon martin
	private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
	{
		Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
		Color[] rpixels = result.GetPixels(0);
		float incX = (1.0f / (float)targetWidth);
		float incY = (1.0f / (float)targetHeight);
		for (int px = 0; px < rpixels.Length; px++)
		{
			rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
		}
		result.SetPixels(rpixels, 0);
		result.Apply();
		return result;
	}
}