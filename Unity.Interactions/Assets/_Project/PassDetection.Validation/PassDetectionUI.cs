using PassDetection.Valwidation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.PassDetection.Validation
{
    public class PassDetectionUI : MonoBehaviour
    {
        [SerializeField] Button _evaluateButton;
        [SerializeField] TMP_Text _resultText;
        [SerializeField] PassValidationApp _passValidationApp;
    
        void Start()
        {
            _evaluateButton.onClick.AddListener(OnEvaluateButtonClicked);
        }

        void OnEvaluateButtonClicked()
        {
            var result = _passValidationApp.EvaluateNext();
            _resultText.SetText($"Prediction Probability: {result*100:F1}%");
        }
    }
}
