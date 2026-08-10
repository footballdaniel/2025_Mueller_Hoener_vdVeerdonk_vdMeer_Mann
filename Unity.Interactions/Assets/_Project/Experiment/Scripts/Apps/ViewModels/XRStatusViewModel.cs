using System.Collections;
using Interactions.Infra;
using UnityEngine;
using UnityEngine.XR.Management;

namespace Interactions.Apps.ViewModels
{
	public class XRStatusViewModel
	{
		public XRStatusViewModel(App app)
		{
			_app = app;
		}

		public string StatusMessage { get; private set; } = "";

		public bool IsBusy { get; private set; }

		public void Startup()
		{
			_app.Transitions.StartExperiment.Execute();
		}

		public bool HasErrors()
		{
			return XRStatusChecker.HasXRErrors();
		}

		public void StartSteamVRAndRetry()
		{
			if (IsBusy)
				return;

			_app.StartCoroutine(StartSteamVRAndRetryRoutine());
		}

		IEnumerator StartSteamVRAndRetryRoutine()
		{
			IsBusy = true;

			if (!SteamVRHeadlessConfig.IsSteamVrRunning())
			{
				StatusMessage = "Starting SteamVR...";
				if (!SteamVRHeadlessConfig.LaunchSteamVr())
				{
					Finish($"Could not start SteamVR. {SteamVRHeadlessConfig.LastError}");
					yield break;
				}

				var waited = 0f;
				while (!SteamVRHeadlessConfig.IsSteamVrRunning() && waited < SteamVrStartupTimeoutSeconds)
				{
					waited += Time.unscaledDeltaTime;
					yield return null;
				}

				if (!SteamVRHeadlessConfig.IsSteamVrRunning())
				{
					Finish("SteamVR did not start. Check that Steam is running and the base stations are powered.");
					yield break;
				}

				yield return new WaitForSecondsRealtime(SecondsToSettleAfterStartup);
			}

			StatusMessage = "Starting XR...";
			_initialisationError = null;
			yield return InitialiseXR();

			if (_initialisationError != null)
			{
				Finish(_initialisationError);
				yield break;
			}

			if (HasErrors())
			{
				Finish("XR still failed to start. Check the console for the OpenXR error.");
				yield break;
			}

			Finish("");
			Startup();
		}

		IEnumerator InitialiseXR()
		{
			var manager = XRGeneralSettings.Instance != null ? XRGeneralSettings.Instance.Manager : null;
			if (manager == null)
			{
				_initialisationError = "No XR manager available. Check XR Plug-in Management in the project settings.";
				yield break;
			}

			if (manager.isInitializationComplete)
			{
				manager.StopSubsystems();
				manager.DeinitializeLoader();
			}

			yield return manager.InitializeLoader();

			if (manager.activeLoader != null)
				manager.StartSubsystems();
		}

		void Finish(string message)
		{
			StatusMessage = message;
			IsBusy = false;
		}

		const float SteamVrStartupTimeoutSeconds = 30f;
		const float SecondsToSettleAfterStartup = 2f;

		string _initialisationError;
		readonly App _app;
	}
}
