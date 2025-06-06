using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.InitializeOnLoad
{
	[InitializeOnLoad, CreateAssetMenu(fileName = "Load Scene On Startup", menuName = "ScriptableObjects/Load Scene On Startup")]
	public class LoadSceneOnStartup : ScriptableObject
	{
		static LoadSceneOnStartup()
		{
			EditorApplication.delayCall += OnStartup;
		}

		const string LOADED_SCENE_ON_STARTUP = "SceneStartup.loadedScene";
		const string ALWAYS_LOAD_SCENES = "SceneStartup.alwaysLoadScenes";
		
		[SerializeField] SceneAsset _sceneToLoad;
		[SerializeField] List<SceneAsset> _additionalScenesToLoad;
		[SerializeField] bool _alwaysLoadScenes;

		void LoadScenes()
		{
			if (Application.isPlaying)
				return;

			if (SceneManager.GetActiveScene().name != _sceneToLoad.name)
				EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(_sceneToLoad));

			foreach (var scene in _additionalScenesToLoad)
				EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene), OpenSceneMode.Additive);
		}

		static void OnStartup()
		{
			if (SessionState.GetBool(LOADED_SCENE_ON_STARTUP, false) && !SessionState.GetBool(ALWAYS_LOAD_SCENES, false))
				return;

			var assets = AssetDatabase.FindAssets($"t:{nameof(LoadSceneOnStartup)}");

			if (assets.Length == 0)
				return;

			var instance = AssetDatabase.LoadAssetAtPath<LoadSceneOnStartup>(AssetDatabase.GUIDToAssetPath(assets[0]));

			if (instance == null)
				return;

			instance.LoadScenes();

			if (instance._alwaysLoadScenes)
				SessionState.SetBool(ALWAYS_LOAD_SCENES, true);
			
			SessionState.SetBool(LOADED_SCENE_ON_STARTUP, true);
		}
	}
}