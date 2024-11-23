using Unity.Sentis;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Tactive.MachineLearning.Models
{
	public class MetadataImporter : IONNXMetadataImportCallbackReceiver
	{
		public void OnMetadataImported(AssetImportContext ctx, ONNXModelMetadata metadata)
		{
			var mainObjectPath = ctx.assetPath;
			var stem = System.IO.Path.GetFileNameWithoutExtension(mainObjectPath);

			void DelayedOperation()
			{
				EditorApplication.delayCall -= DelayedOperation;

				var assetThatWillBecomeChild = AssetDatabase.LoadAssetAtPath<ModelAsset>(mainObjectPath);
				if (assetThatWillBecomeChild != null)
				{
					var directory = System.IO.Path.GetDirectoryName(mainObjectPath);
					var parentFileName = stem + "_with_metadata.asset";
					var parentPath = System.IO.Path.Combine(directory, parentFileName);

					var assetThatWillBecomeParent = ScriptableObject.CreateInstance<ModelAssetWithMetadata>();
					assetThatWillBecomeParent.name = stem + "_with_metadata";

					AssetDatabase.CreateAsset(assetThatWillBecomeParent, parentPath);

					// Remove the original asset before adding it as a sub-asset
					AssetDatabase.RemoveObjectFromAsset(assetThatWillBecomeChild);
					AssetDatabase.DeleteAsset(mainObjectPath);

					AssetDatabase.AddObjectToAsset(assetThatWillBecomeChild, assetThatWillBecomeParent);

					assetThatWillBecomeParent.Initialize(metadata, assetThatWillBecomeChild);

					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}
				else
				{
					Debug.LogError($"Failed to load ModelAsset at path: {mainObjectPath}");
				}
			}

			EditorApplication.delayCall += DelayedOperation;
		}
	}
}