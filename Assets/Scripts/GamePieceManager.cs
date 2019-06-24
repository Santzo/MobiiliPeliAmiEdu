using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GamePieceManager : MonoBehaviour
{
    public GameObject[] pieces;
    public static GamePieceManager instance;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        pieces = LoadAssets<GameObject>("Prefabs/GamePieces");
    }

    private T[] LoadAssets<T>(string path)
    {
        List<object> al = new List<object>();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);

        foreach (string fileName in fileEntries)
        {
            if (!fileName.EndsWith(".meta"))
            {
                string temp = fileName.Replace("\\", "/");
                int index = temp.LastIndexOf("/");
                string localPath = "Assets/" + path;

                if (index > 0)
                    localPath += temp.Substring(index);
                Debug.Log(localPath);
                Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));
                if (t != null && t.GetType() == typeof(GameObject))
                {
                    GameObject a = (GameObject) t;
                    GamePiece piece = a.GetComponent<GamePiece>();

                    if (piece != null)
                        al.Add(a);
                }
            }
        }

        T[] result = new T[al.Count];
        for (int i = 0; i < al.Count; i++)
            result[i] = (T)al[i];

        return result;
    }
}
