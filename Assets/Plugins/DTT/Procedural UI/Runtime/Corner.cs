using UnityEngine;

namespace DTT.UI.ProceduralUI
{
	/// <summary>
	/// Mappings for the corners used by <see cref="RoundedImage"/>.
	/// </summary>
    public enum Corner
    {
        #region Values
		[InspectorName("Top Left")]
		TOP_LEFT = 0,
		[InspectorName("Top Right")]
		TOP_RIGHT = 1,
		[InspectorName("Bottom Left")]
		BOTTOM_LEFT = 2,
		[InspectorName("Bottom Right")]
		BOTTOM_RIGHT = 3,
        #endregion
    }
}