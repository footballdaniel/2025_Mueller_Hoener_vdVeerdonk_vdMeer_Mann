using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

public class XRSessionRestarter : MonoBehaviour
{
	[SerializeField] float _secondsBetweenStopAndStart = 0.5f;

	public bool IsRestarting { get; private set; }

	public void Restart()
	{
		if (IsRestarting)
			return;

		StartCoroutine(RestartRoutine());
	}

	IEnumerator RestartRoutine()
	{
		IsRestarting = true;

		var manager = XRGeneralSettings.Instance != null ? XRGeneralSettings.Instance.Manager : null;
		if (manager == null)
		{
			Debug.LogError("Cannot restart the XR session: no XRManagerSettings available.");
			IsRestarting = false;
			yield break;
		}

		manager.StopSubsystems();
		manager.DeinitializeLoader();

		yield return new WaitForSecondsRealtime(_secondsBetweenStopAndStart);

		yield return manager.InitializeLoader();

		if (manager.activeLoader == null)
		{
			Debug.LogError("XR session restart failed: the loader did not initialise. Is SteamVR still running?");
			IsRestarting = false;
			yield break;
		}

		manager.StartSubsystems();
		IsRestarting = false;
	}
}
