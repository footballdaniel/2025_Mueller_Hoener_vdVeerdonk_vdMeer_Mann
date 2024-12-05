using Interactions.Scripts.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Interactions.Scripts.UI
{
	public class MainUI : MonoBehaviour
	{
		[FormerlySerializedAs("ExperimentUI")] public ExperimentOverlay ExperimentOverlay;
		public WebcamSelectionUI WebcamSelectionUI;
		public InSituUI InSituUI;
		public XRStatusUI XRStatusUI;
		public ConditionSelectionUI ConditionSelectionUI;
	}
}