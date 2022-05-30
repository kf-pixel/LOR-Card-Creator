#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace DTT.Utils.EditorUtilities
{
    /// <summary>
    /// A static class used for drawing in the graphical user interface
    /// in DTT style without use of rectangles.
    /// </summary>
    public static class DTTGUILayout
    {
        #region Methods
        #region Public
        /// <summary>
        /// Draws a clickable link label.
        /// </summary>
        /// <param name="label">The label text.</param>
        /// <returns>Whether the link label has been clicked.</returns>
        public static bool LinkLabel(string label)
        {
            GUIStyle style = DTTGUI.styles.LinkLabel;

            Vector2 size = label.GetGUISize(style);
            Rect rect = GUILayoutUtility.GetRect(size.x, size.y);
            rect.x += style.padding.left;

            return DTTGUI.LinkLabel(rect, label, style);
        }

        /// <summary>
        /// Draws a clickable link label.
        /// </summary>
        /// <param name="content">The text content.</param>
        /// <returns>Whether the link label has been clicked.</returns>
        public static bool LinkLabel(GUIContent content)
        {
            GUIStyle style = DTTGUI.styles.LinkLabel;

            Vector2 size = content.GetGUISize(style);
            Rect rect = GUILayoutUtility.GetRect(size.x, size.y);
            rect.x += style.padding.left;

            return DTTGUI.LinkLabel(rect, content, style);
        }

        /// <summary>
        /// Draws a clickable label in given link style.
        /// </summary>
        /// <param name="label">The label text.</param>
        /// <param name="linkStyle">The link style.</param>
        /// <returns>Whether the link has been clicked.</returns>
        public static bool LinkLabel(string label, GUIStyle linkStyle)
        {
            Vector2 size = label.GetGUISize(linkStyle);
            Rect rect = GUILayoutUtility.GetRect(size.x, size.y);
            rect.x += linkStyle.padding.left;

            return DTTGUI.LinkLabel(rect, label, linkStyle);
        }

        /// <summary>
        /// Draws a clickable label in given link style.
        /// </summary>
        /// <param name="labelContent">The label content text.</param>
        /// <param name="linkStyle">The link style.</param>
        /// <returns>Whether the link has been clicked.</returns>
        public static bool LinkLabel(GUIContent labelContent, GUIStyle linkStyle)
        {
            Vector2 size = labelContent.GetGUISize(linkStyle);
            Rect rect = GUILayoutUtility.GetRect(size.x, size.y);
            rect.x += linkStyle.padding.left;

            return DTTGUI.LinkLabel(rect, labelContent, linkStyle);
        }

        /// <summary>
        /// Draws a horizontal separating line in given color.
        /// </summary>
        /// <param name="color">The color in which to draw the line.</param>
        /// <param name="margin">The margin relative relative to the view it is in.</param>
        public static void Separator(Color color, RectOffset margin = null)
        {
            Rect rect = GUILayoutUtility.GetRect(GUILayoutUtility.GetLastRect().x, 1f);
            RectOffset offset = margin ?? new RectOffset();

            Color originalColor = Handles.color;

            Handles.BeginGUI();
            Handles.color = color;

            float yPosition = rect.yMax - offset.top + offset.bottom;
            Vector3 left = new Vector3(rect.xMin + offset.left, yPosition);
            Vector3 right = new Vector3(rect.xMax - offset.right, yPosition);
            Handles.DrawLine(left, right);

            Handles.color = originalColor;
            Handles.EndGUI();
        }

        /// <summary>
        /// Draws a horizontal separating line in the current editor theme color.
        /// </summary>
        /// <param name="margin">The margin relative relative to the view it is in.</param>
        public static void Separator(RectOffset margin = null)
        {
            Color color = EditorGUIUtility.isProSkin ? DTTColors.lineLight : DTTColors.unityInspectorLight;
            Separator(color, margin);
        }

        /// <summary>
        /// Draws a card header with the content drawn by given action inside it.
        /// </summary>
        /// <param name="action">The action that draws the content inside.</param>
        public static void CardHeader(Action action)
        {
            EditorGUILayout.BeginHorizontal(DTTGUI.styles.CardHeader);
            EditorGUILayout.BeginVertical();
            action.Invoke();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws a card body with the content drawn by given action inside it.
        /// </summary>
        /// <param name="action">The action that draws the content inside.</param>
        public static void CardBody(Action action)
        {
            EditorGUILayout.BeginHorizontal(DTTGUI.styles.CardBody);
            EditorGUILayout.BeginVertical();
            action.Invoke();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        #endregion
        #endregion
    }
}

#endif
