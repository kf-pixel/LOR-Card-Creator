﻿#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace DTT.Utils.EditorUtilities.Publishing
{
    /// <summary>
    /// Enforces the DTT GUI skin and provides the option for drawing
    /// a DTT header at the top of your inspector.
    /// </summary>
    [CanEditMultipleObjects]
    public class DTTInspector : Editor
    {
        #region Variables
        #region Private
        /// <summary>
        /// Whether to display the header or not.
        /// </summary>
        private bool _canDisplay;

        /// <summary>
        /// The header instance used for drawing the DTT header.
        /// </summary>
        private DTTHeader _dttHeader;
        #endregion
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Draws the header if it can be displayed and sets the GUI skin to the 
        /// DTT GUI skin.
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (_canDisplay)
                _dttHeader.OnGUI();
        }
        #endregion
        #region Protected
        /// <summary>
        /// Finds the attributes and creates the header based on found info.
        /// </summary>
        protected virtual void OnEnable() => Initialize();

        /// <summary>
        /// Draws the base header if the DTT header can't be displayed.
        /// <para>
        /// This method isn't called on <see cref="MonoBehaviour"/>
        /// editors but is on <see cref="ScriptableObject"/> editors.
        /// </para>
        /// </summary>
        protected override void OnHeaderGUI()
        {
            if (!_canDisplay)
                base.OnHeaderGUI();
        }
        #endregion
        #region Private
        /// <summary>
        /// Finds the attributes necessary for drawing the header.
        /// </summary>
        private void Initialize()
        {
            // Try retrieving the header attribute.
            DTTHeaderAttribute headerAttribute = GetHeaderAttribute();
            if (headerAttribute != null && headerAttribute.fullPackageName != null)
            {
                AssetJson assetJson = DTTEditorConfig.GetAssetJson(headerAttribute.fullPackageName);
                if (assetJson != null)
                {
                    _canDisplay = true;
                    _dttHeader = new DTTHeader(assetJson);
                }
            }
        }

        /// <summary>
        /// Tries retrieving the <see cref="DTTHeaderAttribute"/>.
        /// <para>Will return null if it isn't found.</para>
        /// </summary>
        /// <returns>The found attribute.</returns>
        private DTTHeaderAttribute GetHeaderAttribute()
        {
            object[] attributes = GetType().GetCustomAttributes(typeof(DTTHeaderAttribute), true);

            return attributes.Length != 0 ? (DTTHeaderAttribute)attributes[0] : null;
        }
        #endregion
        #endregion      
    }
}

#endif