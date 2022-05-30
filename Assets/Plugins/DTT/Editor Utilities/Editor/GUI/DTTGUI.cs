#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DTT.Utils.EditorUtilities
{
    /// <summary>
    /// A static class used for drawing the graphical user interface
    /// in DTT style.
    /// </summary>
    public static class DTTGUI
    {
        #region Variables
        #region Public
        /// <summary>
        /// The skin used by DTT.
        /// </summary>
        public static readonly GUISkin skin;

        /// <summary>
        /// The title font used by DTT.
        /// </summary>
        public static readonly Font titleFont;

        /// <summary>
        /// Additional styles used for styling a GUI in DTT style. Styles
        /// will differ based on Unity's Dark/Light theme.
        /// </summary>
        public static readonly DTTGUIStyles styles;
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the static instance, initializing the static state.
        /// </summary>
        static DTTGUI()
        {
            // Load the dtt skin.
            skin = AssetDatabase.LoadAssetAtPath<GUISkin>(DTTEditorConfig.SkinPath);
            if (skin == null)
                Debug.LogWarning($"Failed loading skin. Make sure it is present at {DTTEditorConfig.SkinPath}");

            titleFont = AssetDatabase.LoadAssetAtPath<Font>(Path.Combine(DTTEditorConfig.FontFolder, "Montserrat-ExtraBold.ttf"));
            styles = new DTTGUIStyles();
        }
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Draws a clickable link label.
        /// </summary>
        /// <param name="rect">The position.</param>
        /// <param name="label">The label text.</param>
        /// <returns>Whether the link label has been clicked.</returns>
        public static bool LinkLabel(Rect rect, string label)
        {
            GUIStyle style = styles.LinkLabel;

            float width = style.CalcSize(new GUIContent(label)).x;
            Rect textRect = new Rect(rect);
            textRect.xMax = rect.xMin + width;

            Vector2 offset = style.contentOffset;
            RectOffset padding = style.padding;

            Vector3 start = new Vector2(rect.xMin + padding.left + offset.x, rect.yMax + offset.y);
            Vector3 end = new Vector2(textRect.xMax - padding.right, rect.yMax + offset.y);

            Handles.BeginGUI();
            Handles.color = style.normal.textColor;
            Handles.DrawLine(start, end);
            Handles.color = Color.white;
            Handles.EndGUI();

            EditorGUIUtility.AddCursorRect(textRect, MouseCursor.Link);

            return GUI.Button(textRect, label, style);
        }

        /// <summary>
        /// Draws a clickable link label.
        /// </summary>
        /// <param name="rect">The position.</param>
        /// <param name="content">The text content.</param>
        /// <returns>Whether the link label has been clicked.</returns>
        public static bool LinkLabel(Rect rect, GUIContent content)
        {
            GUIStyle style = styles.LinkLabel;

            float width = style.CalcSize(content).x;
            Rect textRect = new Rect(rect);
            textRect.xMax = rect.xMin + width;

            Vector2 offset = style.contentOffset;
            RectOffset padding = style.padding;

            Vector3 start = new Vector2(rect.xMin + padding.left + offset.x, rect.yMax + offset.y);
            Vector3 end = new Vector2(textRect.xMax - padding.right, rect.yMax + offset.y);

            Handles.BeginGUI();
            Handles.color = style.normal.textColor;
            Handles.DrawLine(start, end);
            Handles.color = Color.white;
            Handles.EndGUI();

            EditorGUIUtility.AddCursorRect(textRect, MouseCursor.Link);

            return GUI.Button(textRect, content, style);
        }

        /// <summary>
        /// Draws a clickable link label.
        /// </summary>
        /// <param name="rect">The position.</param>
        /// <param name="content">The text content.</param>
        /// <param name="linkStyle">The link style.</param>
        /// <returns>Whether the link label has been clicked.</returns>
        public static bool LinkLabel(Rect rect, GUIContent content, GUIStyle linkStyle)
        {
            float width = linkStyle.CalcSize(content).x;
            Rect textRect = new Rect(rect);
            textRect.xMax = rect.xMin + width;

            Vector2 offset = linkStyle.contentOffset;
            RectOffset padding = linkStyle.padding;

            Vector3 start = new Vector2(rect.xMin + padding.left + offset.x, rect.yMax + offset.y);
            Vector3 end = new Vector2(textRect.xMax - padding.right, rect.yMax + offset.y);

            Handles.BeginGUI();
            Handles.color = linkStyle.normal.textColor;
            Handles.DrawLine(start, end);
            Handles.color = Color.white;
            Handles.EndGUI();

            EditorGUIUtility.AddCursorRect(textRect, MouseCursor.Link);

            return GUI.Button(textRect, content, linkStyle);
        }

        /// <summary>
        /// Draws a clickable link label.
        /// </summary>
        /// <param name="rect">The position.</param>
        /// <param name="label">The text content.</param>
        /// <param name="linkStyle">The link style.</param>
        /// <returns>Whether the link label has been clicked.</returns>
        public static bool LinkLabel(Rect rect, string label, GUIStyle linkStyle)
        {
            float width = linkStyle.CalcSize(new GUIContent(label)).x;
            Rect textRect = new Rect(rect);
            textRect.xMax = rect.xMin + width;

            Vector2 offset = linkStyle.contentOffset;
            RectOffset padding = linkStyle.padding;

            Vector3 start = new Vector2(rect.xMin + padding.left + offset.x, rect.yMax + offset.y);
            Vector3 end = new Vector2(textRect.xMax - padding.right, rect.yMax + offset.y);

            Handles.BeginGUI();
            Handles.color = linkStyle.normal.textColor;
            Handles.DrawLine(start, end);
            Handles.color = Color.white;
            Handles.EndGUI();

            EditorGUIUtility.AddCursorRect(textRect, MouseCursor.Link);

            return GUI.Button(textRect, label, linkStyle);
        }

        /// <summary>
        /// Draws something inside a <see cref="GUI.BeginGroup(Rect)"/>.
        /// </summary>
        /// <param name="rect">The position of the group.</param>
        /// <param name="action">The drawing action.</param>
        public static void DrawGrouped(Rect rect, Action action)
        {
            GUI.BeginGroup(rect);
            action?.Invoke();
            GUI.EndGroup();
        }

        /// <summary>
        /// Draws something inside a <see cref="GUI.BeginGroup(Rect)"/>.
        /// </summary>
        /// <param name="rect">The position of the group.</param>
        /// <param name="style">The style of the group.</param>
        /// <param name="action">The drawing action.</param>
        public static void DrawGrouped(Rect rect, GUIStyle style, Action action)
        {
            GUI.BeginGroup(rect, style);
            action?.Invoke();
            GUI.EndGroup();
        }
        #endregion
        #endregion
    }
}

#endif