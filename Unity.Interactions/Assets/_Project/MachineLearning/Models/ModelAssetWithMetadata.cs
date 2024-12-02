using System.Collections.Generic;
using UnityEngine;

namespace Tactive.MachineLearning.Models
{
	[CreateAssetMenu(fileName = "ModelAssetWithMetadata", menuName = "ScriptableObjects/ModelAssetWithMetadata")]
	public class ModelAssetWithMetadata : ScriptableObject
	{
		[SerializeField] string _modelAssetPath;

		[SerializeField] List<string> _featureNames;

		[SerializeField] List<int> _inputShape;

		[SerializeField] List<float> _sampleInput;

		[SerializeField] float _sampleOutput;
		public string ModelAssetPath => _modelAssetPath;
		public List<string> FeatureNames => _featureNames;
		public List<int> InputShape => _inputShape;
		public List<float> SampleInput => _sampleInput;
		public float SampleOutput => _sampleOutput;

		public void Initialize(List<string> metadata, List<int> inputShape, List<float> sampleInput, float sampleOutput, string modelAssetPath)
		{
			_featureNames = metadata;
			_inputShape = inputShape;
			_sampleInput = sampleInput;
			_sampleOutput = sampleOutput;
			_modelAssetPath = modelAssetPath;
		}
	}
}