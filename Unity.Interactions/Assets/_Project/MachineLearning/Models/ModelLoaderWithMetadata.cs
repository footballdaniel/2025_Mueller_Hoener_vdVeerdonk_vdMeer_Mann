using Unity.Sentis;

namespace Tactive.MachineLearning.Models
{
	public static class ModelLoaderWithMetadata
	{
		public static Model Load(ModelAssetWithMetadata asset)
		{
			return ModelLoader.Load(asset.ModelAsset);
		}
	}
}