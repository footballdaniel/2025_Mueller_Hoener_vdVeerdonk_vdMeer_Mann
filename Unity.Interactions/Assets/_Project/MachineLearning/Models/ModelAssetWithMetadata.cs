using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Sentis;
using UnityEngine;

namespace Tactive.MachineLearning.Models
{
	public class ModelAssetWithMetadata : ScriptableObject
	{
		[field: SerializeReference] public string ModelAssetPath { get; private set; }
		public ONNXModelMetadata MetaData => _metaData;
		[field: SerializeReference] public List<string> FeatureNames { get; private set; }
		[field:SerializeReference] public float[][][] ExampleInput { get; private set; }
		[field:SerializeReference] public float ExampleOutput { get; private set; }
	

		public void Initialize(ONNXModelMetadata metadata, string modelAssetPath)
		{
			ModelAssetPath = modelAssetPath;
			_metaData = metadata;

			if (metadata.MetadataProps.TryGetValue("features", out var featureNames))
				FeatureNames = JsonConvert.DeserializeObject<List<string>>(featureNames);
			
			if (metadata.MetadataProps.TryGetValue("example_input", out var exampleInput))
				ExampleInput = JsonConvert.DeserializeObject<float[][][]>(exampleInput);
			
			if (metadata.MetadataProps.TryGetValue("example_output", out var exampleOutput))
				ExampleOutput = JsonConvert.DeserializeObject<float>(exampleOutput);
		}

		List<string> _featureNames;
		[SerializeField] ONNXModelMetadata _metaData;

	}
}