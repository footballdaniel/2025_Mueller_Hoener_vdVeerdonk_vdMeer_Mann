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
				var path = Path.Combine(directory!, parentFileName);

				var assetWithMetadata = ScriptableObject.CreateInstance<ModelAssetWithMetadata>();
				assetWithMetadata.name = stem + "_with_metadata";

				AssetDatabase.CreateAsset(assetWithMetadata, path);
				
				// copy the asset, and add it as a child to the new assetWithMetadata
				
				
				assetWithMetadata.Initialize(metadata, asset);

				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

			EditorApplication.delayCall += DelayedOperation;
		}
	}
}