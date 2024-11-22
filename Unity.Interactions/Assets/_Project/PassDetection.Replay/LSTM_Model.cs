using System;
using System.Collections.Generic;
using System.IO;
using _Project.Interactions.Scripts.Domain;
using _Project.PassDetection.Replay.Features;
using Src.Domain.Inferences;
using Unity.Sentis;

namespace _Project.PassDetection.Replay
{
	public class LstmModel : IDisposable
	{
		public LstmModel(ModelAsset asset)
		{
			var model = ModelLoader.Load(asset);
			_worker = new Worker(model, BackendType.GPUCompute);
		}

		public void Dispose()
		{
			_worker?.Dispose();
		}


		public float Evaluate(InputData data)
		{
			var zeroedPositionDominantFootCalculator = new ZeroedPositionDominantFootCalculator();
			var footOffsetCalculator = new FootOffsetCalculator();
			var velocitiesDominantFootCalculator = new VelocitiesDominantFootCalculator();
			var velocitiesNonDominantFootCalculator = new VelocitiesNonDominantFootCalculator();

			var features = new List<Feature>();
			features.AddRange(zeroedPositionDominantFootCalculator.Calculate(data));
			features.AddRange(footOffsetCalculator.Calculate(data));
			features.AddRange(velocitiesDominantFootCalculator.Calculate(data));
			features.AddRange(velocitiesNonDominantFootCalculator.Calculate(data));

			var timeseriesCount = features[0].Values.Count;
			var featureCount = features.Count;
			var batchSize = 1;

			var shape = new TensorShape(batchSize, timeseriesCount, featureCount);
			var flattenedValues = new float[timeseriesCount * featureCount];

			var index = 0;

			foreach (var feature in features)
			foreach (var value in feature.Values)
				flattenedValues[index++] = value;


			var desiredTimestamp = 0.1;

			if (Math.Abs(data.Timestamps[0] - desiredTimestamp) < 0.001)
			{
				var flattenedValuesStrings = Array.ConvertAll(flattenedValues, value => value.ToString("F3"));
				var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
				File.WriteAllLines(Path.Combine(desktopPath, "unity_tensor.txt") , flattenedValuesStrings);
			}

			var input = new Tensor<float>(shape, flattenedValues);

			_worker.Schedule(input);
			var outputTensor = _worker.PeekOutput() as Tensor<float>;
			var cpuOutputTensor = outputTensor.ReadbackAndClone();

			var result = cpuOutputTensor[0];
			cpuOutputTensor.Dispose();
			input.Dispose();

			return result;
		}

		readonly ModelAsset _asset;
		readonly Worker _worker;

		Trial _trial;
	}
}