using Interactions.Scripts.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Interactions.Scripts.UI
{
	public class MainUI : MonoBehaviour
	{
		[FormerlySerializedAs("ExperimentUI")] public ExperimentOverlay experimentOverlay;
		public WebcamSelectionUI WebcamSelectionUI;
		public InSituUI InSituUI;
		public XRStatusUI XRStatusUI;
	}
}