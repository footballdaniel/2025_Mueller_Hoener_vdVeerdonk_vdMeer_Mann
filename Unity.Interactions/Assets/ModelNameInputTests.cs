using Unity.Sentis;
using UnityEditor;
using UnityEngine;

public class ModelNameInputTests : MonoBehaviour
{
    [MenuItem("TEST/Features")]
    static void TestLoad()
    {
        var asset = Resources.Load<ModelAsset>("pass_detection_model");
        var model = ModelLoader.Load(asset);
        
        Debug.Log(model.ProducerName);
    }


}
