using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class MaterialExporter : EditorWindow
{
    private MaterialDescriptor[] descriptorNotes;
    [MenuItem("Custom Cosmetics/Material Exporter")]

    public static void ShowWindow()
    {
        GetWindow(typeof(MaterialExporter), false, "Material Exporter", false);
    }

    public void OnFocus()
    {
        descriptorNotes = FindObjectsOfType<MaterialDescriptor>();
    }

    public Vector2 scrollPosition = Vector2.zero;
    public void OnGUI()
    {
        var window = GetWindow(typeof(MaterialExporter), false, "Material Exporter", false);

        int ScrollSpace = (16 + 20) + (16 + 17 + 17 + 20 + 20);
        foreach (var note in descriptorNotes)
        {
            if (note != null)
            {
                ScrollSpace += (16 + 17 + 17 + 20 + 20);
            }
        }

        float currentWindowWidth = EditorGUIUtility.currentViewWidth;
        float windowWidthIncludingScrollbar = currentWindowWidth;
        if (window.position.size.y >= ScrollSpace)
        {
            windowWidthIncludingScrollbar += 30;
        }

        scrollPosition = GUI.BeginScrollView(new Rect(0, 0, EditorGUIUtility.currentViewWidth, window.position.size.y), scrollPosition, new Rect(0, 0, EditorGUIUtility.currentViewWidth - 20, ScrollSpace), false, false);

        foreach (MaterialDescriptor descriptorNote in descriptorNotes)
        {
            if (descriptorNote != null)
            {
                GUILayout.Label(descriptorNote.gameObject.name, EditorStyles.boldLabel, GUILayout.Height(16));
                descriptorNote.Name = EditorGUILayout.TextField("Name:", descriptorNote.Name, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(17));
                descriptorNote.Author = EditorGUILayout.TextField("Author:", descriptorNote.Author, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(17));
                descriptorNote.Description = EditorGUILayout.TextField("Description:", descriptorNote.Description, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(17));

                if (GUILayout.Button("Export " + descriptorNote.Name, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(20)))
                {
                    GameObject noteObject = descriptorNote.gameObject;
                    if (noteObject != null && descriptorNote != null)
                    {
                        if (descriptorNote.Name == "" || descriptorNote.Author == "" || descriptorNote.Description == "")
                        {
                            EditorUtility.DisplayDialog("Export Failed", "It is required to fill in the Name, Author, and Description for your Material.", "OK");
                            return;
                        }

                        string path = EditorUtility.SaveFilePanel("Where will you build your material?", "", descriptorNote.Name + ".material", "material");

                        if (path != "")
                        {
                            Debug.ClearDeveloperConsole();
                            Debug.Log("Exporting Material");
                            EditorUtility.SetDirty(descriptorNote);
                            BuildAssetBundle(descriptorNote.gameObject, path);
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Export Failed", "Please include the path to where the Material will be exported at.", "OK");
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Export Failed", "The Material object couldn't be found.", "OK");
                    }
                }
                GUILayout.Space(20);
            }
        }
        GUI.EndScrollView();
    }
    static public void BuildAssetBundle(GameObject obj, string path)
    {
        GameObject selectedObject = obj;
        string assetBundleDirectoryTEMP = "Assets/ExportedMaterials";

        MaterialDescriptor descriptor = selectedObject.GetComponent<MaterialDescriptor>();

        if (!AssetDatabase.IsValidFolder("Assets/ExportedMaterials"))
        {
            AssetDatabase.CreateFolder("Assets", "ExportedMaterials");
        }

        string MaterialName = descriptor.Name;

        string prefabPathTEMP = "Assets/ExportedMaterials/Material.prefab";

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        PrefabUtility.SaveAsPrefabAsset(selectedObject.gameObject, prefabPathTEMP);
        GameObject contentsRoot = PrefabUtility.LoadPrefabContents(prefabPathTEMP);
        contentsRoot.name = "material";

        if (File.Exists(prefabPathTEMP))
        {
            File.Delete(prefabPathTEMP);
        }

        string newprefabPath = "Assets/ExportedMaterials/" + contentsRoot.name + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(contentsRoot, newprefabPath);
        PrefabUtility.UnloadPrefabContents(contentsRoot);
        AssetImporter.GetAtPath(newprefabPath).SetAssetBundleNameAndVariant("MaterialAssetBundle", "");

        if (!Directory.Exists("Assets/ExportedMaterials"))
        {
            Directory.CreateDirectory(assetBundleDirectoryTEMP);
        }

        string asset_new = assetBundleDirectoryTEMP + "/" + MaterialName;
        if (File.Exists(asset_new + ".material"))
        {
            File.Delete(asset_new + ".material");
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectoryTEMP, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        if (File.Exists(newprefabPath))
        {
            File.Delete(newprefabPath);
        }

        string asset_temporary = assetBundleDirectoryTEMP + "/MaterialAssetBundle";
        string metafile = asset_temporary + ".meta";
        if (File.Exists(asset_temporary))
        {
            File.Move(asset_temporary, asset_new + ".material");
        }

        AssetDatabase.Refresh();
        Debug.ClearDeveloperConsole();

        string path1 = assetBundleDirectoryTEMP + "/" + MaterialName + ".material";
        string path2 = path;

        if (!File.Exists(path2)) // add
        {
            File.Move(path1, path2);
        }
        else // replace
        {
            File.Delete(path2);
            File.Move(path1, path2);
        }
        EditorUtility.DisplayDialog("Export Success", $"Your Material was exported!", "OK");

        try
        {
            AssetDatabase.RemoveAssetBundleName("Materialassetbundle", true);
        }
        catch
        {

        }

        string MaterialPath = path + "/";
        EditorUtility.RevealInFinder(MaterialPath);

        if (AssetDatabase.IsValidFolder("Assets/ExportedMaterials"))
        {
            AssetDatabase.DeleteAsset("Assets/ExportedMaterials");
        }
    }
}