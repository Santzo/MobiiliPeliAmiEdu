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

        pieces = Resources.LoadAll<GameObject>("GamePieces/");
    }


}
