using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEditor.Experimental.RestService;

public class AssetDataMapGenerator : MonoBehaviour
{
    [MenuItem("Tools/Generate AssetReferenceData from Label")]
    public static void BuildDatabase()
    {
        string[] guids = AssetDatabase.FindAssets("l:ItemData");

        var db = ScriptableObject.CreateInstance<AssetReferenceData>();
        db.data = new();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var loadedAsset = AssetDatabase.LoadAssetAtPath<BaseScriptableObject>(path);

            if (loadedAsset != null)
            {
                db.data.Add(new AssetData
                {
                    ID = loadedAsset.ID,
                });
            }
        }

        string savePath = "Assets/07. Data/ItemDatabase.asset";
        Directory.CreateDirectory(Path.GetDirectoryName(savePath));

        AssetDatabase.CreateAsset(db, savePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Database built successfully at " + savePath);
    }

}
