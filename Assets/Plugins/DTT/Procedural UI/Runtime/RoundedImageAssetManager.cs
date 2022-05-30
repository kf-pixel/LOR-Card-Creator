using UnityEngine;

namespace DTT.UI.ProceduralUI
{
	/// <summary>
	/// Manages the assets for <see cref="RoundedImage"/>
	/// </summary>
	public static class RoundedImageAssetManager
	{
		#region Variables
		#region Public
		/// <summary>
		/// The shader used for filled Rounded Images that have a border.
		/// </summary>
		public static Shader RoundingShader => _roundingShader;
		#endregion

		#region Private
		/// <summary>
		/// Cached material used for Rounded Images.
		/// </summary>
		private static Material _material;

		/// <summary>
		/// The shader used for filled Rounded Images that have a border.
		/// </summary>
		private static Shader _roundingShader;
		#endregion

		#region Public
		#region Consts
		/// <summary>
		/// The shader name of the border shader.
		/// Can be used in <see cref="Shader.Find(string)"/>
		/// </summary>
		public const string ROUNDING_SHADER_NAME = "UI/RoundedCorners/RoundedCorners";
		#endregion
		#endregion
		#endregion

		#region Initialization
		/// <summary>
		/// Find the relevant shader and initialize the cached material.
		/// </summary>
		static RoundedImageAssetManager()
		{
			_roundingShader = Shader.Find(ROUNDING_SHADER_NAME);
			_material = new Material(_roundingShader);
		}
		#endregion

		#region Methods
		#region Public
		/// <summary>
		/// Get the rounding material.
		/// </summary>
		/// <returns>The material that rounds according to the mode.</returns>
		public static Material GetRoundingMaterial()
		{
			if (_material == null)
				_material = new Material(_roundingShader);

			return _material;
		}
		#endregion
		#endregion
	}
}