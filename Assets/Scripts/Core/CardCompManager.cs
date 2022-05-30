using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.IO;

public class CardCompManager : MonoBehaviour
{
	[SerializeField] private Camera cam;
	[SerializeField] private Canvas canvas;
	[SerializeField] private RenderTexture renderTexture;

    [ContextMenu("Get Render")]
	public void GetRender()
	{
		if (!Application.isPlaying) return;
		StartCoroutine(GetRenderIE());
	}

	public void QuickComposition(List<ListItem> items)
	{
		
	}

	public IEnumerator GetRenderIE()
	{
		cam.targetTexture = renderTexture;
		SaveTexture("render");
		yield return new WaitForEndOfFrame();
		cam.targetTexture = null;
	}

	private Texture2D ToTexture2D(RenderTexture rTex)
	{
		Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
		RenderTexture.active = rTex;
		tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
		tex.Apply();
		Destroy(tex);
		RenderTexture.active = null;
		return tex;
	}

	private void SaveTexture(string fileName)
	{
		byte[] bytes = ToTexture2D(renderTexture).EncodeToPNG();
		File.WriteAllBytes($"{Application.persistentDataPath}/{fileName}.png", bytes);
	}

}
