#if UNITY_EDITOR

using System;

namespace DTT.Utils.EditorUtilities.Publishing
{
    /// <summary>
    /// Used for drawing a header for a specific package.
    /// </summary>
    public class DTTHeaderAttribute : Attribute
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of this object, setting the package name.
        /// </summary>
        /// <param name="fullPackageName">The full name of the package (e.g. dtt.editorutilities).</param>
        public DTTHeaderAttribute(string fullPackageName) => this.fullPackageName = fullPackageName;
        #endregion

        #region Variables
        #region Public
        /// <summary>
        /// The full name of the package (e.g. dtt.editorutilities).
        /// </summary>
        public readonly string fullPackageName;
        #endregion
        #endregion
    }
}

#endif
