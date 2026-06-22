using System;
using UnityEditor;

namespace _Project.MachineLearning.Editor
{
    /// <summary>
    /// Forces a reimport of every ONNX model once per editor session (i.e. when Unity starts).
    /// Reimporting re-runs the Inference Engine ONNX importer, which fires
    /// <see cref="MetadataImporter"/>'s metadata callback and lets the metadata postprocessor
    /// regenerate the *_with_metadata.asset from the model's embedded metadata. This keeps the
    /// FeatureNames/InputShape/SampleInput in sync without a manual right-click -> Reimport.
    /// </summary>
    [InitializeOnLoad]
    public static class OnnxModelAutoReimporter
    {
        const string SessionKey = "OnnxModelAutoReimporter.HasRun";

        static OnnxModelAutoReimporter()
        {
            // [InitializeOnLoad] runs on every domain reload (each recompile / play-mode enter).
            // SessionState persists across domain reloads but resets when the editor restarts,
            // so this guard makes the reimport happen once per editor launch.
            if (SessionState.GetBool(SessionKey, false))
                return;

            SessionState.SetBool(SessionKey, true);
            EditorApplication.delayCall += ReimportAllOnnxModels;
        }

        static void ReimportAllOnnxModels()
        {
            foreach (var path in AssetDatabase.GetAllAssetPaths())
                if (path.EndsWith(".onnx", StringComparison.OrdinalIgnoreCase))
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
}
