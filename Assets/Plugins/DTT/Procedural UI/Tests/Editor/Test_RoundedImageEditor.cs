using DTT.UI.ProceduralUI;
using DTT.UI.ProceduralUI.Editor;
using NUnit.Framework;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace DTT.UI.ProceduralUI.Tests
{
	/// <summary>
	/// Tests the functionalities of the <see cref="RoundedImageEditor"/>.
	/// </summary>
	public class Test_RoundedImageEditor
	{
		#region Methods
		/// <summary>
		/// Destroy the canvas if there is one active.
		/// </summary>
		[OneTimeSetUp]
		public void Setup()
		{
			var objects = Object.FindObjectsOfType<Canvas>();
			for (int i = 0; i < objects.Length; i++)
			{
				if (objects[i] != null && objects[i].gameObject != null)
					Object.DestroyImmediate(objects[i].gameObject);
			}
		}

		/// <summary>
		/// Tests the creation of a <see cref="RoundedImage"/> using the method used for the component menu.
		/// </summary>
		[UnityTest]
		public IEnumerator TestCreatingImageFromMenu()
		{
			// First round creates a canvas.
			RoundedImageEditor.RoundedImageMenuItem();
			Assert.IsTrue(Selection.activeGameObject.TryGetComponent(out RoundedImage roundedImage1));
			roundedImage1.GetType().GetField("_selectedUnit", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(roundedImage1, RoundingUnit.WORLD);

			yield return null;

			// Second round creates the component nested inside the top one.
			RoundedImageEditor.RoundedImageMenuItem();
			Assert.IsTrue(Selection.activeGameObject.TryGetComponent(out RoundedImage roundedImage2));

			yield return null;

			// Destroy the objects.
			Object.DestroyImmediate(roundedImage2.gameObject);
			Object.DestroyImmediate(roundedImage1.gameObject);
		}

		[UnityTest]
		public IEnumerator TestInspectPrefabNoCanvasMissingError()
		{
			RoundedImageEditor.RoundedImageMenuItem();
			string name = "Rounded Image name";
			Selection.activeGameObject.name = name;
			PrefabUtility.SaveAsPrefabAsset(Selection.activeGameObject, $"Assets/{name}.prefab", out bool success);
			yield return null;
			RoundedImage prefab = AssetDatabase.LoadAssetAtPath<RoundedImage>($"Assets/{name}.prefab");
			EditorGUIUtility.PingObject(prefab);
			yield return null;
			Assert.IsFalse(prefab.ErrorHandler.AnyErrors);

			AssetDatabase.DeleteAsset($"Assets/{name}.prefab");
		}
		#endregion
	}
}