using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
	public class MissingScriptFinder : EditorWindow
	{
		[MenuItem("Tools/Find Missing Scripts in Scene")]
		public static void ShowWindow()
		{
			EditorWindow.GetWindow(typeof(MissingScriptFinder));
		}

		void OnGUI()
		{
			if (GUILayout.Button("Find Missing Scripts"))
			{
				FindMissingScripts();
			}
		}

		private void FindMissingScripts()
		{
			GameObject[] goArray = SceneManager.GetActiveScene().GetRootGameObjects();
			int goCount = 0, componentsCount = 0, missingCount = 0;
        
			foreach (GameObject g in goArray)
			{
				Component[] components = g.GetComponentsInChildren<Component>(true);
				for (int i = 0; i < components.Length; i++)
				{
					goCount++;
					if (components[i] == null)
					{
						missingCount++;
						string s = g.name;
						Transform t = g.transform;
						while (t.parent != null) 
						{
							s = t.parent.name + "/" + s;
							t = t.parent;
						}
						Debug.Log(s + " has a missing script attached in " + g.name, g);
					}
					else
					{
						componentsCount++;
					}
				}
			}

			Debug.Log($"Searched {goCount} GameObjects, {componentsCount} components, found {missingCount} missing scripts.");
		}
	}
}