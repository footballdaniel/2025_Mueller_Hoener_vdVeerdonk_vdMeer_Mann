using Unity.Sentis;
using UnityEngine;

namespace Tactive.MachineLearning.Models
{
	public static class ModelLoaderWithMetadata
	{
		public static Model Load(ModelAssetWithMetadata asset)
		{
			var resource = Resources.Load<ModelAsset>(asset.ModelAsset.name);
			return ModelLoader.Load(resource);
		}
	}
}