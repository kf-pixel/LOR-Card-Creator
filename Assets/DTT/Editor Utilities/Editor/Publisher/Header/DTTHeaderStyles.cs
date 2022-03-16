#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace DTT.Utils.EditorUtilities.Publishing
{
    /// <summary>
    /// Contains the styles used for drawing the header.
    /// </summary>
    public class DTTHeaderStyles : GUIStyleCache
    {
        #region Variables
        #region Public
        /// <summary>
        /// Used for drawing a label inside the header.
        /// </summary>
        public GUIStyle Label => base[nameof(Label)];

        /// <summary>
        /// Used for drawing the company name inside the header.
        /// </summary>
        public GUIStyle TitleLabel => base[nameof(TitleLabel)];

        /// <summary>
        /// The link style.
        /// </summary>
        public GUIStyle Link => base[nameof(Link)];
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of this object, initializing
        /// the styles.
        /// </summary>
        public DTTHeaderStyles()
        {
            Add(nameof(Label), () =>
            {
                GUIStyle style = new GUIStyle(DTTGUI.skin.label);
                style.fontSize = 11;
                style.normal.textColor = EditorGUIUtility.isProSkin ? DTTColors.unityInspectorLight : Color.black;
                return style;
            });

            Add(nameof(TitleLabel), () =>
            {
                GUIStyle style = new GUIStyle(DTTGUI.skin.label);
                style.fontSize = 13;
                style.font = DTTGUI.titleFont;
                style.wordWrap = true;
                style.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : DTTColors.dttInspectorDark;
                return style;
            });

            Add(nameof(Link), () =>
            {
                GUIStyle style = new GUIStyle(DTTGUI.styles.MiniLinkLabel);
                style.fontSize = 10;
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = DTTColors.DTTRed;
                style.active.textColor = DTTColors.DTTRed;
                style.hover.textColor = DTTColors.DTTRed;
                return style;
            });
        }
        #endregion
    }
}

#endif