using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelEditor : EditorWindow
{
    [System.Serializable]
    public class EditorGrid
    {
        public int gridX = 4, gridY = 4;
        [System.Serializable]
        public class GridInfo
        {
            public string name;
            public Vector2 texturePos;
            public Texture2D sprite;
        }
    }

    [System.Serializable]
    public class SaveData
    {
        public int gridX, gridY;
        public List<string> gridData;
        public List<string> bonusPieceData;
    }

    private bool needsRepaint;
    private SaveData saveData;
    private GameObject[] go;
    private GameObject coin;
    private GUISkin style;
    private EditorGrid grid;
    private EditorGrid.GridInfo[,] gridInfo;
    private string levelName;
    private float range;

    private int GridX
    {
        get
        {
            return grid.gridX;
        }
        set
        {
            if (grid.gridX == value)
                return;
            grid.gridX = value;
            ChangeGridSize();
        }
    }

    private int GridY
    {
        get
        {
            return grid.gridY;
        }
        set
        {
            if (grid.gridY == value)
                return;
            grid.gridY = value;
            ChangeGridSize();
        }
    }

    private int nodeSize = 70;
    private int halfNode;
    private int[] amountToDrop;

    [MenuItem("Window/LevelEditor %e")]
    public static void Init()
    {
        var window = GetWindow<LevelEditor>("Level Editor");
        window.maxSize = new Vector2(1000f, 750f);
        window.minSize = window.maxSize;
        window.maximized = true;
        window.Show();
    }

    private void OnGUI()
    {
        Event e = Event.current;
        Handles.color = Color.black;
        GUI.Box(new Rect(halfNode, halfNode, GridX * nodeSize, GridY * nodeSize), "", style.box);

        for (int x = 1; x < GridX; x++)
        {
            Handles.DrawLine(new Vector2(x * nodeSize + halfNode, halfNode), new Vector2(x * nodeSize + halfNode, GridY * nodeSize + halfNode));
        }
        for (int y = 1; y < GridY; y++)
        {
            Handles.DrawLine(new Vector2(halfNode, y * nodeSize + halfNode), new Vector2(GridX * nodeSize + halfNode, y * nodeSize + halfNode));
        }

        GUI.Box(new Rect(halfNode, 600f, 400f, position.height - halfNode - 600f), "", style.button); // Luodaan gridin koko tausta + sliderit
        GUI.Label(new Rect(halfNode + 10f, 610f, 280f, 30f), "Grid Size X", style.label);
        GridX = EditorGUI.IntSlider(new Rect(halfNode + 10f, 630f, 280f, 15f), GridX, 1, 7);

        GUI.Label(new Rect(halfNode + 10f, 660f, 280f, 30f), "Grid Size Y", style.label);
        GridY = EditorGUI.IntSlider(new Rect(halfNode + 10f, 680f, 280f, 15f), GridY, 1, 7);

        GUI.Box(new Rect(550f, halfNode, position.width - halfNode - 550f, position.height - nodeSize), "", style.button);

        GUI.Label(new Rect(560f, halfNode + 10f, 250f, 30f), "Level Editor", style.customStyles[1]);

        int posX = 560;
        int posY = halfNode + 50;

        for (int i = 0; i < go.Length; i++)
        {
            SpriteRenderer sprite = go[i].GetComponent<SpriteRenderer>();
            if (go[i] == Selection.activeGameObject)
                GUI.Button(new Rect(posX, posY, 140f, 94f), sprite.sprite.texture, style.customStyles[0]);

            else
                if (GUI.Button(new Rect(posX, posY, 140f, 94f), sprite.sprite.texture, style.button))
                Selection.activeGameObject = go[i];

            amountToDrop[i] = EditorGUI.IntField(new Rect(posX, posY + 100f, 140f, 16f), amountToDrop[i]);
            amountToDrop[i] = amountToDrop[i] < 0 ? 0 : amountToDrop[i];

            if (i % 2 > 0 && i > 0 && i < go.Length - 1)
            {
                posY += 150;
                posX -= 200;
            }
            else
                posX += 200;
        }

        GUI.Label(new Rect(560f, posY + 130f, 150f, 20f), "Level Name", style.label);
        levelName = EditorGUI.TextField(new Rect(700f, posY + 134f, 200f, 16f), levelName);

        if (GUI.Button(new Rect(560f, posY + 170f, 110f, 30f), "Save Level", style.button)) // SAVE LEVEL NAPPULA
        {
            if (levelName == "" || levelName == null)
                ShowMessage("Please enter a level name");
            else
                SaveLevel();
        }


        if (GUI.Button(new Rect(680f, posY + 170f, 110f, 30f), "Load Level", style.button))  // LOAD LEVEL NAPPULA
        {
            string[] menuOptions = RetrieveLevels();
            if (menuOptions.Length > 0)
            {
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < menuOptions.Length; i++)
                {
                    int a = i;
                    menu.AddItem(new GUIContent(menuOptions[i]), false, () => LoadLevel(menuOptions[a]));
                }
                menu.ShowAsContext();
            }
        }

        if (GUI.Button(new Rect(800f, posY + 170f, 110f, 30f), "Clear Level", style.button)) // CLEAR LEVEL NAPPULA
        {
            if (EditorUtility.DisplayDialog("Clear level?", "Are you sure you want to completely clear the level?", "OK", "Cancel"))
            {
                gridInfo = new EditorGrid.GridInfo[GridX, GridY];
                for (int x = 0; x < GridX; x++)
                {
                    for (int y = 0; y < GridY; y++)
                    {
                        gridInfo[x, y] = new EditorGrid.GridInfo();
                        int texY = Mathf.Abs(y - (GridY - 1));
                        gridInfo[x, y].texturePos = new Vector2(x * nodeSize + halfNode, texY * nodeSize + halfNode);
                    }
                }
                for (int i = 0; i < amountToDrop.Length; i++)
                {
                    amountToDrop[i] = 0;
                }
                levelName = "";
            }
        }



        range = range + Time.deltaTime;
        if (e.type == EventType.MouseDown && e.button == 0)
            CheckMousePosition(e.mousePosition, true);
        if (range > 0.1f)
        {
            for (int x = 0; x < GridX; x++)
            {
                for (int y = 0; y < GridY; y++)
                {
                    if (gridInfo[x, y].sprite != null)
                        GUI.DrawTexture(new Rect(gridInfo[x, y].texturePos, new Vector2(nodeSize, nodeSize)), gridInfo[x, y].sprite);
                }
            }
            this.Repaint();
            CheckMousePosition(e.mousePosition);
            range = 0.0f;
        }

    }
    void CheckMousePosition(Vector2 pos, bool button = false)
    {
        if (pos.x < halfNode || pos.y < halfNode || pos.x > GridX * nodeSize + halfNode || pos.y > GridY * nodeSize + halfNode)
            return;
        int newPosX;
        int newPosY;
        newPosX = Mathf.Clamp(Mathf.CeilToInt((pos.x - halfNode) / nodeSize) - 1, 0, GridX - 1);
        newPosY = Mathf.Clamp(Mathf.CeilToInt((pos.y - halfNode) / nodeSize) - 1, 0, GridY - 1);
        EditorGUI.DrawRect(new Rect(newPosX * nodeSize + halfNode, newPosY * nodeSize + halfNode, nodeSize, nodeSize), new Color(0.35f, 0.92f, 0.09f, 0.4f));

        if (button)
        {
            if (Selection.activeGameObject == null)
                return;
            newPosY = GridY - newPosY - 1;
            gridInfo[newPosX, newPosY].name = Selection.activeGameObject.name;
            gridInfo[newPosX, newPosY].sprite = Selection.activeGameObject.GetComponent<SpriteRenderer>().sprite.texture;


        }


    }


    private void OnEnable()
    {

        halfNode = nodeSize / 2;
        coin = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Coin.prefab", typeof(GameObject));
        style = (GUISkin)Resources.Load("CustomSkin");
        go = LoadGamePieces<GameObject>("Resources/GamePieces");
        amountToDrop = new int[go.Length];
        grid = new EditorGrid();
        ChangeGridSize();
        amountToDrop = new int[go.Length];


    }
    private void OnDisable()
    {
        foreach (GameObject obj in go)
        {
            DestroyImmediate(obj);
        }

        gridInfo = null;
    }

    private T[] LoadGamePieces<T>(string path)
    {

        ArrayList al = new ArrayList();
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
                Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));
                if (t != null)
                {
                    GameObject _temp = new GameObject();
                    GameObject a = (GameObject)t;
                    GamePiece gp = a.GetComponent<GamePiece>();
                    if (gp != null)
                    {
                        _temp.AddComponent<SpriteRenderer>().sprite = gp.levelEditorSprite;
                        _temp.name = a.name;
                        al.Add(_temp);
                    }
                }
            }
        }

        T[] result = new T[al.Count];

        for (int i = 0; i < al.Count; i++)
            result[i] = (T)al[i];

        return result;
    }

    void ChangeGridSize()
    {

        var tempGrid = gridInfo;
        gridInfo = new EditorGrid.GridInfo[GridX, GridY];

        for (int x = 0; x < GridX; x++)
        {
            for (int y = 0; y < GridY; y++)
            {
                gridInfo[x, y] = new EditorGrid.GridInfo();
                if (tempGrid == null || tempGrid.GetLength(0) - 1 < x || tempGrid.GetLength(1) - 1 < y || tempGrid[x, y] == null)
                {

                    gridInfo[x, y].name = null;
                    gridInfo[x, y].sprite = null;
                }
                else
                {
                    gridInfo[x, y] = tempGrid[x, y];
                }
                int texY = Mathf.Abs(y - (GridY - 1));
                gridInfo[x, y].texturePos = new Vector2(x * nodeSize + halfNode, texY * nodeSize + halfNode);
            }
        }

    }

    void ShowMessage(string warning)
    {
        ShowNotification(new GUIContent(warning), 1.5);
        return;
    }

    void SaveLevel()
    {
        var file = Application.dataPath + "/Resources/Levels/" + levelName + ".txt";
        if (File.Exists(file))
        {
            if (!EditorUtility.DisplayDialog("Level already exists", "Are you sure you want to want to overwrite the save file?", "OK", "Cancel"))
                return;
        }

        File.Create(file).Dispose();

        saveData = new SaveData();
        saveData.gridX = GridX;
        saveData.gridY = GridY;
        saveData.gridData = new List<string>();
        saveData.bonusPieceData = new List<string>();
        for (int x = 0; x < GridX; x++)
        {
            for (int y = 0; y < GridY; y++)
            {
                saveData.gridData.Add(gridInfo[x, y].name);
            }
        }
        for (int i = 0; i < amountToDrop.Length; i++)
        {
            for (int a = 0; a < amountToDrop[i]; a++)
            {
                saveData.bonusPieceData.Add(go[i].name);
            }
        }


        var data = JsonUtility.ToJson(saveData);
        File.WriteAllText(file, data);
        ShowMessage("Level saved.");
    }

    string[] RetrieveLevels()
    {
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/Resources/Levels/");
        List<string> availableLevels = new List<string>();
        foreach (string fileName in fileEntries)
        {
            if (fileName.EndsWith(".txt"))
            {
                string temp = fileName.Replace("\\", "/");
                int index = temp.LastIndexOf("/");
                int indexTwo = temp.LastIndexOf(".");
                availableLevels.Add(temp.Substring(index + 1, indexTwo - index - 1));
                string final = "Assets/Resources/Levels" + temp.Substring(index);
            }
        }
        string[] levels = availableLevels.ToArray();
        return levels;
    }

    void LoadLevel(string level)
    {
        string alevel = Application.dataPath + "/Resources/Levels/" + level + ".txt";
        if (File.Exists(alevel))
        {
            if (!EditorUtility.DisplayDialog("Load level?", "Are you sure you want to want to load " + level + " level? All unsaved changes will be deleted.", "OK", "Cancel"))
                return;
        }

        levelName = level;
        saveData = new SaveData();
        saveData.gridData = new List<string>();
        saveData.bonusPieceData = new List<string>();

        var data = File.ReadAllText(alevel);

        try
        {
            saveData = (SaveData)JsonUtility.FromJson(data, typeof(SaveData));
        }
        catch 
        {
            ShowMessage("Corrupted level file. ");
            return;
        }

        gridInfo = new EditorGrid.GridInfo[saveData.gridX, saveData.gridY];
        GridX = saveData.gridX;
        GridY = saveData.gridY;
        for (int x = 0; x < GridX; x++)
        {
            for (int y = 0; y < GridY; y++)
            {
                gridInfo[x, y] = new EditorGrid.GridInfo();
                gridInfo[x, y].name = saveData.gridData[y + (x * GridY)];
                int texY = Mathf.Abs(y - (GridY - 1));
                gridInfo[x, y].texturePos = new Vector2(x * nodeSize + halfNode, texY * nodeSize + halfNode);

                if (gridInfo[x, y].name != "" && gridInfo[x, y].name != null)
                {
                    foreach (GameObject obj in go)
                    {
                        if (gridInfo[x, y].name == obj.name)
                        {
                            gridInfo[x, y].sprite = obj.GetComponent<SpriteRenderer>().sprite.texture;
                            break;
                        }
                    }
                }
                else { gridInfo[x, y].sprite = null; }
            }
        }

        for (int a = 0; a < saveData.bonusPieceData.Count; a++)
        {
            for (int i = 0; i < amountToDrop.Length; i++)
            {
                if (saveData.bonusPieceData[a] == go[i].name)
                {
                    amountToDrop[i]++;
                    break;
                }
                
            }
        }
        ShowMessage("Level loaded.");


    }
}


