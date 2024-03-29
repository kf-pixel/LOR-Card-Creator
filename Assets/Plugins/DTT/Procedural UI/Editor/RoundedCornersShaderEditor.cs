using DTT.Utils.EditorUtilities;
using DTT.Utils.EditorUtilities.Publishing;
using UnityEditor;

namespace DTT.UI.ProceduralUI.Editor
{
	/// <summary>
	/// Custom editor for the rounded corners shader.
	/// </summary>
	public class RoundedCornersShaderEditor : ShaderGUI
	{
		#region Variables
		#region Private
		/// <summary>
		/// The DTT header that is drawn on top of the shader settings.
		/// </summary>
		private DTTHeader _header;

		/// <summary>
		/// Whether this editor has been initialized.
		/// </summary>
		private bool _initialized = false;
		#endregion
		#endregion

		#region Unity Methods
		/// <summary>
		/// Draws the DTT header on top of the shader settings.
		/// </summary>
		/// <param name="materialEditor"><inheritdoc/></param>
		/// <param name="properties"><inheritdoc/></param>
		public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
		{
			if (!_initialized)
				Initialize();

			if (_header != null)
				_header.OnGUI();
			base.OnGUI(materialEditor, properties);
		}
		#endregion

		#region Methods
		#region Private
		/// <summary>
		/// Initializes the editor.
		/// </summary>
		private void Initialize()
		{
			_initialized = true;
			AssetJson assetJson = DTTEditorConfig.GetAssetJson("dtt.proceduralui");
			if (assetJson != null)
				_header = new DTTHeader(assetJson);
		}
		#endregion
		#endregion
	}
}