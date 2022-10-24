using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LookAt))]
public class LookAtEditor : Editor {
	public override void OnInspectorGUI()
	{
		LookAt lookat = (LookAt)target;
		lookat.target = EditorGUILayout.ObjectField ("当前目标", lookat.target, typeof(Transform), true) as Transform;
		if (GUILayout.Button ("LookAt")) {
			lookat.transform.LookAt (lookat.target);
		}
	}
}
