#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace DTT.Utils.EditorUtilities
{
    /// <summary>
    /// The styles used for drawing the readme editor.
    /// </summary>
    public class DTTReadMeStyles : GUIStyleCache
    {
        #region Variables
        #region Public
        /// <summary>
        /// The content title style.
        /// </summary>
        public GUIStyle ContentTitle => base[nameof(ContentTitle)];

        /// <summary>
        /// The content style.
        /// </summary>
        public GUIStyle Content => base[nameof(Content)];

        /// <summary>
        /// The content link style.
        /// </summary>
        public GUIStyle ContentLink => base[nameof(ContentLink)];

        /// <summary>
        /// The inline content link style.
        /// </summary>
        public GUIStyle InlineContentLink => base[nameof(InlineContentLink)];
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance initializing the styles.
        /// </summary>
        public DTTReadMeStyles()
        {
            Add(nameof(ContentTitle), () =>
            {
                GUIStyle style = new GUIStyle(DTTGUI.skin.label);
                style.fontSize = 16;
                style.margin.top = 3;
                style.margin.bottom = 3;
                style.font = DTTGUI.titleFont;
                style.normal.textColor = EditorGUIUtility.isProSkin ? DTTColors.labelGrey : DTTColors.unityInspectorDark;
                return style;
            });

            Add(nameof(Content), () =>
            {
                GUIStyle style = new GUIStyle(DTTGUI.skin.label);
                style.fontSize = 13;
                style.margin.top = 3;
                style.margin.bottom = 3;
                style.wordWrap = true;
                style.normal.textColor = EditorGUIUtility.isProSkin ? DTTColors.labelGrey : DTTColors.unityInspectorDark;
                return style;
            });

            Add(nameof(ContentLink), () =>
            {
                GUIStyle style = new GUIStyle(DTTGUI.styles.LinkLabel);
                style.fontSize = 13;
                style.padding.left = 3;
                style.margin.top = 3;
                style.margin.bottom = 3;
                style.normal.textColor = DTTColors.DTTRed;
                style.active.textColor = DTTColors.DTTRed;
                style.hover.textColor = DTTColors.DTTRed;
                return style;
            });

            Add(nameof(InlineContentLink), () =>
            {
                GUIStyle style = new GUIStyle(DTTGUI.styles.LinkLabel);
                style.fontSize = 13;
                style.padding.left = 0;
                style.padding.top = 5;
                style.margin.top = 3;
                style.margin.bottom = 3;
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
