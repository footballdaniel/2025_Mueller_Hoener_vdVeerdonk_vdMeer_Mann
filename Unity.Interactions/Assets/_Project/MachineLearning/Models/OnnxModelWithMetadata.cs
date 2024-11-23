using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Sentis;
using UnityEngine;

namespace Tactive.MachineLearning.Models
{
	/// <summary>
	/// This is automatically created as a child of a ModelAsset when importing an ONNX
	/// </summary>
	public class OnnxModelWithMetadata : ScriptableObject
	{
		public ModelAsset ModelAsset { get; private set; }
		public ONNXModelMetadata MetaData => _metaData;
		[field: SerializeReference] public List<string> FeatureNames { get; private set; }

		public void Initialize(ONNXModelMetadata metadata, ModelAsset parent)
		{
			_metaData = metadata;

			if (metadata.MetadataProps.TryGetValue("features", out var featureNames))
				FeatureNames = JsonConvert.DeserializeObject<List<string>>(featureNames);

			ModelAsset = parent;
		}

		List<string> _featureNames;
		[SerializeField] ONNXModelMetadata _metaData;
	}
}