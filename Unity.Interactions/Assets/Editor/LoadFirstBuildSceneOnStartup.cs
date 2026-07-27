using System;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor
{
	/// <summary>
	/// Opens the first enabled scene of the build scene list once per editor session,
	/// so the editor always starts on the project's main scene.
	/// </summary>
	[InitializeOnLoad]
	public static class LoadFirstBuildSceneOnStartup
	{
		private const string HasRunKey = "LoadFirstBuildSceneOnStartup.HasRun";

		static LoadFirstBuildSceneOnStartup()
		{
			// SessionState survives domain reloads but resets on editor restart,
			// so this runs on editor launch only - not after every recompile.
			if (SessionState.GetBool(HasRunKey, false)) return;
			SessionState.SetBool(HasRunKey, true);

			EditorApplication.delayCall += OpenFirstBuildScene;
		}

		private static void OpenFirstBuildScene()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode) return;

			var first = Array.Find(EditorBuildSettings.scenes, scene => scene.enabled);
			if (first == null || string.IsNullOrEmpty(first.path)) return;
			if (EditorSceneManager.GetActiveScene().path == first.path) return;

			if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
			EditorSceneManager.OpenScene(first.path, OpenSceneMode.Single);
		}
	}
}
