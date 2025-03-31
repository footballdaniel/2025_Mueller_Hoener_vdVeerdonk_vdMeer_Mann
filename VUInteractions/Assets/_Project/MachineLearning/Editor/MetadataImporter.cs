using Unity.Sentis;
using UnityEditor.AssetImporters;

namespace _Project.MachineLearning.Editor
{
	public class MetadataImporter : IONNXMetadataImportCallbackReceiver
	{
		public static ONNXModelMetadata Metadata;

		public void OnMetadataImported(AssetImportContext ctx, ONNXModelMetadata metadata)
		{
			Metadata = metadata;
		}
	}
}