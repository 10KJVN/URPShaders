#if UNITY_EDITOR
using Custom_Overrides;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CustomEditor(typeof(CustomOverride2))]
public class CustomVolumeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tintColor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("intensity"));
        serializedObject.ApplyModifiedProperties();
    }
}
#endif