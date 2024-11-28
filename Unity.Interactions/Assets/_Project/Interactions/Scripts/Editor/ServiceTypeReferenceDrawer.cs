#if UNITY_EDITOR
using System;
using System.Linq;
using Interactions.Scripts.Application;
using UnityEditor;
using UnityEngine;

namespace _Project.Interactions.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(ServiceTypeReference))]
    public class ServiceTypeReferenceDrawer : PropertyDrawer
    {
        private MonoScript script = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty assemblyQualifiedNameProp = property.FindPropertyRelative("assemblyQualifiedName");

            EditorGUI.BeginProperty(position, label, property);

            // Get the current MonoScript from the assemblyQualifiedName, if possible
            Type currentType = !string.IsNullOrEmpty(assemblyQualifiedNameProp.stringValue) 
                ? Type.GetType(assemblyQualifiedNameProp.stringValue) 
                : null;

            if (currentType != null)
            {
                script = AssetDatabase.FindAssets($"{currentType.Name} t:MonoScript")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<MonoScript>)
                    .FirstOrDefault(ms => ms != null && ms.GetClass() == currentType);
            }
            else
            {
                script = null;
            }

            // Draw the MonoScript ObjectField
            EditorGUI.BeginChangeCheck();
            MonoScript newScript = EditorGUI.ObjectField(position, label, script, typeof(MonoScript), false) as MonoScript;
            if (EditorGUI.EndChangeCheck())
            {
                if (newScript != null)
                {
                    Type newType = newScript.GetClass();

                    if (newType == null)
                    {
                        Debug.LogWarning($"The script {newScript.name} does not contain a valid class.");
                        assemblyQualifiedNameProp.stringValue = null;
                    }
                    else
                    {
                        // Optional: Add validation here (e.g., check for interfaces)
                        assemblyQualifiedNameProp.stringValue = newType.AssemblyQualifiedName;
                    }
                }
                else
                {
                    assemblyQualifiedNameProp.stringValue = null;
                }
            }

            EditorGUI.EndProperty();
        }
    }
}
#endif
