using UnityEngine;

namespace Editor
{
	public class MenuItems
	{
		[UnityEditor.MenuItem("EXPERIMENT/Show Persistent Data Path")]
		public static void ShowPersistentDataPath()
		{
			var path = Application.persistentDataPath;
			var uri = new System.Uri(path);
			Application.OpenURL(uri.AbsoluteUri);
		}
	}
}