using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [Header("Grid Size")]
    public int gridX;
    public int gridY;
    public Material coinMaterial;
    [Space]
    public Node[,] nodes;
    public float nodeSize = 1f;

    public static Vector2 startPos;
    public static Grid grid;

    private Transform centerPoint;

    private void Awake()
    {
        grid = this;
    }

    void Start()
    {
        centerPoint = transform.GetChild(0);
        nodes = new Node[gridX, gridY];
        nodeSize += (GameManager.instance.wsW * GameManager.instance.wsW) * 0.01f;
        if (GameManager.instance.wsW == 6f) nodeSize -= 0.03f;
        startPos = new Vector2(-(gridX * 0.5f * nodeSize) - Mathf.Abs(centerPoint.position.x), -(gridY * 0.5f * nodeSize) - Mathf.Abs(centerPoint.position.y));
        DrawGrid();
        InitializeField();
    }



    void DrawGrid()                                     // Piirrettään pelikenttä
    {
        CreateGridBackground();                         // Kutsutaan funktiota joka luo pelikentän taustan  

        for (int x = 1; x <= gridX - 1; x++)            // Näillä kahdella for loopilla luodaan ruudukon viivat. Käyttää tällä hetkellä linerenderiä ihan yksinkertaisiin viivoihin
        {                                               // Korvataan todennäköisesti jossain vaiheessa jollain viivaspriteillä.
            GameObject obj = new GameObject();
            obj.transform.parent = transform;
            LineRenderer lr = obj.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.material = new Material(Shader.Find("Sprites/Default"));

            lr.SetPosition(0, new Vector2(startPos.x + x * nodeSize, startPos.y));
            lr.SetPosition(1, new Vector2(startPos.x + x * nodeSize, startPos.y + gridY * nodeSize));
            lr.startWidth = 0.025f;
            lr.endWidth = 0.025f;
            lr.startColor = new Color(0.57f, 0.33f, 0.16f);
            lr.endColor = new Color(0.36f, 0.18f, 0.04f);
            lr.sortingOrder = 2;

        }
        for (int y = 1; y <= gridY - 1; y++)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = transform;
            LineRenderer lr = obj.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.material = new Material(Shader.Find("Sprites/Default"));

            lr.SetPosition(0, new Vector2(startPos.x, startPos.y + y * nodeSize));
            lr.SetPosition(1, new Vector2(startPos.x + gridX * nodeSize, startPos.y + y * nodeSize));
            lr.startWidth = 0.025f;
            lr.endWidth = 0.025f;
            lr.startColor = new Color(0.36f, 0.18f, 0.04f);
            lr.endColor = new Color(0.57f, 0.33f, 0.16f);
            lr.sortingOrder = 2;

        }

    }

    void InitializeField()                              // Luodaan kenttä, eli käydään nollasta maksi X ja y koordinaatteihin.
    {
        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                float posX = (x * nodeSize + (nodeSize * 0.5f) - Mathf.Abs(startPos.x));    // Lasketaan tyhjän ruudun kohta x-koordinaatista, kerrottaan nodeSizellä
                float posY = (y * nodeSize + (nodeSize * 0.5f) - Mathf.Abs(startPos.y));

                GameObject obj = ObjectPooler.op.Spawn("GamePiece", new Vector3(posX, posY, -2f));  // Luodaan uusi "GamePiece" käyttäen ObjectPooleria. Objectpoolerin tarkoitus siis tosiaan on se
                int piece = Random.Range(0, GamePieceManager.instance.pieces.Length);               // että mitään objekteja ei ikinä oikeasti tuhottaisiin, vaan niitä kierrätetään aktivoimalla /
                GamePiece tempPiece = GamePieceManager.instance.pieces[piece].GetComponent<GamePiece>();    // deaktivoimalla niitä.

                obj.name = GamePieceManager.instance.pieces[piece].name;
                obj.transform.localScale = new Vector3(obj.transform.localScale.x * nodeSize, obj.transform.localScale.y * nodeSize, 0.07f);
                Animator anim = obj.GetComponent<Animator>();
                anim.enabled = true;
                

                Renderer rend = obj.GetComponent<Renderer>();
                MeshRenderer mesh = obj.GetComponent<MeshRenderer>();
                rend.material = coinMaterial;
                rend.material.SetTexture("_MainTex", tempPiece.sprite.texture);



                nodes[x, y] = new Node(obj.name, tempPiece.scoreValue, tempPiece.comboMultiplier, x, y, posX, posY, obj);   // Luodaan uusi Node-luokka x,y koordinaatteihin. Tämä luokka pitää sisällään
                                                                                                                            // kaiken yhteen ruutuun kuuluvan tärkeän tiedon

            }
        }
    }

    public static Node ReturnNodeInfo(Vector2 pos)                                                          // Tämä funktio palauttaa oikean x,y koordinaatin kun pelaaja painaa pelinappulan kohtaan
    {                                                                                                       // Tätä siis kutsutaan PlayerController luokasta, kun pelaaja sormellaan osoitta tiettyä pelinappulaa
        float currentX = pos.x / grid.nodeSize + Mathf.Abs(grid.centerPoint.position.x);
        float currentY = pos.y / grid.nodeSize + Mathf.Abs(grid.centerPoint.position.y / grid.nodeSize);

        float gridSizeX = grid.gridX / 2f;
        float gridSizeY = grid.gridY / 2f;

        int x = Mathf.FloorToInt(currentX + gridSizeX);
        int y = Mathf.FloorToInt(currentY + gridSizeY);

        return grid.nodes[x, y];
    }


    public static IEnumerator MoveNode(Transform obj, Vector3 targetPos)                    // Funktio jota käytetään pelinappuloihin jotka ovat tyhjän ruudun päällä. Tätä kutsutaan siis silloin
    {                                                                                       // kun pelinappuloita tuhotaan kentältä.
        float moveSpeed = Random.Range(0.05f, 0.12f);
        while (obj.transform.position.y != targetPos.y)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPos, moveSpeed);
            yield return null;
        }
        obj.transform.position = targetPos;
        yield return null;
    }

    public static IEnumerator BlowUpCoins(List<Node> nodes)                                 // Räjäytetään kolikot, lisäämällä niihin satunnainen määrä voimaa
    {
        GameObject[] obj = new GameObject[nodes.Count];

        for (int i = 0; i < nodes.Count; i++)
        {
            obj[i] = nodes[i].obj;
            obj[i].GetComponent<Animator>().enabled = false;
        }
        foreach (GameObject _obj in obj)
        {
            Rigidbody rb = _obj.GetComponent<Rigidbody>();
            rb.isKinematic = false;

            Vector3 force = new Vector3(Random.Range(-300f, 300f), Random.Range(100f, 700f), Random.Range(-300f, 0f));
            rb.AddForce(force);
            rb.AddTorque(force);
        }

        yield return new WaitForSeconds(3f);

        foreach (GameObject _obj in obj)
        {
            Rigidbody rb = _obj.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            _obj.SetActive(false);
        }


        Debug.Log("Jaa");
    }

    private void CreateGridBackground()
    {
        GameObject panel = new GameObject();
        panel.transform.parent = transform;

        SpriteRenderer panelSr = panel.AddComponent<SpriteRenderer>();
        panelSr.sprite = GameManager.instance.backgroundPanel;
        panelSr.drawMode = SpriteDrawMode.Sliced;
        panelSr.color = new Color(1f, 1F, 1f, 1f);

        panelSr.sortingOrder = 1;
        panel.transform.localScale = new Vector3(gridX * nodeSize, gridY * nodeSize, 0f);
        panel.transform.position = new Vector3(startPos.x, startPos.y + gridY * nodeSize, 0f);

        GameObject scorePanel = new GameObject();
        scorePanel.transform.parent = transform;

        SpriteRenderer scoreSr = scorePanel.AddComponent<SpriteRenderer>();
        scoreSr.sprite = GameManager.instance.scoreBackground;
        scoreSr.drawMode = SpriteDrawMode.Sliced;
        scoreSr.color = panelSr.color;

        scoreSr.sortingOrder = 2;
        scoreSr.transform.localScale = new Vector3(panel.transform.localScale.x * 1.05f, panel.transform.localScale.y * 1.25f, 1f);
        scoreSr.transform.position = new Vector3(startPos.x, startPos.y);

        GameManager.instance.scorePosition = new Vector2(scoreSr.transform.position.x + (scoreSr.bounds.size.x * 0.65f), scoreSr.transform.position.y - (scoreSr.bounds.size.y * 0.125f));
        GameObject.FindGameObjectWithTag("ScoreCounter").transform.position = GameManager.instance.scorePosition;
    }
}
