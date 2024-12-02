using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using System.IO;
using _Project.MachineLearning.Editor;
using Newtonsoft.Json;

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

					var featureNames = new List<string>();
					if (metadata.MetadataProps.TryGetValue("features", out var featureNamesJson))
						featureNames = JsonConvert.DeserializeObject<List<string>>(featureNamesJson);

					var inputShape = new List<int>();
					if (metadata.MetadataProps.TryGetValue("input_shape", out var inputShapeJson))
						inputShape = JsonConvert.DeserializeObject<List<int>>(inputShapeJson);

					var sampleInput = new List<float>();
					if (metadata.MetadataProps.TryGetValue("sample_input", out var sampleInputJson))
						sampleInput = JsonConvert.DeserializeObject<List<float>>(sampleInputJson);

					var sampleOutput = 0f;
					if (metadata.MetadataProps.TryGetValue("sample_output", out var sampleOutputJson))
						sampleOutput = JsonConvert.DeserializeObject<float>(sampleOutputJson);
					
					
					
					
					assetWithMetadata.Initialize(featureNames, inputShape, sampleInput, sampleOutput, stem);

					// add as asset
					AssetDatabase.CreateAsset(assetWithMetadata, metadataPath);
					AssetDatabase.SaveAssets();
				}
			}
		}

	}
}