using System;
using UnityEditor;

namespace _Project.MachineLearning.Editor
{
    [InitializeOnLoad]
    public static class OnnxModelAutoReimporter
    {
        const string SessionKey = "OnnxModelAutoReimporter.HasRun";

        static OnnxModelAutoReimporter()
        {
            if (BuildPipeline.isBuildingPlayer)
                return;

            if (SessionState.GetBool(SessionKey, false))
                return;

            SessionState.SetBool(SessionKey, true);
            EditorApplication.delayCall += ReimportAllOnnxModels;
        }

        static void ReimportAllOnnxModels()
        {
            if (BuildPipeline.isBuildingPlayer)
                return;

            foreach (var path in AssetDatabase.GetAllAssetPaths())
                if (path.EndsWith(".onnx", StringComparison.OrdinalIgnoreCase))
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
}
