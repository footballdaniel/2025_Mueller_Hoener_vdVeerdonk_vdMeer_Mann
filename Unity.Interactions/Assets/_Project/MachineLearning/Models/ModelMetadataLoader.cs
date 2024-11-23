using Unity.Sentis;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Tactive.MachineLearning.Models
{
	public class ModelMetadataLoader : ScriptableObject, IONNXMetadataImportCallbackReceiver
	{
		public void OnMetadataImported(AssetImportContext ctx, ONNXModelMetadata metadata)
		{
			var metadataWrapper = CreateInstance<ONNXModelMetadataWrapper>();
			metadataWrapper.Initialize(metadata);

			ctx.AddObjectToAsset("Metadata", metadataWrapper);

			ctx.SetMainObject(metadataWrapper);
		}
	}

	/// <summary>
	/// Wrapper to hold the ONNXModelMetadata as a ScriptableObject for Unity's asset database.
	/// </summary>
	public class ONNXModelMetadataWrapper : ScriptableObject
	{

		public ONNXModelMetadata Metadata => metadata;

		// Initialize the wrapper with metadata
		public void Initialize(ONNXModelMetadata metadata)
		{
			this.metadata = metadata;
		}

		[SerializeField] ONNXModelMetadata metadata;
	}


}