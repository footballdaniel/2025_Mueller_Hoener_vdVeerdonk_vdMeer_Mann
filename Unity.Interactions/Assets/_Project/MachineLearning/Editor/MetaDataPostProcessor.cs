using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using System.IO;
using _Project.MachineLearning.Editor;

namespace Tactive.MachineLearning.Models
{
	public class MetadataPostprocessor : AssetPostprocessor
	{
		static void OnPostprocessAllAssets(
			string[] importedAssets,
			string[] deletedAssets,
			string[] movedAssets,
			string[] movedFromAssetPaths)
		{
			foreach (string assetPath in importedAssets)
			{
				if (Path.GetExtension(assetPath).Equals(".onnx"))
				{
					var stem = Path.GetFileNameWithoutExtension(assetPath);

					var metadata = MetadataImporter.Metadata;

					var metadataPath = Path.Combine(Path.GetDirectoryName(assetPath), stem + "_with_metadata.asset");
					
					var assetWithMetadata = ScriptableObject.CreateInstance<ModelAssetWithMetadata>();
					assetWithMetadata.name = stem + "_with_metadata";
					assetWithMetadata.Initialize(metadata, stem);

					// add as asset
					AssetDatabase.CreateAsset(assetWithMetadata, metadataPath);
					AssetDatabase.SaveAssets();
				}
			}
		}

	}
}