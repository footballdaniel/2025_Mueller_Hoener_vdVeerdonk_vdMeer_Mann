using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Sentis;
using UnityEngine;

namespace Tactive.MachineLearning.Models
{
	[CreateAssetMenu(fileName = "ModelAssetWithMetadata", menuName = "ScriptableObjects/ModelAssetWithMetadata")]
	public class ModelAssetWithMetadata : ScriptableObject
	{
		[SerializeField] string modelAssetPath;

		[SerializeField] ONNXModelMetadata metaData;

		[SerializeField] List<string> featureNames;

		[SerializeField] List<int> inputShape;

		[SerializeField] List<float> sampleInput;

		[SerializeField] float sampleOutput;
		public string ModelAssetPath => modelAssetPath;
		public ONNXModelMetadata MetaData => metaData;
		public List<string> FeatureNames => featureNames;
		public List<int> InputShape => inputShape;
		public List<float> SampleInput => sampleInput;
		public float SampleOutput => sampleOutput;

		public void Initialize(ONNXModelMetadata metadata, string modelAssetPath)
		{
			this.modelAssetPath = modelAssetPath;
			metaData = metadata;

			if (metadata.MetadataProps.TryGetValue("features", out var featureNamesJson))
				featureNames = JsonConvert.DeserializeObject<List<string>>(featureNamesJson);

			if (metadata.MetadataProps.TryGetValue("input_shape", out var inputShapeJson))
				inputShape = JsonConvert.DeserializeObject<List<int>>(inputShapeJson);

			if (metadata.MetadataProps.TryGetValue("sample_input", out var sampleInputJson))
				sampleInput = JsonConvert.DeserializeObject<List<float>>(sampleInputJson);

			if (metadata.MetadataProps.TryGetValue("sample_output", out var sampleOutputJson))
				sampleOutput = JsonConvert.DeserializeObject<float>(sampleOutputJson);
		}
	}
}