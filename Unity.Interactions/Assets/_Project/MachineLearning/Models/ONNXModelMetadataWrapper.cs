using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Sentis;
using UnityEngine;

namespace Tactive.MachineLearning.Models
{
	public class OnnxModelMetadataWrapper : ScriptableObject
	{
		public ONNXModelMetadata Metadata => metadata;
		[field: SerializeReference] public List<string> FeatureNames { get; private set; }

		public void Initialize(ONNXModelMetadata metadata)
		{
			this.metadata = metadata;

			if (metadata.MetadataProps.TryGetValue("features", out var featureNames))
				FeatureNames = JsonConvert.DeserializeObject<List<string>>(featureNames);
		}

		List<string> _featureNames;
		[SerializeField] ONNXModelMetadata metadata;
	}
}