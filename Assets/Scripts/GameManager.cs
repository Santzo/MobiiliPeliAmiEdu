using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class SaveData
    {
        public int gridX, gridY;
        public List<string> gridData;
        public List<string> bonusPieceData;
    }


    [HideInInspector] public GameObject[] pieces;
    [HideInInspector] public TextAsset[] levels;
    [HideInInspector] public float wsW;
    [HideInInspector] public float wsH;
    [HideInInspector] public Vector2 scorePosition;
    [HideInInspector] public PlayerController player;

    public static GameManager instance;
    public Sprite backgroundPanel;
    public Sprite scoreBackground;


   

    private GameObject background;

    private void Awake()
    {

        wsH = Camera.main.orthographicSize * 2;
        wsW = wsH / Screen.height * Screen.width;

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        pieces = Resources.LoadAll<GameObject>("GamePieces/");
        levels = Resources.LoadAll<TextAsset>("Levels/");

    }

    private void Start()
    {
        background = GameObject.Find("Background");
        SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
        background.transform.localScale = new Vector3(wsW / sr.sprite.bounds.size.x, wsH / sr.sprite.bounds.size.y, 1);
        
    }

    public void LoadLevel(string filename, out int gridX, out int gridY, out List<string> activePieces, out List<string> bonusPieces)
    {
        var saveData = new SaveData();
        saveData.gridData = new List<string>();
        saveData.bonusPieceData = new List<string> ();
        TextAsset data = null;
        foreach (var asset in levels)
        {
            if (filename.ToUpper() == asset.name.ToUpper())
            {
                data = asset;
                break;
            }
        }
        data = data == null ? data = levels[0] : data;
        try
        {
            saveData = JsonUtility.FromJson<SaveData>(data.text);
        }
        catch
        {
            Debug.Log("Corrupted data.");
        }

        gridX = saveData.gridX;
        gridY = saveData.gridY;

        activePieces = saveData.gridData;
        bonusPieces = saveData.bonusPieceData;
       
    }


}
