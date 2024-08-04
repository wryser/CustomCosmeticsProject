using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

[CustomEditor(typeof(CosmeticBehaviour))]
//https://blog.theknightsofunity.com/use-customeditor-customize-script-inspector/

public class BehaviourCustomWindow : Editor
{
    private SerializedProperty button;
    private SerializedProperty objectsToToggle;
    private void OnEnable()
    {
        button = serializedObject.FindProperty("button");
        objectsToToggle = serializedObject.FindProperty("objectsToToggle");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(button, new GUIContent("Button"));
        EditorGUILayout.PropertyField(objectsToToggle, new GUIContent("Objects To Toggle Active"));

        //EditorGUI.
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
    }
}
