using Unity.Sentis;

namespace Tactive.MachineLearning.Models
{
	public record ModelWithMetadata(Model Model, ONNXModelMetadata Metadata);
}