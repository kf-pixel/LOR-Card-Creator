using DTT.UI.ProceduralUI.Exceptions;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using DTT.Utils.EditorUtilities;
using UnityEditor.SceneManagement;
#endif

namespace DTT.UI.ProceduralUI.Tests
{
    /// <summary>
    /// Class that contains tests for <see cref="RoundedImage"/>.
    /// </summary>
    public class Test_RoundedImage
    {
        #region Variables
        /// <summary>
        /// Camera component.
        /// </summary>
        private Camera _camera;

        /// <summary>
        /// RoundedImage component.
        /// </summary>
        private RoundedImage _roundedImage;

        /// <summary>
        /// The field info of the _selectedUnit.
        /// </summary>
        private FieldInfo _selectedUnitFieldInfo;

        /// <summary>
        /// The method info of the ApplyCornerRounding method.
        /// </summary>
        private MethodInfo _applyCornerRoundingMethodInfo;
        #endregion

#if UNITY_EDITOR
        #region Tests
        /// <summary>
        /// Sets up the testing environment by loading a predefined scene and getting the references from it.
        /// </summary>
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            string scenePath;
            if (DTTEditorConfig.GetAssetJson("dtt.proceduralui").assetStoreRelease)
                scenePath = "Assets/DTT/Procedural UI/Tests/Scenes/Test Scene.unity";
            else
                scenePath = "Packages/dtt.proceduralui/Tests/Scenes/Test Scene.unity";
            EditorSceneManager.LoadSceneInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Single));
            yield return new WaitForSeconds(0.1f);
            _camera = Object.FindObjectOfType<Camera>();
            _roundedImage = Object.FindObjectOfType<RoundedImage>();

            // Set rounded image sizing. This is needed for getting correct results from the hitbox testing.
            _roundedImage.rectTransform.pivot = Vector2.one * 0.5f;
            _roundedImage.rectTransform.anchorMin = Vector2.one * 0.5f;
            _roundedImage.rectTransform.anchorMax = Vector2.one * 0.5f;
            _roundedImage.rectTransform.sizeDelta = Vector2.one * 1000;

            // Get field and method info.
            _selectedUnitFieldInfo = _roundedImage.GetType().GetField("_selectedUnit",
                BindingFlags.NonPublic | BindingFlags.Instance);
            _applyCornerRoundingMethodInfo = _roundedImage.GetType().GetMethod("ApplyCornerRounding",
                BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Tests whether exceptions are thrown for out of range values.
        /// Expects the <see cref="ArgumentOutOfRangeException"/> to be thrown with values outside the range of 0 to 1.
        /// </summary>
        [Test]
        public void TestSetRoundingExceptionThrownIncorrectAmount()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _roundedImage.SetCornerRounding(Corner.TOP_RIGHT, 2),
                "No out of range exception thrown.");
            Assert.Throws<ArgumentOutOfRangeException>(() => _roundedImage.SetCornerRounding(Corner.BOTTOM_LEFT, -10),
                "No out of range exception thrown.");
            Assert.Throws<ArgumentOutOfRangeException>(() => _roundedImage.SetCornerRounding(-7, 10, -3, 5),
                "No out of range exception thrown.");
        }

        /// <summary>
        /// Applies rounding to an individual corner and expects the returned value to be equal to the input.
        /// </summary>
        [Test]
        public void TestSetRoundingIndividualActuallyGetsApplied()
        {
            Corner sampleCorner = Corner.TOP_LEFT;
            float testValue = 0.5f;
            _roundedImage.SetCornerRounding(sampleCorner, testValue);

            Assert.AreEqual(testValue, _roundedImage.GetCornerRounding(sampleCorner),
                $"Sample corner {sampleCorner} doesn't reflect set value {testValue}");
        }

        /// <summary>
        /// Applies rounding to an every corner and expects the returned value to be equal to the input.
        /// </summary>
        [Test]
        public void TestSetRoundingActuallyGetsApplied()
        {
            Dictionary<Corner, float> testValues = new Dictionary<Corner, float>
            {
                { Corner.TOP_LEFT, 0.3f },
                { Corner.TOP_RIGHT, 0.7f },
                { Corner.BOTTOM_LEFT, 0.2f },
                { Corner.BOTTOM_RIGHT, 0.5f },
            };
            _roundedImage.SetCornerRounding(testValues[Corner.TOP_LEFT],
                                            testValues[Corner.TOP_RIGHT],
                                            testValues[Corner.BOTTOM_LEFT],
                                            testValues[Corner.BOTTOM_RIGHT]);

            Assert.AreEqual(testValues, _roundedImage.GetCornerRounding(), $"Test values don't reflect actual values.");
        }

        /// <summary>
        /// Applies rounding in the percentage unit and expects the output to be equal to the input.
        /// </summary>
        [Test]
        public void TestApplyCornerRoundingPercentage()
        {
            Corner sampleCorner = Corner.TOP_LEFT;
            float testValue = 0.5f;
            _selectedUnitFieldInfo.SetValue(_roundedImage, RoundingUnit.PERCENTAGE);

            _applyCornerRoundingMethodInfo.Invoke(_roundedImage, new object[] { sampleCorner, testValue });

            Assert.AreEqual(testValue, _roundedImage.GetCornerRounding(sampleCorner),
                $"Sample corner {sampleCorner} doesn't reflect set value {testValue}");
        }

        /// <summary>
        /// Applies rounding in world units and expects the output to be equal to the input.
        /// </summary>
        [Test]
        public void TestApplyCornerRoundingWorldUnit()
        {
            Corner sampleCorner = Corner.TOP_LEFT;
            float testValue = 0.5f;
            _selectedUnitFieldInfo.SetValue(_roundedImage, RoundingUnit.WORLD);

            _applyCornerRoundingMethodInfo.Invoke(_roundedImage, new object[] { sampleCorner, testValue });

            Assert.AreEqual(testValue, _roundedImage.GetCornerRounding(sampleCorner),
                $"Sample corner {sampleCorner} doesn't reflect set value {testValue}");

            _selectedUnitFieldInfo.SetValue(_roundedImage, RoundingUnit.PERCENTAGE);
        }

        /// <summary>
        /// Tests the hit detection by inputting an amount of screen points with expected return values.
        /// Expects the result of <see cref="RoundedImage.IsRaycastLocationValid(Vector2, Camera)"/> to equal that of the input.
        /// </summary>
        [Test]
        public void TestHitDetectionWithSampleLocations()
        {
            KeyValuePair<Vector2, bool>[] screenPointShouldHitPairs = new KeyValuePair<Vector2, bool>[]
            {
				// Corner Samples
				new KeyValuePair<Vector2, bool>(new Vector2(1449.7f, 1556.6f), false),
                new KeyValuePair<Vector2, bool>(new Vector2(2357.1f, 1513.3f), false),
                new KeyValuePair<Vector2, bool>(new Vector2(2293.4f, 733.4f), false),

				// Inside samples
				new KeyValuePair<Vector2, bool>(new Vector2(2163.4f, 1322.1f), false),
                new KeyValuePair<Vector2, bool>(new Vector2(1676.6f, 1324.7f), false),
                new KeyValuePair<Vector2, bool>(new Vector2(1674.0f, 843.0f), false),
                new KeyValuePair<Vector2, bool>(new Vector2(2109.9f, 939.8f), false),

				// Inside on border samples
				new KeyValuePair<Vector2, bool>(new Vector2(1513.5f, 1487.8f), true),
                new KeyValuePair<Vector2, bool>(new Vector2(2270.5f, 1444.5f), true),
                new KeyValuePair<Vector2, bool>(new Vector2(2188.9f, 858.3f), true),
                new KeyValuePair<Vector2, bool>(new Vector2(1577.2f, 730.8f), true),
            };

            _roundedImage.SetCornerRounding(.25f, .5f, .0f, 1.0f);
            _roundedImage.Mode = RoundingMode.BORDER;

            _roundedImage.BorderThickness = 0.5f;
            _roundedImage.UseHitboxOutside = true;
            _roundedImage.UseHitboxInside = true;

            foreach (var pair in screenPointShouldHitPairs)
                Assert.AreEqual(pair.Value, _roundedImage.IsRaycastLocationValid(pair.Key, _camera));
        }

        /// <summary>
        /// Tests whether the sample positions return the expected result 
        /// from the <see cref="Image.IsRaycastLocationValid(Vector2, Camera)"/>.
        /// Expects that the corresponding boolean values are returned. 
        /// </summary>
        [Test]
        public void TestHitDetectionWithNoDynamicsEnabled()
        {
            KeyValuePair<Vector2, bool>[] screenPointShouldHitPairs = new KeyValuePair<Vector2, bool>[]
            {
				// Corner samples.
				new KeyValuePair<Vector2, bool>(new Vector2(1449.7f, 1556.6f), true),
                new KeyValuePair<Vector2, bool>(new Vector2(2357.1f, 1513.3f), true),
                new KeyValuePair<Vector2, bool>(new Vector2(2293.4f, 733.4f), true),

				// Inside samples.
				new KeyValuePair<Vector2, bool>(new Vector2(2163.4f, 1322.1f), true),
                new KeyValuePair<Vector2, bool>(new Vector2(1676.6f, 1324.7f), true),
                new KeyValuePair<Vector2, bool>(new Vector2(1674.0f, 843.0f), true),
                new KeyValuePair<Vector2, bool>(new Vector2(2109.9f, 939.8f), true),

				// Inside on border samples.
				new KeyValuePair<Vector2, bool>(new Vector2(1513.5f, 1487.8f), true),
                new KeyValuePair<Vector2, bool>(new Vector2(2270.5f, 1444.5f), true),
                new KeyValuePair<Vector2, bool>(new Vector2(2188.9f, 858.3f), true),
                new KeyValuePair<Vector2, bool>(new Vector2(1577.2f, 730.8f), true),
            };

            _roundedImage.SetCornerRounding(.25f, .5f, .0f, 1.0f);
            _roundedImage.BorderThickness = 0.5f;
            _roundedImage.UseHitboxOutside = false;
            _roundedImage.UseHitboxInside = false;

            foreach (var pair in screenPointShouldHitPairs)
                Assert.AreEqual(pair.Value, _roundedImage.IsRaycastLocationValid(pair.Key, _camera));
        }

        /// <summary>
        /// Tests whether the sample positions return the expected result 
        /// from the <see cref="Image.IsRaycastLocationValid(Vector2, Camera)"/>.
        /// Expects that the corresponding boolean values are returned. 
        /// </summary>
        [Test]
        public void TestHitDetectionWithPreserveAspect()
        {
            KeyValuePair<Vector2, bool>[] screenPointShouldHitPairs = new KeyValuePair<Vector2, bool>[]
            {
				// Corner Samples
				new KeyValuePair<Vector2, bool>(new Vector2(1449.7f, 1556.6f), false),
                new KeyValuePair<Vector2, bool>(new Vector2(2357.1f, 1513.3f), false),
                new KeyValuePair<Vector2, bool>(new Vector2(2293.4f, 733.4f), false),

				// Inside samples
				new KeyValuePair<Vector2, bool>(new Vector2(2163.4f, 1322.1f), false),
                new KeyValuePair<Vector2, bool>(new Vector2(1676.6f, 1324.7f), false),
                new KeyValuePair<Vector2, bool>(new Vector2(1674.0f, 843.0f), false),
                new KeyValuePair<Vector2, bool>(new Vector2(2109.9f, 939.8f), false),

				// Inside on border samples
				new KeyValuePair<Vector2, bool>(new Vector2(1513.5f, 1487.8f), true),
                new KeyValuePair<Vector2, bool>(new Vector2(2270.5f, 1444.5f), true),
                new KeyValuePair<Vector2, bool>(new Vector2(2188.9f, 858.3f), true),
                new KeyValuePair<Vector2, bool>(new Vector2(1577.2f, 730.8f), true),
            };

            _roundedImage.SetCornerRounding(.25f, .5f, .0f, 1.0f);
            _roundedImage.rectTransform.sizeDelta = new Vector2(2000, 1000);
            _roundedImage.preserveAspect = true;
            _roundedImage.BorderThickness = 0.5f;
            _roundedImage.UseHitboxOutside = true;
            _roundedImage.UseHitboxInside = true;

            foreach (var pair in screenPointShouldHitPairs)
                Assert.AreEqual(pair.Value, _roundedImage.IsRaycastLocationValid(pair.Key, _camera));
        }

        /// <summary>
        /// Tests whether the shaders on the materials are correct.
        /// </summary>
        [Test]
        public void TestMaterialUsesCorrectShader()
        {
            _roundedImage.Mode = RoundingMode.FILL;
            Assert.AreEqual(_roundedImage.material.shader, Shader.Find(RoundedImageAssetManager.ROUNDING_SHADER_NAME),
                "Rounding material shader doesn't match.");

        }

        /// <summary>
        /// Tests the fall off distance.
        /// Expects the fall off to return the same value as the input.
        /// </summary>
        [Test]
        public void TestGetDistanceFalloff()
        {
            float testFalloff = 5;
            _selectedUnitFieldInfo.SetValue(_roundedImage, RoundingUnit.WORLD);
            _roundedImage.DistanceFalloff = testFalloff;
            Assert.AreEqual(testFalloff, _roundedImage.DistanceFalloff);

            _selectedUnitFieldInfo.SetValue(_roundedImage, RoundingUnit.PERCENTAGE);
            _roundedImage.DistanceFalloff = testFalloff;
            Assert.AreEqual(testFalloff, _roundedImage.DistanceFalloff);
        }

        /// <summary>
        /// Tests whether an exception is thrown when using a negative value on the distance fall off.
        /// Expects to throw a <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        [Test]
        public void TestDistanceFalloffOutOfRangeValue()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _roundedImage.DistanceFalloff = -1,
                "Expected out of range exception to be thrown.");
        }

        /// <summary>
        /// Tests whether you're able to reassign the material.
        /// Expects no update to be done.
        /// </summary>
        [Test]
        public void TestCantSetMaterial()
        {
            Material expected = _roundedImage.material;

            _roundedImage.material = null;

            Assert.AreEqual(expected, _roundedImage.material,
                "Expected the material not to be updated but it was.");
        }

        /// <summary>
        /// Tests whether the input is the same as the result of the <see cref="RoundedImage.Mode"/>.
        /// Expects the output to equal the input.
        /// </summary>
        [Test]
        public void TestCorrectModeReturn()
        {
            RoundingMode testMode = RoundingMode.FILL;
            _roundedImage.Mode = testMode;
            Assert.AreEqual(testMode, _roundedImage.Mode);
            testMode = RoundingMode.BORDER;
            _roundedImage.Mode = testMode;
            Assert.AreEqual(testMode, _roundedImage.Mode);
        }

        /// <summary>
        /// Tests the border thickness conversion handling with world units.
        /// Expects the output to equal the input.
        /// </summary>
        [Test]
        public void TestBorderThicknessWithWorldUnits()
        {
            float testValue = 0.5f;
            _selectedUnitFieldInfo.SetValue(_roundedImage, RoundingUnit.WORLD);
            _roundedImage.BorderThickness = testValue;
            Assert.AreEqual(testValue, _roundedImage.BorderThickness);
            _selectedUnitFieldInfo.SetValue(_roundedImage, RoundingUnit.PERCENTAGE);
        }

        /// <summary>
        /// Tests whether the relevant exceptions are thrown when checking for errors.
        /// Expects the relevant exception to be thrown based on the given setup.
        /// </summary>
        [Test]
        public void TestErrorHandling()
        {
            Canvas canvas = _roundedImage.canvas;
            canvas.gameObject.SetActive(false);
            Assert.Throws<CanvasMissingException>(() => _roundedImage.ErrorHandler.CheckForErrors());
            canvas.gameObject.SetActive(true);
            _roundedImage.canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;
            Assert.Throws<TexCoord1MissingException>(() => _roundedImage.ErrorHandler.CheckForErrors());
            _roundedImage.canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
            Assert.Throws<TexCoord2MissingException>(() => _roundedImage.ErrorHandler.CheckForErrors());
            _roundedImage.canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;
            Assert.Throws<TexCoord3MissingException>(() => _roundedImage.ErrorHandler.CheckForErrors());
            _roundedImage.canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord3;
            Assert.DoesNotThrow(() => _roundedImage.ErrorHandler.CheckForErrors());
        }
        #endregion
#endif
    }
}

