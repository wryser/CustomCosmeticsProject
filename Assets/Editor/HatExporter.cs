using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class HatExporter : EditorWindow
{
    private HatDescriptor[] descriptorNotes;
    [MenuItem("Custom Cosmetics/Hat Exporter")]

    public static void ShowWindow()
    {
        GetWindow(typeof(HatExporter), false, "Hat Exporter", false);
    }

    public void OnFocus()
    {
        descriptorNotes = FindObjectsOfType<HatDescriptor>();
    }

    public Vector2 scrollPosition = Vector2.zero;
    public void OnGUI()
    {
        var window = GetWindow(typeof(HatExporter), false, "Hat Exporter", false);

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

        foreach (HatDescriptor descriptorNote in descriptorNotes)
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
                            EditorUtility.DisplayDialog("Export Failed", "It is required to fill in the Name, Author, and Description for your hat.", "OK");
                            return;
                        }

                        string path = EditorUtility.SaveFilePanel("Where will you build your hat?", "", descriptorNote.Name + ".hat", "hat");

                        if (path != "")
                        {
                            Debug.ClearDeveloperConsole();
                            Debug.Log("Exporting hat");
                            EditorUtility.SetDirty(descriptorNote);
                            BuildAssetBundle(descriptorNote.gameObject, path);
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Export Failed", "Please include the path to where the hat will be exported at.", "OK");
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Export Failed", "The hat object couldn't be found.", "OK");
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
        string assetBundleDirectoryTEMP = "Assets/Exportedhats";

        HatDescriptor descriptor = selectedObject.GetComponent<HatDescriptor>();

        if (!AssetDatabase.IsValidFolder("Assets/Exportedhats"))
        {
            AssetDatabase.CreateFolder("Assets", "Exportedhats");
        }

        string hatName = descriptor.Name;
        // string hatAuthor = descriptor.Author;
        // string hatDescription = descriptor.Description;

        string prefabPathTEMP = "Assets/Exportedhats/hat.prefab";

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        PrefabUtility.SaveAsPrefabAsset(selectedObject.gameObject, prefabPathTEMP);
        GameObject contentsRoot = PrefabUtility.LoadPrefabContents(prefabPathTEMP);
        contentsRoot.name = "hat";

        // Text player_info = contentsRoot.AddComponent<Text>();
        // string split = "$"; // splits each of the strings
        // player_info.text = hatName + split + hatAuthor + split + hatDescription;

        if (File.Exists(prefabPathTEMP))
        {
            File.Delete(prefabPathTEMP);
        }

        string newprefabPath = "Assets/Exportedhats/" + contentsRoot.name + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(contentsRoot, newprefabPath);
        PrefabUtility.UnloadPrefabContents(contentsRoot);
        AssetImporter.GetAtPath(newprefabPath).SetAssetBundleNameAndVariant("hatAssetBundle", "");

        if (!Directory.Exists("Assets/Exportedhats"))
        {
            Directory.CreateDirectory(assetBundleDirectoryTEMP);
        }

        string asset_new = assetBundleDirectoryTEMP + "/" + hatName;
        if (File.Exists(asset_new + ".hat"))
        {
            File.Delete(asset_new + ".hat");
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectoryTEMP, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        if (File.Exists(newprefabPath))
        {
            File.Delete(newprefabPath);
        }

        string asset_temporary = assetBundleDirectoryTEMP + "/hatAssetBundle";
        string metafile = asset_temporary + ".meta";
        if (File.Exists(asset_temporary))
        {
            File.Move(asset_temporary, asset_new + ".hat");
        }

        AssetDatabase.Refresh();
        Debug.ClearDeveloperConsole();

        string path1 = assetBundleDirectoryTEMP + "/" + hatName + ".hat";
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
        EditorUtility.DisplayDialog("Export Success", $"Your hat was exported!", "OK");

        try
        {
            AssetDatabase.RemoveAssetBundleName("hatassetbundle", true);
        }
        catch
        {

        }

        string hatPath = path + "/";
        EditorUtility.RevealInFinder(hatPath);

        if (AssetDatabase.IsValidFolder("Assets/Exportedhats"))
        {
            AssetDatabase.DeleteAsset("Assets/Exportedhats");
        }
    }
}