#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace DTT.Utils.EditorUtilities
{
    /// <summary>
    /// Additional styles used for styling a GUI in DTT style. Styles
    /// will differ based on Unity's Dark/Light theme.
    /// </summary>
    public class DTTGUIStyles : GUIStyleCache
    {
        #region Variables
        #region Public
        /// <summary>
        /// The style of a normal link label.
        /// </summary>
        public GUIStyle LinkLabel => base[nameof(LinkLabel)];

        /// <summary>
        /// The style of a small link label.
        /// </summary>
        public GUIStyle MiniLinkLabel => base[nameof(MiniLinkLabel)];

        /// <summary>
        /// A style for a normal label.
        /// </summary>
        public GUIStyle Label => base[nameof(Label)];

        /// <summary>
        /// A style for a label that uses the dtt title font.
        /// </summary>
        public GUIStyle TitleLabel => base[nameof(TitleLabel)];

        /// <summary>
        /// The style for a card header.
        /// </summary>
        public GUIStyle CardHeader => base[nameof(CardHeader)];

        /// <summary>
        /// The style for a card body.
        /// </summary>
        public GUIStyle CardBody => base[nameof(CardBody)];

        /// <summary>
        /// The style for a button.
        /// </summary>
        public GUIStyle Button => base[nameof(Button)];
        #endregion
        #region Private
        /// <summary>
        /// The normal color of link text when not in pro mode.
        /// </summary>
        private readonly Color _linkBlue = new Color32(0, 82, 255, 255);
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance, initializing the styles.
        /// </summary>
        public DTTGUIStyles()
        {
            Add(nameof(LinkLabel), () =>
            {
                GUIStyle style = new GUIStyle(EditorStyles.linkLabel);
                style.contentOffset = new Vector2(0, -2f);
                style.clipping = TextClipping.Overflow;

                if (!EditorGUIUtility.isProSkin)
                {
                    style.normal.textColor = _linkBlue;
                    style.hover.textColor = _linkBlue;
                }

                return style;
            });

            Add(nameof(MiniLinkLabel), () =>
            {
                GUIStyle style = new GUIStyle(LinkLabel);
                style.fontSize = EditorStyles.miniLabel.fontSize;
                return style;
            });

            Add(nameof(Label), () =>
            {
                GUIStyle style = new GUIStyle(DTTGUI.skin.label);
                style.normal.textColor = EditorStyles.label.normal.textColor;
                style.fontSize = 12;
                return style;
            });

            Add(nameof(TitleLabel), () =>
            {
                GUIStyle style = new GUIStyle(DTTGUI.skin.label);
                style.font = DTTGUI.titleFont;
                style.normal.textColor = EditorStyles.label.normal.textColor;
                style.fontSize = 12;
                return style;
            });

            Add(nameof(CardHeader), () =>
            {
                GUIStyle style = new GUIStyle(DTTGUI.skin.box);

                // No scaled backgrounds should be used, only use the card background texture.
                style.normal.scaledBackgrounds = null;
                style.normal.background = EditorGUIUtility.isProSkin ? DTTTextures.cardHeaderDark : DTTTextures.cardHeaderLight;

                // The border property ensures the rounding of the edges persists with stretch.
                style.border = new RectOffset(16, 16, 16, 16);

                // Set the margin and padding to work with the card body.
                style.margin.top = 8;
                style.margin.left = 8;
                style.margin.right = 8;
                style.margin.bottom = 0;
                style.padding = new RectOffset(15, 15, 15, 15);
                return style;
            });

            Add(nameof(CardBody), () =>
            {
                GUIStyle style = new GUIStyle(DTTGUI.skin.box);

                // No scaled backgrounds should be used, only use the card background texture.
                style.normal.scaledBackgrounds = null;
                style.normal.background = EditorGUIUtility.isProSkin ? DTTTextures.cardBodyDark : DTTTextures.cardBodyLight;

                // The border property ensures the rounding of the edges persists with stretch.
                style.border = new RectOffset(16, 16, 16, 16);

                // Set the margin and padding to work with the card header.
                style.margin.top = 0;
                style.margin.left = 8;
                style.margin.right = 8;
                style.padding = new RectOffset(15, 15, 15, 15);
                return style;
            });

            Add(nameof(Button), () =>
            {
                GUIStyle style = new GUIStyle(DTTGUI.skin.button);
                style.normal = EditorStyles.miniButton.normal;
                style.hover = EditorStyles.miniButton.hover;
                style.active = EditorStyles.miniButton.active;
                return style;
            });
        }
        #endregion
    }
}

#endif