using UnityEngine;
using Unity.Sentis;

public class ModelExecutionSingleInput : MonoBehaviour
{
    [SerializeField]
    ModelAsset modelAsset;
    Worker m_Worker;
    Tensor m_Input;

    void Start()
    {
        var model = ModelLoader.Load(modelAsset);
        m_Worker = new Worker(model, BackendType.GPUCompute);

        // Initialize a single input tensor with shape (1, 10, 12)
        m_Input = new Tensor<float>(new TensorShape(1, 10, 12), new float[]
        {
            // All values in a flat array for the shape (1, 10, 12)
            0.0000f,  0.0000f, -0.0000f,  0.46078f, -0.063869f,  0.20658f,  0.0000f,  0.0000f,  0.0000f,  0.0000f,  0.0000f,  0.0000f,
            0.011501f,  0.0038801f, -0.35737f,  0.37971f, -0.064205f, -0.15098f, -0.062511f,  0.035441f,  0.088464f,  0.11501f,  0.038801f, -3.5737f,
            -0.022080f,  0.18695f, -0.53246f,  0.38767f, -0.24986f, -0.34012f,  0.070988f, -0.025914f,  0.097862f, -0.33581f,  1.8307f, -1.7510f,
            0.0000f,  0.0000f,  0.0000f,  0.5000f, -0.0800f,  0.2000f,  0.1000f,  0.0200f,  0.0300f,  0.0400f,  0.0000f,  0.1000f,
            -0.1000f,  0.2000f, -0.3000f,  0.4000f, -0.1500f, -0.1200f,  0.0600f, -0.0500f,  0.0800f, -0.0400f,  1.5000f, -1.2000f,
            0.0150f,  0.0250f, -0.0350f,  0.0450f, -0.0550f, -0.0650f, -0.0750f,  0.0850f,  0.0950f,  0.1050f,  0.1150f, -0.1250f,
            -0.0135f,  0.1455f, -0.2510f,  0.3515f, -0.4515f, -0.5615f,  0.6515f, -0.7215f,  0.8415f, -0.9515f,  1.0515f, -1.1515f,
            0.0200f,  0.0300f, -0.0400f,  0.0600f, -0.0700f, -0.0800f,  0.0900f,  0.1000f,  0.1200f,  0.1300f,  0.1400f, -0.1500f,
            -0.0250f,  0.0350f, -0.0450f,  0.0550f, -0.0650f, -0.0750f,  0.0850f, -0.0950f,  0.1050f, -0.1150f,  1.2050f, -1.3050f,
            0.0000f,  0.0100f, -0.0200f,  0.0300f, -0.0400f, -0.0500f,  0.0600f,  0.0700f,  0.0800f,  0.0900f,  0.1000f, -0.1100f
        });

        // Run inference once with the single input tensor
        m_Worker.Schedule(m_Input);

        // Retrieve output tensor (assuming a single output named "output")
        var outputTensor = m_Worker.PeekOutput("output") as Tensor<float>;

        // Download the output tensor to CPU for reading
        var cpuOutputTensor = outputTensor.ReadbackAndClone();

        // Use the output values, e.g., logging the first few elements
        Debug.Log("Inference Output:");
        for (int i = 0; i < Mathf.Min(cpuOutputTensor.count, 5); i++)
        {
            Debug.Log(cpuOutputTensor[i]);
        }

        // Clean up
        cpuOutputTensor.Dispose();
        m_Input.Dispose();
        m_Worker.Dispose();
    }
}
