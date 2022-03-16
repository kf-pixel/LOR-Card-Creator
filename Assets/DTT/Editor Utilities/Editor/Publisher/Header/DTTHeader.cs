#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace DTT.Utils.EditorUtilities.Publishing
{
    /// <summary>
    /// Used for drawing the DTT header in the GUI.
    /// </summary>
    public class DTTHeader
    {
        #region Variables
        #region Public
        /// <summary>
        /// The styles used when drawing the header.
        /// </summary>
        public readonly DTTHeaderStyles styles;

        /// <summary>
        /// The content used for drawing the header.
        /// </summary>
        public readonly DTTHeaderContent content;
        #endregion
        #region Private
        /// <summary>
        /// The height of the header.
        /// </summary>
        private const float HEADER_HEIGHT = 40f;

        /// <summary>
        /// The margin between the icon and the labels.
        /// </summary>
        private const float LABEL_MARGIN = 5f;

        /// <summary>
        /// The total inspector margin (left + right) based on the
        /// default margin values used by unity. This is used to
        /// determine whether the inspector scroll bar is active.
        /// </summary>
        private const float TOTAL_HORIZONTAL_INSPECTOR_MARGIN = 23f;

        /// <summary>
        /// Whether the inspector scroll bar is active or not.
        /// </summary>
        private bool _activeInspectorScrollBar;

        /// <summary>
        /// The asset json of the package displayed by the header.
        /// </summary>
        private AssetJson _assetJson;
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of this object, initializing the 
        /// content and styling.
        /// </summary>
        /// <param name="packageName">The package name to display.</param>
        /// <param name="version">The package version to display.</param>
        /// <param name="documentationUrl">The url to the documentation.</param>
        public DTTHeader(AssetJson assetJson)
        {
            _assetJson = assetJson;

            styles = new DTTHeaderStyles();
            content = new DTTHeaderContent(assetJson.displayName.AddSpacesBeforeCapitals(), assetJson.version);
        }
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Draws the header at the top of the screen.
        /// </summary>
        public void OnGUI()
        {
            Rect rect = GetInitialRect();

            DrawBackground(rect);
            DrawContent(rect);

        }
        #endregion
        #region Private
        /// <summary>
        /// Returns the initial rectangle to draw the header in. Will also
        /// determine whether the inspector scroll bar is active or not.
        /// </summary>
        /// <returns>The initial rectangle to draw the header in.</returns>
        private Rect GetInitialRect()
        {
            float viewWidth = EditorGUIUtility.currentViewWidth;
            Rect rect = GUILayoutUtility.GetRect(viewWidth, HEADER_HEIGHT);

            // If the difference in width between the rectangle reserved for content
            // and the view is greater than the total horizontal inspector margin,
            // the inspector scroll bar is drawn.
            _activeInspectorScrollBar = viewWidth - rect.width >= TOTAL_HORIZONTAL_INSPECTOR_MARGIN;

            rect.x = rect.y = 0;
            rect.width = viewWidth;

            return rect;
        }

        /// <summary>
        /// Draws the background of the header.
        /// </summary>
        /// <param name="initRect">The initial rectangle the header is drawn in.</param>
        private void DrawBackground(Rect initRect)
            => EditorGUI.DrawRect(initRect, EditorGUIUtility.isProSkin ? DTTColors.dttInspectorDark : DTTColors.dttInspectorLight);

        /// <summary>
        /// Draws the header content inside the initial rectangle.
        /// </summary>
        /// <param name="initRect">The initial rectangle the header is drawn in.</param>
        private void DrawContent(Rect initRect)
        {
            DrawIcon(initRect);

            DrawTitle(initRect);
            DrawVersion(initRect);

            DrawDocumentationLink(initRect);
        }

        /// <summary>
        /// Draws the DTT icon.
        /// </summary>
        /// <param name="initRect">The initial rectangle the header is drawn in.</param>
        private void DrawIcon(Rect initRect)
        {
            Rect iconRect = new Rect(initRect);
            iconRect.width = initRect.height;

            EditorGUI.DrawRect(iconRect, DTTColors.DTTRed);
            GUI.DrawTexture(iconRect, DTTTextures.icon);
        }

        /// <summary>
        /// Draws the title.
        /// </summary>
        /// <param name="initRect">The initial rectangle the header is drawn in.</param>
        private void DrawTitle(Rect initRect)
        {
            Rect titleRect = new Rect(initRect);
            titleRect.x += HEADER_HEIGHT + LABEL_MARGIN;
            titleRect.y += LABEL_MARGIN;

            Vector2 size = content.PackageNameLabel.GetGUISize(styles.TitleLabel);
            titleRect.width = size.x;
            titleRect.height = size.y;

            GUI.Label(titleRect, content.PackageNameLabel, styles.TitleLabel);
        }

        /// <summary>
        /// Draws the documentation link.
        /// </summary>
        /// <param name="initRect">The initial rectangle the header is drawn in.</param>
        private void DrawDocumentationLink(Rect initRect)
        {
            Rect linkRect = new Rect(initRect);
            linkRect.x += HEADER_HEIGHT + LABEL_MARGIN;
            linkRect.y += (HEADER_HEIGHT * 0.5f);
            linkRect.height = content.DocumentationLabel.GetGUISize(styles.Link).y;

            if (DTTGUI.LinkLabel(linkRect, content.DocumentationLabel, styles.Link))
                DTTEditorConfig.OpenPackageDocumentation(_assetJson);
        }

        /// <summary>
        /// Draws the package version.
        /// </summary>
        /// <param name="initRect">The initial rectangle the header is drawn in.</param>
        private void DrawVersion(Rect initRect)
        {
            float scrollBarOffset = _activeInspectorScrollBar ? 18f : 0f;
            Vector2 size = content.VersionLabel.GetGUISize(styles.Label);
            Rect versionRect = new Rect(initRect);
            versionRect.x = initRect.xMax - size.x - LABEL_MARGIN - scrollBarOffset;
            versionRect.width = size.x;
            versionRect.height = size.y;
            versionRect.y += LABEL_MARGIN;

            GUI.Label(versionRect, content.VersionLabel, styles.Label);
        }
        #endregion
        #endregion
    }
}

#endif
