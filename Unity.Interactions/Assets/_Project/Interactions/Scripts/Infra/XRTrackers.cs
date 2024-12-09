using UnityEngine;

namespace Interactions.Infra
{
	public class XRTrackers : MonoBehaviour
	{
		[field: SerializeReference] public XRTracker HeadTracker { get; set; }
		[field: SerializeReference] public XRTracker DominantFootTracker { get; set; }
		[field: SerializeReference] public XRTracker NonDominantFootTracker { get; set; }
		[field: SerializeReference] public XRTracker DefenderHipsTracker { get; set; }
	}
}