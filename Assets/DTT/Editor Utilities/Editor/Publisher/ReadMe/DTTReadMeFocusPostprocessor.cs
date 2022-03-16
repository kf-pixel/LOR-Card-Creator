#if UNITY_EDITOR

using System.IO;
using UnityEditor;

namespace DTT.Utils.EditorUtilities.Publishing
{
    /// <summary>
    /// A static class initialized on load that opens the ReadMe Window 
    /// the first time someone imports this package.
    /// </summary>
    public class DTTReadMeFocusPostprocessor : AssetPostprocessor
    {
        #region Methods
        #region Private

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            for (int i = 0; i < importedAssets.Length; i++)
            {
                string assetPath = importedAssets[i];
                if (PathUtility.IsAssetJson(assetPath))
                    TryFocusReadMe(assetPath);
            }
        }

        /// <summary>
        /// Focuses the <see cref="DTTReadMe"/> asset if it can.
        /// </summary>
        private static void TryFocusReadMe(string assetJsonPath)
        {
            string json = File.ReadAllText(assetJsonPath);
            AssetJson assetJson = new AssetJson();
            EditorJsonUtility.FromJsonOverwrite(json, assetJson);

            string displayName = assetJson.displayName;
            if (displayName == null || !DTTEditorConfig.HasReadMeSections(assetJson))
                return;

            string focusKey = DTTEditorConfig.GetReadMeFocusKey(assetJson.displayName);
            if (!EditorPrefs.GetBool(focusKey))
            {
                DTTReadMeEditorWindow.Open(assetJson);

                EditorPrefs.SetBool(focusKey, true);
            }
        }
        #endregion
        #endregion
    }
}

#endif