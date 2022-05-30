﻿#if UNITY_EDITOR

using DTT.Utils.EditorUtilities.Publishing;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DTT.Utils.EditorUtilities
{
    /// <summary>
    /// Updates the asset.json files of DTT packages with the relevant "assetStoreRelease"
    /// value when they are being imported in the project.
    /// </summary>
    public class ConfigPostProcessor : AssetPostprocessor
    {
        /// <summary>
        /// Checks imported assets for asset.json files.
        /// </summary>
        /// <param name="importedAssets">The imported assets.</param>
        /// <param name="deletedAssets">The deleted assets.</param>
        /// <param name="movedAssets">The moved assets.</param>
        /// <param name="movedFromAssetPaths">The moved assets from asset paths.</param>
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
                {
                    // If the asset is a DTT package with an asset.json file, update  
                    // its data to the relevant state.
                    string folder = PathUtility.GetDirectoryName(assetPath, 0);
                    bool assetStoreRelease = folder == "Assets";
                    UpdateAssetInfo(assetPath, assetStoreRelease);
                }
            }
        }

        /// <summary>
        /// Updates the asset.json file at given path with the given
        /// asset store release value.
        /// </summary>
        /// <param name="assetJsonPath">The path at which the asset.json file resides.</param>
        /// <param name="inAssetsFolder">Whether the package is the assets folder.</param>
        private static void UpdateAssetInfo(string assetJsonPath, bool inAssetsFolder)
        {
            // Get current asset info.
            string json = File.ReadAllText(assetJsonPath);
            AssetJson assetJson = new AssetJson();
            EditorJsonUtility.FromJsonOverwrite(json, assetJson);

            if (inAssetsFolder)
            {
                if (!assetJson.assetStoreRelease)
                {
                    // If the package is in the assets folder and the
                    // asset store release flag isn't set, set it.
                    assetJson.assetStoreRelease = true;
                    string updatedJson = EditorJsonUtility.ToJson(assetJson, true);
                    File.WriteAllText(assetJsonPath, updatedJson);
                }

                // Add a scripting define symbol indicating that it is an asset store release.
                AddScriptingDefineSymbolForAsset(assetJson.displayName);
            }
            else
            {
                // If the package is not in the assets folder, it should not be an asset store release.
                // Package files are immutable, that is why we can't update it manually.
                if (assetJson.assetStoreRelease)
                    Debug.LogWarning("The asset json is imported as a package but set as asset store release. " +
                        "Make sure the asset store release flag in the asset.json file is up to date.");

                // Remove the scripting define symbol indicating that it is an asset store release.
                RemoveScriptingDefineSymbolForPackage(assetJson.displayName);
            }
        }

        /// <summary>
        /// Adds a define symbol for a package that is set for asset store release.
        /// </summary>
        /// <param name="displayName">The display name of the package.</param>
        private static void AddScriptingDefineSymbolForAsset(string displayName)
        {
            string symbol = $"{displayName.FromReadableFormatToAllCaps()}_ASSET_STORE_RELEASE";
            PlayerSettingsUtility.AddScriptingDefineSymbol(symbol);
        }

        /// <summary>
        /// Removes a define symbol for a package that is set not for asset store release.
        /// </summary>
        /// <param name="displayName">The display name of the package.</param>
        private static void RemoveScriptingDefineSymbolForPackage(string displayName)
        {
            string symbol = $"{displayName.FromReadableFormatToAllCaps()}_ASSET_STORE_RELEASE";
            PlayerSettingsUtility.RemoveScriptingDefineSymbol(symbol);
        }
    }
}

#endif