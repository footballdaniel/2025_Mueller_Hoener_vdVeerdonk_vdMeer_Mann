using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;
using PoseControl = UnityEngine.InputSystem.XR.PoseControl;

#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
using UnityEditor;
#endif


namespace UnityEngine.XR.OpenXR.Features.Interactions
{
#if UNITY_EDITOR
	/// <summary>    
	/// This <see cref="OpenXRInteractionFeature"/> enables the use of HTC Vive Trackers interaction profiles in OpenXR.    /// </summary>#if UNITY_EDITOR    
	[OpenXRFeature(
		UiName = "HTC Vive Tracker Profile",
		BuildTargetGroups = new[] { BuildTargetGroup.Standalone, BuildTargetGroup.WSA },
		Company = "MASSIVE",
		Desc = "Allows for mapping input to the HTC Vive Tracker interaction profile.",
		DocumentationLink = Constants.k_DocumentationManualURL,
		OpenxrExtensionStrings = extensionName,
		Version = "0.0.1",
		Category = FeatureCategory.Interaction,
		FeatureId = featureId)]
#endif
	public class HTCViveTrackerProfile : OpenXRInteractionFeature
	{
		protected override string GetDeviceLayoutName()
		{
			return "HTC Vive Tracker (OpenXR)";
		}

		public const string featureId = "com.massive.openxr.feature.input.htcvivetracker";
		public const string profile = "/interaction_profiles/htc/vive_tracker_htcx";

		/// <summary>    
		/// The interaction profile string used to reference the <a href="https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#:~:text=in%20this%20case.-,VIVE%20Tracker%20interaction%20profile,-Interaction%20profile%20path">HTC Vive Tracker</a>.        /// </summary>        public const string profile = "/interaction_profiles/htc/vive_tracker_htcx";    
		public const string extensionName = "XR_HTCX_vive_tracker_interaction";

		const string kDeviceLocalizedName = "HTC Vive Tracker OpenXR";

		public static class TrackerUserPaths
		{
			public const string leftFoot = "/user/vive_tracker_htcx/role/left_foot";
			public const string rightFoot = "/user/vive_tracker_htcx/role/right_foot";
			public const string leftShoulder = "/user/vive_tracker_htcx/role/left_shoulder";
			public const string rightShoulder = "/user/vive_tracker_htcx/role/right_shoulder";
			public const string leftElbow = "/user/vive_tracker_htcx/role/left_elbow";
			public const string rightElbow = "/user/vive_tracker_htcx/role/right_elbow";
			public const string leftKnee = "/user/vive_tracker_htcx/role/left_knee";
			public const string rightKnee = "/user/vive_tracker_htcx/role/right_knee";
			public const string waist = "/user/vive_tracker_htcx/role/waist";
			public const string chest = "/user/vive_tracker_htcx/role/chest";
			public const string camera = "/user/vive_tracker_htcx/role/camera";
			public const string keyboard = "/user/vive_tracker_htcx/role/keyboard";
		}

		public static class TrackerComponentPaths
		{
			public const string grip = "/input/grip/pose";
		}

		[InputControlLayout(isGenericTypeOfDevice = true, displayName = "XR Tracker")]
		public class XRTracker : TrackedDevice
		{
		}

		/// <summary>    
		/// An Input System device based off the <a href="https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#_htc_vive_controller_profile">HTC Vive Tracker</a>.        /// </summary>        [Preserve]    
		[InputControlLayout(displayName = "HTC Vive Tracker (OpenXR)",
			commonUsages = new[]
			{
				"Left Foot", "Right Foot", "Left Shoulder", "Right Shoulder", "Left Elbow", "Right Elbow", "Left Knee",
				"Right Knee", "Waist", "Chest", "Camera", "Keyboard"
			})]
		public class XRViveTracker : XRTracker
		{
			/// <summary>    
			/// A <see cref="PoseControl"/> that represents information from the <see cref="HTCViveTrackerProfile.grip"/> OpenXR binding.            /// </summary>            [Preserve]    
			[InputControl(offset = 0, aliases = new[] { "device", "gripPose" }, usage = "Device", noisy = true)]
			public PoseControl devicePose { get; private set; }

			/// <summary>    
			/// A [Vector3Control](xref:UnityEngine.InputSystem.Controls.Vector3Control) required for back compatibility with the XRSDK layouts. This is the device position. For the Oculus Touch device, this is both the grip and the pointer position. This value is equivalent to mapping devicePose/position.            /// </summary>            [Preserve]    
			[InputControl(offset = 8, alias = "gripPosition", noisy = true)]
			public new Vector3Control devicePosition { get; private set; }

			/// <summary>    
			/// A [QuaternionControl](xref:UnityEngine.InputSystem.Controls.QuaternionControl) required for backwards compatibility with the XRSDK layouts. This is the device orientation. For the Oculus Touch device, this is both the grip and the pointer rotation. This value is equivalent to mapping devicePose/rotation.            /// </summary>            [Preserve]    
			[InputControl(offset = 20, alias = "gripOrientation", noisy = true)]
			public new QuaternionControl deviceRotation { get; private set; }

			[Preserve, InputControl(offset = 60)] public new ButtonControl isTracked { get; private set; }

			[Preserve, InputControl(offset = 64)] public new IntegerControl trackingState { get; private set; }

			protected override void FinishSetup()
			{
				base.FinishSetup();
				devicePose = GetChildControl<PoseControl>("devicePose");
				devicePosition = GetChildControl<Vector3Control>("devicePosition");
				deviceRotation = GetChildControl<QuaternionControl>("deviceRotation");
				isTracked = GetChildControl<ButtonControl>("isTracked");
				trackingState = GetChildControl<IntegerControl>("trackingState");

				var capabilities = description.capabilities;
				var deviceDescriptor = XRDeviceDescriptor.FromJson(capabilities);

				if ((deviceDescriptor.characteristics &
				     (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerLeftFoot) != 0)
					InputSystem.InputSystem.SetDeviceUsage(this, "Left Foot");
				else if ((deviceDescriptor.characteristics &
				          (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerRightFoot) != 0)
					InputSystem.InputSystem.SetDeviceUsage(this, "Right Foot");
				else if ((deviceDescriptor.characteristics &
				          (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerLeftShoulder) != 0)
					InputSystem.InputSystem.SetDeviceUsage(this, "Left Shoulder");
				else if ((deviceDescriptor.characteristics &
				          (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerRightShoulder) != 0)
					InputSystem.InputSystem.SetDeviceUsage(this, "Right Shoulder");
				else if ((deviceDescriptor.characteristics &
				          (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerLeftElbow) != 0)
					InputSystem.InputSystem.SetDeviceUsage(this, "Left Elbow");
				else if ((deviceDescriptor.characteristics &
				          (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerRightElbow) != 0)
					InputSystem.InputSystem.SetDeviceUsage(this, "Right Elbow");
				else if ((deviceDescriptor.characteristics &
				          (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerLeftKnee) != 0)
					InputSystem.InputSystem.SetDeviceUsage(this, "Left Knee");
				else if ((deviceDescriptor.characteristics &
				          (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerRightKnee) != 0)
					InputSystem.InputSystem.SetDeviceUsage(this, "Right Knee");
				else if ((deviceDescriptor.characteristics &
				          (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerWaist) != 0)
					InputSystem.InputSystem.SetDeviceUsage(this, "Waist");
				else if ((deviceDescriptor.characteristics &
				          (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerChest) != 0)
					InputSystem.InputSystem.SetDeviceUsage(this, "Chest");
				else if ((deviceDescriptor.characteristics &
				          (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerCamera) != 0)
					InputSystem.InputSystem.SetDeviceUsage(this, "Camera");
				else if ((deviceDescriptor.characteristics &
				          (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerKeyboard) != 0)
					InputSystem.InputSystem.SetDeviceUsage(this, "Keyboard");
			}
		}

		protected override void RegisterDeviceLayout()
		{
			InputSystem.InputSystem.RegisterLayout<XRTracker>();

			InputSystem.InputSystem.RegisterLayout(typeof(XRViveTracker),
				matches: new InputDeviceMatcher()
					.WithInterface(XRUtilities.InterfaceMatchAnyVersion)
					.WithProduct(kDeviceLocalizedName));
		}

		protected override void UnregisterDeviceLayout()
		{
			InputSystem.InputSystem.RemoveLayout(nameof(XRViveTracker));
			InputSystem.InputSystem.RemoveLayout(nameof(XRTracker));
		}

		// Summary:    
		//     A set of bit flags describing XR.InputDevice characteristics.        //"Left Foot", "Right Foot", "Left Shoulder", "Right Shoulder", "Left Elbow", "Right Elbow", "Left Knee", "Right Knee", "Waist", "Chest", "Camera", "Keyboard"        [Flags]    
		public enum InputDeviceTrackerCharacteristics : uint
		{
			TrackerLeftFoot = 0x1000u,
			TrackerRightFoot = 0x2000u,
			TrackerLeftShoulder = 0x4000u,
			TrackerRightShoulder = 0x8000u,
			TrackerLeftElbow = 0x10000u,
			TrackerRightElbow = 0x20000u,
			TrackerLeftKnee = 0x40000u,
			TrackerRightKnee = 0x80000u,
			TrackerWaist = 0x100000u,
			TrackerChest = 0x200000u,
			TrackerCamera = 0x400000u,
			TrackerKeyboard = 0x800000u
		}

		protected override void RegisterActionMapsWithRuntime()
		{
			var actionMap = new ActionMapConfig()
			{
				name = "htcvivetracker",
				localizedName = kDeviceLocalizedName,
				desiredInteractionProfile = profile,
				manufacturer = "HTC",
				serialNumber = "",
				deviceInfos = new List<DeviceConfig>()
				{
					new()
					{
						characteristics = InputDeviceCharacteristics.TrackedDevice |
						                  (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerLeftFoot,
						userPath = TrackerUserPaths.leftFoot
					},
					new()
					{
						characteristics = InputDeviceCharacteristics.TrackedDevice |
						                  (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics
							                  .TrackerRightFoot,
						userPath = TrackerUserPaths.rightFoot
					},
					new()
					{
						characteristics = InputDeviceCharacteristics.TrackedDevice |
						                  (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics
							                  .TrackerLeftShoulder,
						userPath = TrackerUserPaths.leftShoulder
					},
					new()
					{
						characteristics = InputDeviceCharacteristics.TrackedDevice |
						                  (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics
							                  .TrackerRightShoulder,
						userPath = TrackerUserPaths.rightShoulder
					},
					new()
					{
						characteristics = InputDeviceCharacteristics.TrackedDevice |
						                  (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics
							                  .TrackerLeftElbow,
						userPath = TrackerUserPaths.leftElbow
					},
					new()
					{
						characteristics = InputDeviceCharacteristics.TrackedDevice |
						                  (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics
							                  .TrackerRightElbow,
						userPath = TrackerUserPaths.rightElbow
					},
					new()
					{
						characteristics = InputDeviceCharacteristics.TrackedDevice |
						                  (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerLeftKnee,
						userPath = TrackerUserPaths.leftKnee
					},
					new()
					{
						characteristics = InputDeviceCharacteristics.TrackedDevice |
						                  (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics
							                  .TrackerRightKnee,
						userPath = TrackerUserPaths.rightKnee
					},
					new()
					{
						characteristics = InputDeviceCharacteristics.TrackedDevice |
						                  (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerWaist,
						userPath = TrackerUserPaths.waist
					},
					new()
					{
						characteristics = InputDeviceCharacteristics.TrackedDevice |
						                  (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerChest,
						userPath = TrackerUserPaths.chest
					},
					new()
					{
						characteristics = InputDeviceCharacteristics.TrackedDevice |
						                  (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerCamera,
						userPath = TrackerUserPaths.camera
					},
					new()
					{
						characteristics = InputDeviceCharacteristics.TrackedDevice |
						                  (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerKeyboard,
						userPath = TrackerUserPaths.keyboard
					}
				},
				actions = new List<ActionConfig>()
				{
					new()
					{
						name = "devicePose",
						localizedName = "Device Pose",
						type = ActionType.Pose,
						usages = new List<string>()
						{
							"Device"
						},
						bindings = new List<ActionBinding>()
						{
							new()
							{
								interactionPath = TrackerComponentPaths.grip,
								interactionProfileName = profile
							}
						}
					}
				}
			};
			AddActionMap(actionMap);
		}

		protected override bool OnInstanceCreate(ulong xrInstance)
		{
			var res = base.OnInstanceCreate(xrInstance);

			if (!OpenXRRuntime.IsExtensionEnabled("XR_HTCX_vive_tracker_interaction"))
				Debug.LogError("HTC Vive Tracker OpenXR extension is not enabled. Please enable it in the OpenXR settings.");

			return res;
		}
	}
}