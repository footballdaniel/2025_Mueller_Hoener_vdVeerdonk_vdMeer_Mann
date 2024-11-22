using System.Collections.Generic;
using _Project.PassDetection.Replay.Features;
using Unity.Sentis;
using UnityEditor;
using UnityEngine;

public class FeatureTest : MonoBehaviour
{
    [MenuItem("TEST/Feature Test")]
    public static void TestArray()
    {
        // Example: List of Features
        var features = new List<Feature>
        {
            new Feature("feature_1", new List<float> { 0.1f, 0.2f, 0.3f }),
            new Feature("feature_2", new List<float> { 1.1f, 1.2f, 1.3f }),
            new Feature("feature_3", new List<float> { 2.1f, 2.2f, 2.3f })
        };

        // Dimensions
        var featureCount = features.Count; // Rows
        var timeseriesCount = features[0].Values.Count; // Columns

        // Flatten the features into a row-major array
        var flattenedValues = new float[featureCount * timeseriesCount];
        var index = 0;

        foreach (var feature in features)
        {
            foreach (var value in feature.Values)
            {
                flattenedValues[index++] = value;
            }
        }
    
        // Convert to tensor shape
        var batchSize = 1;
        var shape = new TensorShape(batchSize, timeseriesCount, featureCount);
    
        var tensor = new Tensor<float>(shape, flattenedValues);

        // Debug: Print the tensor
        Debug.Log("Tensor:");
        Debug.Log(string.Join(", ", tensor.DownloadToArray()));
    }

}
