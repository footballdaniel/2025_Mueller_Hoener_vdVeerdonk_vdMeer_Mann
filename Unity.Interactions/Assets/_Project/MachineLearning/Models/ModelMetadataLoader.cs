using Tactive.MachineLearning.Models;
using Unity.Sentis;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Tactive.MachineLearning._Project.MachineLearning
{
	public class ModelMetadataLoader : IONNXMetadataImportCallbackReceiver
	{
		public void OnMetadataImported(AssetImportContext ctx, ONNXModelMetadata metadata)
		{
			var metadataWrapper = ScriptableObject.CreateInstance<OnnxModelMetadataWrapper>();
			metadataWrapper.name = "ONNX Metadata";
			metadataWrapper.Initialize(metadata);
			ctx.AddObjectToAsset("Metadata", metadataWrapper);
			
			// make this the main object, context should become child
			ctx.SetMainObject(metadataWrapper);
			
			Debug.Log($"ONNX Metadata imported: {metadata.DocString}");
		}
	}

}