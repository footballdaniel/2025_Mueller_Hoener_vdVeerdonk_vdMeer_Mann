using UnityEngine;
using UnityEngine.Serialization;

namespace Interactions.UI
{
	public class MainUI : MonoBehaviour
	{
		[FormerlySerializedAs("ExperimentUI")] public ExperimentUI _experimentUI;
		public WebcamSelectionUI WebcamSelectionUI;
		public XRStatusUI XRStatusUI;
		public InSituUI InSituUI;
		public ExperimentSetupUI ExperimentSetupUI;
		public OpponentUI OpponentUI;
	}
}