using UnityEngine;

[CreateAssetMenu(fileName = "New Colour Style", menuName = "Colour Style")]
public class ColourStyle : ScriptableObject
{
	public Color hover;
	public Color hoverText;
	public Color pressed;
	public Color pressedText;

	[Header("For Toggle Components")]
	public Color active;
	public Color activeText;

	[Header("Optional Material change")]
	public Material hoverMaterial;
}