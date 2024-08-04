using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;

[CustomEditor(typeof(BadgeDescriptor))]
//https://blog.theknightsofunity.com/use-customeditor-customize-script-inspector/

public class BadgeCustomWindow : Editor
{
    public SerializedProperty Name;
    public SerializedProperty Author;
    public SerializedProperty Description;
    public SerializedProperty id;
    private void OnEnable()
    {
        Name = serializedObject.FindProperty("Name");
        Author = serializedObject.FindProperty("Author");
        Description = serializedObject.FindProperty("Description");
        id = serializedObject.FindProperty("id");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(Name, new GUIContent("Badge Name"));
        EditorGUILayout.PropertyField(Author, new GUIContent("Badge Author"));
        EditorGUILayout.PropertyField(Description, new GUIContent("Badge Description"));

        //EditorGUI.
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
    }
}
