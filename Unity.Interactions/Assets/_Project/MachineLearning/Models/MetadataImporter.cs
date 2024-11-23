using Tactive.MachineLearning.Models;
using Unity.Sentis;
using UnityEditor.AssetImporters;
using UnityEngine;

public class MetadataImporter : IONNXMetadataImportCallbackReceiver
{
	public void OnMetadataImported(AssetImportContext ctx, ONNXModelMetadata metadata)
	{
		var metadataWrapper = ScriptableObject.CreateInstance<OnnxModelWithMetadata>();
		metadataWrapper.name = "ONNX Metadata";
		ctx.AddObjectToAsset("Metadata", metadataWrapper);
		var parent = (ModelAsset)ctx.mainObject;
		metadataWrapper.Initialize(metadata, parent);
	}
}