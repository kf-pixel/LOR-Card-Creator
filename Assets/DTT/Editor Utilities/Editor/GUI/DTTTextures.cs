#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

namespace DTT.Utils.EditorUtilities
{
    /// <summary>
    /// Contains the textures used as part of the DTT styling. 
    /// </summary>
    public static class DTTTextures
    {
        /// <summary>
        /// The DTT icon, without background.
        /// </summary>
        public static readonly Texture icon;

        /// <summary>
        /// The DTT logo, for light theme.
        /// </summary>
        public static readonly Texture logoLight;

        /// <summary>
        /// The DTT logo, for dark theme.
        /// </summary>
        public static readonly Texture logoDark;

        /// <summary>
        /// The card header texture used with the light theme.
        /// </summary>
        public static readonly Texture2D cardHeaderLight;

        /// <summary>
        /// The card body texture used with the light theme.
        /// </summary>
        public static readonly Texture2D cardBodyLight;

        /// <summary>
        /// The card header texture used with the dark theme.
        /// </summary>
        public static readonly Texture2D cardHeaderDark;

        /// <summary>
        /// The card body texture used with the dark theme.
        /// </summary>
        public static readonly Texture2D cardBodyDark;

        /// <summary>
        /// Loads the textures.
        /// </summary>
        static DTTTextures()
        {
            icon = AssetDatabase.LoadAssetAtPath<Texture>(Path.Combine(DTTEditorConfig.ArtFolder, "icon-large.png"));
            cardHeaderLight = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(DTTEditorConfig.ArtFolder, "card_header_light.png"));
            cardBodyLight = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(DTTEditorConfig.ArtFolder, "card_body_light.png"));
            cardHeaderDark = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(DTTEditorConfig.ArtFolder, "card_header_dark.png"));
            cardBodyDark = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(DTTEditorConfig.ArtFolder, "card_body_dark.png"));
            logoLight = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(DTTEditorConfig.ArtFolder, "logo_dark.png"));
            logoDark = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(DTTEditorConfig.ArtFolder, "logo_white.png"));
        }
    }
}

#endif