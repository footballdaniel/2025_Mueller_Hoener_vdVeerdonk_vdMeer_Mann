using UnityEngine;
using UnityEngine.Serialization;

namespace Interactions.UI
{
	public class MainUI : MonoBehaviour
	{
		[FormerlySerializedAs("ExperimentUI")] public ExperimentOverlay ExperimentOverlay;
		public WebcamSelectionUI WebcamSelectionUI;
		public InSituUI InSituUI;
		public XRStatusUI XRStatusUI;
		public ExperimentSetupUI ExperimentSetupUI;
	}
}