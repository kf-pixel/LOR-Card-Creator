#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace DTT.Utils.EditorUtilities
{
    /// <summary>
    /// Contains pre-defined colors used for styling your GUI.
    /// </summary>
    public static class DTTColors
    {
        #region Variables
        #region Public
        /// <summary>
        /// The dark theme color for the inspector used by Unity.
        /// </summary>
        public static readonly Color unityInspectorDark;

        /// <summary>
        /// The light theme color for the inspector used by Unity.
        /// </summary>
        public static readonly Color unityInspectorLight;

        /// <summary>
        /// The dark theme color for the inspector used by DTT.
        /// </summary>
        public static readonly Color dttInspectorDark;

        /// <summary>
        /// The light theme color for the inspector used by DTT.
        /// </summary>
        public static readonly Color dttInspectorLight;

        /// <summary>
        /// A grey label color.
        /// </summary>
        public static readonly Color labelGrey;

        /// <summary>
        /// A dark line color.
        /// </summary>
        public static readonly Color lineDark;

        /// <summary>
        /// A light line color.
        /// </summary>
        public static readonly Color lineLight;

        /// <summary>
        /// The DTT red color.
        /// </summary>
        public static Color DTTRed => EditorGUIUtility.isProSkin ?
            new Color32(208, 83, 64, 255) : new Color32(235, 83, 64, 255);

        /// <summary>
        /// The color used for end lines in the inspector.
        /// </summary>
        public static Color LineColor => EditorGUIUtility.isProSkin ? lineDark : lineLight;
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the static instance, initializing the field color values.
        /// </summary>
        static DTTColors()
        {

            unityInspectorDark = new Color32(62, 62, 62, 255);
            unityInspectorLight = new Color32(203, 203, 203, 255);

            dttInspectorDark = new Color32(40, 40, 40, 255);
            dttInspectorLight = new Color32(235, 235, 235, 255);

            labelGrey = new Color32(196, 196, 196, 255);

            lineDark = new Color32(26, 26, 26, 255);
            lineLight = new Color32(127, 127, 127, 255);
        }
        #endregion
    }
}

#endif