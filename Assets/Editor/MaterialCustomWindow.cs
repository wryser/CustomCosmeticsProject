using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;

[CustomEditor(typeof(MaterialDescriptor))]
//https://blog.theknightsofunity.com/use-customeditor-customize-script-inspector/

public class MaterialCustomWindow : Editor
{
    public SerializedProperty Name;
    public SerializedProperty Author;
    public SerializedProperty Description;
    public SerializedProperty id;
    public SerializedProperty customColors;

    private void OnEnable()
    {
        Name = serializedObject.FindProperty("Name");
        Author = serializedObject.FindProperty("Author");
        Description = serializedObject.FindProperty("Description");
        customColors = serializedObject.FindProperty("customColors");
        id = serializedObject.FindProperty("id");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(Name, new GUIContent("Material Name"));
        EditorGUILayout.PropertyField(Author, new GUIContent("Material Author"));
        EditorGUILayout.PropertyField(Description, new GUIContent("Material Description"));
        EditorGUILayout.PropertyField(customColors, new GUIContent("Custom Colours"));

        //EditorGUI.
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
    }
}
