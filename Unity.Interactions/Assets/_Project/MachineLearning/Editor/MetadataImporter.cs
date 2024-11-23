using System.IO;
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
			var assetPath = ctx.assetPath;
			var stem = Path.GetFileNameWithoutExtension(assetPath);

			void DelayedOperation()
			{
				EditorApplication.delayCall -= DelayedOperation;

				var asset = AssetDatabase.LoadAssetAtPath<ModelAsset>(assetPath);
				var directory = Path.GetDirectoryName(assetPath);
				var parentFileName = stem + "_with_metadata.asset";
				var parentPath = Path.Combine(directory!, parentFileName);

				var assetWithMetadata = ScriptableObject.CreateInstance<ModelAssetWithMetadata>();
				assetWithMetadata.name = stem + "_with_metadata";
				assetWithMetadata.Initialize(metadata, asset);

				AssetDatabase.CreateAsset(assetWithMetadata, parentPath);
				AssetDatabase.RemoveObjectFromAsset(asset);
				AssetDatabase.DeleteAsset(assetPath);
				AssetDatabase.AddObjectToAsset(asset, assetWithMetadata);


				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

			EditorApplication.delayCall += DelayedOperation;
		}
	}
}