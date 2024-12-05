using System.Collections.Generic;
using System.Linq;
using _Project.Interactions.Scripts.UI;
using Interactions.Scripts.UI;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Interactions.Scripts.Editor
{
	[InitializeOnLoad, Overlay(typeof(SceneView), "ID_UISelectionOverlay", "UI Selection", defaultDisplay = true)]
	internal class UIOverlay : IMGUIOverlay, ITransientOverlay, IPreprocessBuildWithReport
	{
		static UIOverlay()
		{
			EditorApplication.playModeStateChanged += EnsureAllUIsActiveBeforePlay;
			EditorApplication.delayCall += UpdateUIElements;
		}

		static List<UIScreen> _uiScreens = new();

		public int callbackOrder { get; }

		public void OnPreprocessBuild(BuildReport report)
		{
			EnsureUIValidated();
		}

		public bool visible => _uiScreens.Count > 0;

		public override void OnGUI()
		{
			if (_uiScreens.Count == 0)
			{
				GUILayout.Label("No UI elements found.");
				return;
			}

			foreach (var ui in _uiScreens)
			{
				if (!ui)
					continue;

				if (GUILayout.Button(ui.name, GUILayout.Width(200)))
					ToggleActiveState(ui);
			}
		}

		static void EnsureAllUIsActiveBeforePlay(PlayModeStateChange state)
		{
			var states = new List<PlayModeStateChange>
			{
				PlayModeStateChange.EnteredEditMode,
				PlayModeStateChange.ExitingEditMode
			};

			if (!states.Contains(state))
				return;

			EnsureUIValidated();
		}

		static void EnsureUIValidated()
		{
			UpdateUIElements();

			foreach (var ui in _uiScreens)
			{
				if (!ui)
					continue;
				ui.Show();
			}
		}

		void ToggleActiveState(UIScreen selectedUI)
		{
			foreach (var ui in _uiScreens)
				if (ui == selectedUI)
				{
					ui.Show();
					EditorGUIUtility.PingObject(ui);
				}
				else
					ui.Hide();
		}

		[InitializeOnLoadMethod]
		static void UpdateUIElements()
		{
			_uiScreens = SceneManager.GetActiveScene()
				.GetRootGameObjects()
				.SelectMany(ui => ui.GetComponentsInChildren<UIScreen>(true))
				.ToList();
		}
	}
}