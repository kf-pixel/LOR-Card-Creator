#if UNITY_EDITOR

using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace DTT.Utils.EditorUtilities.Publishing
{
    /// <summary>
    /// Draws the editor readme content.
    /// </summary>
    public class DTTReadMeEditorWindow : EditorWindow
    {
        #region Variables
        #region Private
        /// <summary>
        /// The read me target to be edited.
        /// </summary>
        [SerializeField]
        private DTTReadMe _readMe;

        /// <summary>
        /// The asset json of the package the readme is for.
        /// </summary>
        [SerializeField]
        private AssetJson _assetJson;

        /// <summary>
        /// The styles to use for displaying the readme window.
        /// </summary>
        private DTTReadMeStyles _styles;

        /// <summary>
        /// The minimum size of this window.
        /// </summary>
        private readonly Vector2 _minSize = new Vector2(500, 450);

        /// <summary>
        /// The current scroll position off the readme.
        /// </summary>
        private Vector2 _scrollPosition = Vector2.zero;

        /// <summary>
        /// The regular expression used for matching content in readme sections.
        /// </summary>
        private readonly Regex _contentMatcher = new Regex(@"\[(.*?)\]");

        /// <summary>
        /// The character used to identify a link in readme content.
        /// </summary>
        private const string LINK_CHARACTER = "l";

        /// <summary>
        /// The character used to identify an image in readme content.
        /// </summary>
        private const string IMAGE_CHARACTER = "i";
        #endregion
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Opens the window to show the readme of given asset json, 
        /// docking it onto the inspector window if possible.
        /// </summary>
        /// <returns>The window instance.</returns>
        public static DTTReadMeEditorWindow Open(AssetJson assetJson)
        {
            Assembly editorAssembly = typeof(Editor).Assembly;
            Type inspectorType = editorAssembly.GetType("UnityEditor.InspectorWindow");

            string windowName = assetJson.displayName + " ReadMe";
            DTTReadMeEditorWindow window = GetWindow<DTTReadMeEditorWindow>(windowName, true, inspectorType);
            window.Initialize(assetJson);

            return window;
        }
        #endregion
        #region Private
        /// <summary>
        /// Initializes the window state.
        /// </summary>
        private void OnEnable()
        {
            if (_assetJson != null)
                _readMe.Initialize(_assetJson);

            _styles = new DTTReadMeStyles();

            minSize = _minSize;
        }

        /// <summary>
        /// Draws the ReadMe card.
        /// </summary>
        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            DTTGUILayout.CardHeader(DrawDTTHeaderContent);
            DTTGUILayout.CardBody(DrawSections);
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Initialezes the content to be shown based on the given asset json.
        /// </summary>
        /// <param name="assetJson">The asset json of a package.</param>
        private void Initialize(AssetJson assetJson)
        {
            _assetJson = assetJson;
            _readMe = new DTTReadMe();
            _readMe.Initialize(_assetJson);
        }

        /// <summary>
        /// Draws the DTT header content.
        /// </summary>
        private void DrawDTTHeaderContent()
        {
            Texture icon = EditorGUIUtility.isProSkin ? DTTTextures.logoDark : DTTTextures.logoLight;
            if (icon != null)
            {
                Vector2 size = new Vector2(icon.width * 0.5f, icon.height * 0.5f);

                EditorGUILayout.BeginHorizontal(GUILayout.Width(size.x));
                DrawIcon(icon, size);
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Draws the given icon in given icon size.
        /// </summary>
        /// <param name="icon">The icon to draw.</param>
        /// <param name="iconSize">The size in which to draw the icon.</param>
        private void DrawIcon(Texture icon, Vector2 iconSize)
        {
            Rect iconRect = GUILayoutUtility.GetRect(iconSize.x, iconSize.y);
            GUI.DrawTexture(iconRect, icon);
        }

        /// <summary>
        /// Draws the different readme sections.
        /// </summary>
        private void DrawSections()
        {
            foreach (ReadMeSection section in _readMe.loadedSections)
            {
                DrawSectionHeader(section);
                DrawSectionContent(section);
            }
        }

        /// <summary>
        /// Draws the header for given section.
        /// </summary>
        /// <param name="section">The section to draw the header for.</param>
        private void DrawSectionHeader(ReadMeSection section)
        {
            EditorGUILayout.Space();
            GUILayout.Label(section.content.title, _styles.ContentTitle);
            RectOffset margin = new RectOffset(2, 2, 0, 0);
            DTTGUILayout.Separator(EditorGUIUtility.isProSkin ? DTTColors.lineLight : DTTColors.unityInspectorLight, margin);
        }

        /// <summary>
        /// Draws the content for given section.
        /// </summary>
        /// <param name="section">The section to draw content for.</param>
        private void DrawSectionContent(ReadMeSection section)
        {
            int linkCount = 0;
            int imageCount = 0;

            MatchCollection matches = _contentMatcher.Matches(section.content.text);
            foreach (Match match in matches)
            {
                string content = match.Groups[1].Value;
                switch (content)
                {
                    case LINK_CHARACTER:
                        ReadMeSection.LinkContent link = section.GetLink(linkCount++);
                        DrawLinkContent(link);
                        break;

                    case IMAGE_CHARACTER:
                        Texture image = section.GetImage(imageCount++, _readMe.SectionsFolder);
                        Rect rect = GUILayoutUtility.GetRect(image.width, image.height);
                        GUI.DrawTexture(rect, image, ScaleMode.ScaleToFit);
                        break;

                    default:
                        GUILayout.Label(content, _styles.Content);
                        break;
                }
            }
        }

        /// <summary>
        /// Draws the GUI for given link content.
        /// </summary>
        /// <param name="link">The link content to draw the GUI of.</param>
        private void DrawLinkContent(ReadMeSection.LinkContent link)
        {
            if (link != null)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    DrawPrefix(link.prefix);
                    DrawLink(link);
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                Debug.LogWarning("Link was null. Make sure the links match the [l] amount in your text.");
            }
        }

        /// <summary>
        /// Draws the given prefix of a link.
        /// </summary>
        /// <param name="prefix">The prefix of the link.</param>
        private void DrawPrefix(string prefix)
        {
            if (!string.IsNullOrEmpty(prefix))
                GUILayout.Label(prefix, _styles.Content);
        }

        /// <summary>
        /// Draws the link label of given link content.
        /// </summary>
        /// <param name="link">The link content.</param>
        private void DrawLink(ReadMeSection.LinkContent link)
        {
            GUIStyle style = link.inline ? _styles.InlineContentLink : _styles.ContentLink;
            if (DTTGUILayout.LinkLabel(link.text, style))
            {
                if (link.url.StartsWith("mailto:"))
                    Application.OpenURL(link.url);
                else
                    DTTEditorConfig.OpenPackageLink(_assetJson, link.url);
            }
        }
        #endregion
        #endregion
    }
}

#endif