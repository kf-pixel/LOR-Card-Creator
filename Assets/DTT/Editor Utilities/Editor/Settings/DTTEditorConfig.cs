#if UNITY_EDITOR

using DTT.Utils.EditorUtilities.Publishing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace DTT.Utils.EditorUtilities
{
    /// <summary>
    /// The configuration settings used for the Editor Utilities package.
    /// </summary>
    public static class DTTEditorConfig
    {
        #region Variables
        #region Public
        /// <summary>
        /// The path relative to the editor utilities folder where the dtt gui skin is stored.
        /// </summary>
        public static string SkinPath => Path.Combine(EditorUtilitiesFolder, SKIN_PATH_RELATIVE);

        /// <summary>
        /// The path relative to the editor utilities folder towards the folder in which
        /// the readme sections used in the DTT readme are stored.
        /// </summary>
        public static string ReadMeSectionsFolder => Path.Combine(EditorUtilitiesFolder, DTT_README_SECTIONS_FOLDER_RELATIVE);

        /// <summary>
        /// The path relative to the editor utilities folder towards the readme asset.
        /// </summary>
        public static string ReadMeLoadPath => Path.Combine(EditorUtilitiesFolder, DTT_README_LOAD_PATH_RELATIVE);

        /// <summary>
        /// The folder relative to the editor utilities folder in which editor utility art is stored.
        /// </summary>
        public static string ArtFolder => Path.Combine(EditorUtilitiesFolder, "Editor", "Art");

        /// <summary>
        /// The folder relative to the editor utilities folder in which editor utility fonts are stored.
        /// </summary>
        public static string FontFolder => Path.Combine(EditorUtilitiesFolder, "Editor", "Fonts");

        /// <summary>
        /// The folder in the project in which all DTT assets should be found. 
        /// </summary>
        public static string DTTProjectFolder
        {
            get
            {
                if (!Directory.Exists(_dttProjectFolder))
                {
                    // If the dtt project folder doesn't exist, look for it in the project.
                    string folder = Directory.GetDirectories("Assets", "DTT", SearchOption.AllDirectories).FirstOrDefault();
                    if (string.IsNullOrEmpty(folder))
                    {
                        string defaultPath = DEFAULT_DTT_PROJECT_FOLDER;
                        Debug.Log("No DTT folder was found in the project. Creating one at " + defaultPath);

                        Directory.CreateDirectory(defaultPath);
                        AssetDatabase.ImportAsset(defaultPath);

                        _dttProjectFolder = defaultPath;
                    }
                    else
                    {
                        _dttProjectFolder = folder;
                    }
                }

                return _dttProjectFolder;
            }
        }

        /// <summary>
        /// The full dtt project folder path.
        /// </summary>
        public static string FullDTTProjectFolderPath => Path.GetFullPath(DTTProjectFolder);

        /// <summary>
        /// The full package name for editor utilities.
        /// </summary>
        public const string FULL_PACKAGE_NAME = "dtt.editorutilities";

        /// <summary>
        /// The editor utilities asset json file container.
        /// </summary>
        public static readonly AssetJson assetInfo;

        /// <summary>
        /// The folder this package is contained inside.
        /// </summary>
        public static string EditorUtilitiesFolder
        {
            get
            {
                if (assetInfo == null)
                    return PackagesFolder;
                else
                    return assetInfo.assetStoreRelease ? ProjectFolder : PackagesFolder;
            }
        }

        /// <summary>
        /// The relative path towards the folder in which the readme sections 
        /// used in the DTT readme are stored.
        /// </summary>
        public const string DTT_README_SECTIONS_FOLDER_RELATIVE = "Editor/Publisher/ReadMeSections";
        #endregion
        #region Private
        /// <summary>
        /// The relative path towards the readme asset.
        /// </summary>
        private const string DTT_README_LOAD_PATH_RELATIVE = "Editor/Publisher/ReadMe/DTTReadMe.asset";

        /// <summary>
        /// The name of the folder in which the editor utilitie package is stored when used by the package manager.
        /// </summary>
        private static string PackagesFolder => Path.Combine("Packages", FULL_PACKAGE_NAME);

        /// <summary>
        /// The name of the folder in which the editor utilities package is stored when used as asset store release.
        /// </summary>
        public static string ProjectFolder => Path.Combine(DTTProjectFolder, PROJECT_FOLDER_NAME);

        /// <summary>
        /// The default folder in which all assets should be found.
        /// </summary>
        private const string DEFAULT_DTT_PROJECT_FOLDER = "Assets/DTT";

        /// <summary>
        /// The suffix used for the key to store the editor preference regarding whether the 
        /// a package its ReadMe has been focused or not.
        /// </summary>
        private const string README_FOCUS_KEY_SUFFIX = "README_FOCUSED";

        /// <summary>
        /// The containers of asset json in the project.
        /// </summary>
        private static List<AssetJson> _assetJsonInProject;

        /// <summary>
        /// The relative path towards the dtt gui skin.
        /// </summary>
        private const string SKIN_PATH_RELATIVE = "Editor/GUI/DTTGUISkin.guiskin";

        /// <summary>
        /// The package display name for editor utilities.
        /// </summary>
        private const string PROJECT_FOLDER_NAME = "Editor Utilities";

        /// <summary>
        /// The folder in the project in which all asset files should be found.
        /// <para>This will be updated to an alternative folder if it can't be found.</para>
        /// </summary>
        private static string _dttProjectFolder = DEFAULT_DTT_PROJECT_FOLDER;

        /// <summary>
        /// A regular expression for testing a documentation web url.
        /// </summary>
        private static readonly Regex _webUrlRegex;
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new static instance, setting up the settings for this package.
        /// </summary>
        static DTTEditorConfig()
        {
            RefreshAssetJsonInProject();

            assetInfo = GetAssetJson(FULL_PACKAGE_NAME);
            _webUrlRegex = new Regex(@"((www.?)|(https:\/\/))[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)");
        }
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Retrieves asset json data about an asset.
        /// </summary>
        /// <param name="fullPackageName"></param>
        /// <returns>The asset json data.</returns>
        public static AssetJson GetAssetJson(string fullPackageName)
        {
            if (fullPackageName == null)
                throw new NullReferenceException("Full package name is null.");

            string packagePath = Path.GetFullPath(Path.Combine("Packages", fullPackageName, "asset.json"));
            if (File.Exists(packagePath))
                return GetAssetJsonInPackages(packagePath);
            else
                return GetAssetJsonInProject(fullPackageName);
        }

        /// <summary>
        /// Opens a ReadMe for package with given package name.
        /// </summary>
        /// <param name="fullPackageName">The full package name.</param>
        public static void OpenReadMe(string fullPackageName)
        {
            AssetJson assetJson = GetAssetJson(fullPackageName);
            if (assetJson != null)
                DTTReadMeEditorWindow.Open(assetJson);
            else
                Debug.LogWarning($"Failed opening ReadMe. No asset json could be found for package {fullPackageName}.");
        }

        /// <summary>
        /// Returns the key to store the editor preference regarding whether the 
        /// package with given display name its ReadMe has been focused.
        /// </summary>
        /// <param name="packageDisplayName">The display name of the package.</param>
        public static string GetReadMeFocusKey(string packageDisplayName)
        {
            if (packageDisplayName == null)
                throw new NullReferenceException("Package display name is null");

            string allcaps = packageDisplayName.FromReadableFormatToAllCaps();
            string focusKey = $"{allcaps}_{README_FOCUS_KEY_SUFFIX}";
            return focusKey;
        }

        /// <summary>
        /// Returns whether the editor configuration can find the readme sections 
        /// folder for package with given asset json.
        /// </summary>
        /// <param name="assetJson">The asset json of the package.</param>
        /// <returns>Whether the editor configuration can find the readme sections folder.</returns>
        public static bool HasReadMeSections(AssetJson assetJson)
            => Directory.Exists(Path.Combine(GetFullContentFolderPath(assetJson), DTT_README_SECTIONS_FOLDER_RELATIVE));

        /// <summary>
        /// Returns the full path towards towards given package its folder.
        /// </summary>
        /// <param name="assetJson">The asset json of the package.</param>
        /// <returns>The full path towards towards given package its folder.</returns>
        public static string GetFullContentFolderPath(AssetJson assetJson)
        {
            string fullPackagePath = Path.GetFullPath(Path.Combine("Packages", assetJson.packageName));
            if (Directory.Exists(fullPackagePath))
                return fullPackagePath;
            else
                return Path.GetFullPath(Path.Combine(DTTProjectFolder, assetJson.displayName));
        }

        /// <summary>
        /// Returns the relative path from the project root towards towards given package its folder.
        /// </summary>
        /// <param name="assetJson">The asset json of the package.</param>
        /// <returns>The relative path from the project root towards towards given package its folder.</returns>
        public static string GetContentFolderPath(AssetJson assetJson)
        {
            string packagePath = Path.Combine("Packages", assetJson.packageName);
            if (Directory.Exists(Path.GetFullPath(packagePath)))
                return packagePath;
            else
                return Path.Combine(DTTProjectFolder, assetJson.displayName);
        }

        /// <summary>
        /// Opens documentation of a package with given full package name.
        /// <para>The documentation url can be a file path relative to the package its folder.</para>
        /// </summary>
        /// <param name="assetJson">The asset json of the package.</param>
        public static void OpenPackageDocumentation(AssetJson assetJson) => OpenPackageLink(assetJson, assetJson.documentationUrl);

        /// <summary>
        /// Opens a link for a package. This can be a url or a path relative to the package folder.
        /// </summary>
        /// <param name="assetJson">The asset json of the package.</param>
        /// <param name="urlOrPath">The url or path.</param>
        public static void OpenPackageLink(AssetJson assetJson, string urlOrPath)
        {
            if (assetJson.packageName == null)
                throw new NullReferenceException("Full package name is null, make sure " +
                    "it can be retrieved correctly from the DTTHeader.");

            if (_webUrlRegex.IsMatch(urlOrPath))
            {
                // Use the url directly if it is a web url.
                Application.OpenURL(urlOrPath);
            }
            else
            {
                // We assume it is a path if the url isn't a web url.
                string path = Path.Combine(GetFullContentFolderPath(assetJson), urlOrPath);
                if (File.Exists(path))
                {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                    OpenDocumentationInOSXOrLinux(path);
#else
                    Application.OpenURL(path);
#endif
                }
                else
                {
                    Debug.LogWarning($"Path {path} couldn't be found. If this is a web url, make sure it starts with 'www.'"
                       + "If it is a path, use only the name of you file that is stored in the root folder.");
                }
            }
        }

#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
        /// <summary>
        /// Opens the documentation file locally for OSX or Linux devices.
        /// </summary>
        /// <param name="path">The path towards the documentation.</param>
        private static void OpenDocumentationInOSXOrLinux(string path)
        {
            // On OSX and Linux we will open the directory the file is in because
            // permissions are necessary to open files.
            FileAttributes attributes = File.GetAttributes(path);
            if (!attributes.HasFlag(FileAttributes.Directory))
                path = Path.GetDirectoryName(path);

            // Add quotes to account for spaces in the path.
            if (!path.StartsWith("\""))
                path = "\"" + path;

            if (!path.EndsWith("\""))
                path = path + "\"";

            System.Diagnostics.Process.Start("open", path);
        }
#endif

        /// <summary>
        /// Refreshes the cache of project asset info.
        /// </summary>
        public static void RefreshAssetJsonInProject()
        {
            if (_assetJsonInProject == null)
                _assetJsonInProject = new List<AssetJson>();

            _assetJsonInProject.Clear();

            // If the project folder doesn't exist, no refresh should be done.
            if (!Directory.Exists(DTTProjectFolder))
                return;

            // Add asset.json files found in the DTT project folder to the containers.
            foreach (string file in Directory.EnumerateFiles(DTTProjectFolder, "*.json", SearchOption.AllDirectories))
            {
                if (Path.GetFileNameWithoutExtension(file) == "asset")
                {
                    AssetJson container = new AssetJson();
                    string json = File.ReadAllText(file);
                    JsonUtility.FromJsonOverwrite(json, container);

                    _assetJsonInProject.Add(container);
                }
            }
        }
        #endregion
        #region Private 
        /// <summary>
        /// Retrieves asset json from the project (In the <see cref="DTTProjectFolder"/>).
        /// </summary>
        /// <param name="fullPackageName">The full package name of the package.</param>
        /// <returns>The asset json data.</returns>
        private static AssetJson GetAssetJsonInProject(string fullPackageName)
        {
            foreach (AssetJson assetJson in _assetJsonInProject)
                if (assetJson.packageName == fullPackageName)
                    return assetJson;

            Debug.LogWarning("Failed retrieving asset json in project.");
            return null;
        }

        /// <summary>
        /// Retrieves asset json from the package folder.
        /// </summary>
        /// <param name="fullPackageName">The full package name of the package.</param>
        /// <returns>The asset json data.</returns>
        private static AssetJson GetAssetJsonInPackages(string jsonPath)
        {
            if (File.Exists(jsonPath))
            {
                AssetJson assetJson = new AssetJson();
                string json = File.ReadAllText(jsonPath);
                JsonUtility.FromJsonOverwrite(json, assetJson);
                return assetJson;
            }
            else
            {
                Debug.LogWarning($"Failed retrieving asset json at {jsonPath}");
                return null;
            }
        }
        #endregion
        #endregion
    }
}

#endif