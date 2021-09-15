using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class UserContentLoader : MonoBehaviour
{
	[Header("User Keyword Sprites")]
	[SerializeField] private TMP_SpriteAsset spriteAsset;
	[SerializeField] private Texture2D placeholderUserSprites;
	[SerializeField] private Texture2D stex;

	[Header("User Regions")]
	[SerializeField] private Sprite placeholderRegion;
	[SerializeField] private SpritesVariable regions;
	[SerializeField] private RegionToggle[] regionToggles;
	private int maxUserRegions = 7;

	private void Start()
    {
		//StartCoroutine(LoadUserSpritesIE());
		StartCoroutine(LoadUserRegions());
	}

	private IEnumerator LoadUserRegions()
	{
		// create placeholder
		if (!File.Exists(Application.persistentDataPath + "/user_region1.png"))
		{
			byte[] pfile = placeholderRegion.texture.EncodeToPNG();
			File.WriteAllBytes(Application.persistentDataPath + "/user_region1.png", pfile);
		}

		yield return new WaitForEndOfFrame();

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
		}
	}

	private IEnumerator LoadUserSpritesIE()
	{
		if (!File.Exists(Application.persistentDataPath + "/user_sprites.png"))
		{
			byte[] bits = placeholderUserSprites.EncodeToPNG();
			File.WriteAllBytes(Application.persistentDataPath + "/user_sprites.png", bits);
		}

		yield return new WaitForEndOfFrame();
		if (!File.Exists(Application.persistentDataPath + "/user_sprites.png")) yield break;

		WWW image = new WWW(Application.persistentDataPath + "/user_sprites.png");
		yield return image;

		// Create Texture
		stex = new Texture2D(1, 1);
		image.LoadImageIntoTexture(stex);

		// adjust texture settings
		stex.name = "loaded_sprites";
		stex.Apply();

		// Apply to sprite asset
		spriteAsset.spriteSheet = stex;
		spriteAsset.UpdateLookupTables();
	}

    private void OnDisable()
    {
		//spriteAsset.spriteSheet = placeholderUserSprites;
	}
}