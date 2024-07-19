using System;
using UnityEngine;
using UnityEngine.UI;

public class UIReplay : MonoBehaviour
{
    public event Action Next;
    public event Action Previous;
    
    [SerializeField] Button _nextButton;
    [SerializeField] Button _previousButton;
    
    void Awake()
    {
        _nextButton.onClick.AddListener(() => Next?.Invoke());
        _previousButton.onClick.AddListener(() => Previous?.Invoke());
    }
}