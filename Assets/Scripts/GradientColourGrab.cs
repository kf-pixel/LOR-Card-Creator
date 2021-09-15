﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradientColourGrab : MonoBehaviour
{
	[SerializeField] private Canvas canvas;
	[SerializeField] private RectTransform source;
	[SerializeField] private Image gradient;
	private Texture2D texture;

	private float targetSaturation = 0.7f, targetBrightness = 0.4f;

	private IEnumerator GradientCoroutine;

	private bool instantGrab;

	private void SetGradient()
	{
		// Read Source Template
		float canvasLocalScale = canvas.transform.localScale.x;
		Vector2 sourceScreen = source.position;
		Vector2 sourceSizeScreen = source.rect.size * canvasLocalScale;

		// Create a texture the size of the screen
		int width = (int)(sourceSizeScreen.x);
		int height = (int)(sourceSizeScreen.y);

		if (texture != null) Destroy(texture);
		texture = new Texture2D(width, height, TextureFormat.ARGB32, false, true);

		// Read screen contents into the texture
		texture.ReadPixels(new Rect(sourceScreen.x, sourceScreen.y, sourceSizeScreen.x, sourceSizeScreen.y), 0, 0);
		texture.Apply();


		gradient.color = ColorBlend(texture);
	}

	public void GetGradient(bool wait)
	{
		if (instantGrab && wait) return;
		if (GradientCoroutine != null)
		{
			StopCoroutine(GradientCoroutine);
		}
		GradientCoroutine = GetGradientIE(wait);
		StartCoroutine(GradientCoroutine);
	}

	private IEnumerator GetGradientIE(bool wait)
	{
		if (wait)
		{
			yield return new WaitForSeconds(0.1f);
		}
		else
		{
			instantGrab = true;
		}
		yield return new WaitForEndOfFrame();
		SetGradient();
		GradientCoroutine = null;
		instantGrab = false;
	}
	private Color ColorBlend(Texture2D tex)
	{
		Color32[] texColors = tex.GetPixels32();

		int total = texColors.Length;

		float r = 0;
		float g = 0;
		float b = 0;

		for (int i = 0; i < total; i++)
		{

			r += texColors[i].r;

			g += texColors[i].g;

			b += texColors[i].b;

		}

        // Get average color and modified color and blend

		Color32 averageColour32 = new Color32((byte)(r / total), (byte)(g / total), (byte)(b / total), 255);
		Color averageColour = averageColour32;

		float h, s, v;
		Color.RGBToHSV(averageColour, out h, out s, out v);

		Color modifiedColour = Color.HSVToRGB(h, targetSaturation, targetBrightness);
		Color blendedColour = new Color((averageColour.r + averageColour.r + modifiedColour.r) / 3, (averageColour.g + averageColour.g + modifiedColour.g) / 3, (averageColour.b + averageColour.b + modifiedColour .b) / 3, 1f);

		return blendedColour;
	}
}