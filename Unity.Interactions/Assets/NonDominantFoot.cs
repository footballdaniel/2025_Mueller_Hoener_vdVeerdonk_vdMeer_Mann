using UnityEngine;

public class NonDominantFoot : MonoBehaviour
{
    [SerializeField] XRTracker _nonDominantFootTracker;

    void Start()
    {
        transform.parent = _nonDominantFootTracker.gameObject.transform;
    }
}
