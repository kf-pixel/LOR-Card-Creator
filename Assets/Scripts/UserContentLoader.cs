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
	[SerializeField] private Texture2D loadedUserSprites;

	[Header("User Regions")]
	[SerializeField] private Sprite[] userRegions;
	private void Start()
    {
		if (!File.Exists(Application.persistentDataPath + "/user_sprites.png"))
		{
			byte[] bits = placeholderUserSprites.EncodeToPNG();
			File.WriteAllBytes(Application.persistentDataPath + "/user_sprites.png", bits);
		}

		StartCoroutine(LoadUserSpritesIE());
	}

	private IEnumerator LoadUserSpritesIE()
	{
		yield return new WaitForEndOfFrame();
		if (!File.Exists(Application.persistentDataPath + "/user_sprites.png")) yield break;

		WWW image = new WWW(Application.persistentDataPath + "/user_sprites.png");
		yield return image;

		// Create Texture
		loadedUserSprites = new Texture2D(1, 1);
		image.LoadImageIntoTexture(loadedUserSprites);

		// adjust texture settings
		loadedUserSprites.name = "loaded_sprites";
		loadedUserSprites.Apply();

		// Apply to sprite asset
		spriteAsset.spriteSheet = loadedUserSprites;
		spriteAsset.UpdateLookupTables();
	}

    private void OnDisable()
    {
		spriteAsset.spriteSheet = placeholderUserSprites;
	}
}