using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PassDetection
{
    public class PassDetectionUI : MonoBehaviour
    {
        [SerializeField] Button _evaluateButton;
        [SerializeField] TMP_Text _resultText;
        [SerializeField] LSTM_Model _lstmModel;
    
        void Start()
        {
            _evaluateButton.onClick.AddListener(OnEvaluateButtonClicked);
        }

        void OnEvaluateButtonClicked()
        {
            var result = _lstmModel.Evaluate();
            _resultText.SetText($"Prediction Probability: {result*100:F1}%");
        }
    }
}
