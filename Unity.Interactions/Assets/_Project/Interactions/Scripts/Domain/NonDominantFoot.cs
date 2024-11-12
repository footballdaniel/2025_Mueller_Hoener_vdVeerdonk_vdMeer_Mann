using UnityEngine;

namespace _Project.Interactions.Scripts.Domain
{
    public class NonDominantFoot : MonoBehaviour
    {
        [SerializeField] XRTracker _nonDominantFootTracker;

        void Start()
        {
            transform.parent = _nonDominantFootTracker.gameObject.transform;
        }
    }
}
