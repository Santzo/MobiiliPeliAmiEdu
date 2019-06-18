using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [Header("Grid Size")]
    public int gridX;
    public int gridY;
    [Space]
    public Node[,] nodes;
    public float nodeSize = 1f;

    public static Vector2 startPos;
    public static Grid grid;

    private void Awake()
    {
        grid = this;
    }

    void Start()
    {
        nodes = new Node[gridX, gridY];
        startPos = new Vector2(-(gridX * 0.5f * nodeSize), -(gridY * 0.5f * nodeSize));
        DrawGrid();
        InitializeField();
    }



    void DrawGrid()
    {

        for (int x = 0; x <= gridX; x++)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = transform;
            LineRenderer lr = obj.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.material = new Material(Shader.Find("Sprites/Default"));

            lr.SetPosition(0, new Vector2(startPos.x + x * nodeSize, startPos.y));
            lr.SetPosition(1, new Vector2(startPos.x + x * nodeSize, startPos.y + gridY * nodeSize));
            lr.startWidth = 0.03f;
            lr.endWidth = 0.03f;
            lr.startColor = Color.white;
            lr.endColor = Color.grey;

        }
        for (int y = 0; y <= gridY; y++)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = transform;
            LineRenderer lr = obj.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.material = new Material(Shader.Find("Sprites/Default"));

            lr.SetPosition(0, new Vector2(startPos.x, startPos.y + y * nodeSize));
            lr.SetPosition(1, new Vector2(startPos.x + gridX * nodeSize, startPos.y + y * nodeSize));
            lr.startWidth = 0.03f;
            lr.endWidth = 0.03f;
            lr.startColor = Color.white;
            lr.endColor = Color.grey;

        }

    }

    void InitializeField()
    {
        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                float posX = (x * nodeSize + (nodeSize * 0.5f) - Mathf.Abs(startPos.x));
                float posY = (y * nodeSize + (nodeSize * 0.5f) - Mathf.Abs(startPos.y));
                GameObject obj = ObjectPooler.op.Spawn("TestSphere", new Vector2(posX, posY));
                obj.transform.localScale = Vector3.one * (nodeSize * 0.8f);

                int color = Random.Range(1, 6);
                nodes[x, y] = new Node(color, x, y, posX, posY, obj);

                obj.transform.GetComponent<SpriteRenderer>().color = ObjectCreator.CreateColor(color);
            }
        }
    }

    public static Node ReturnNodeInfo(Vector2 pos)
    {
        float currentX = pos.x / grid.nodeSize;
        float currentY = pos.y / grid.nodeSize;

        float gridSizeX = grid.gridX / 2f;
        float gridSizeY = grid.gridY / 2f;

        int x = Mathf.FloorToInt(currentX + gridSizeX);
        int y = Mathf.FloorToInt(currentY + gridSizeY);

        return grid.nodes[x, y];
    }


    public static IEnumerator MoveNode(Transform obj, Vector3 targetPos)
    {
        float moveSpeed = Random.Range(0.05f, 0.12f);
        while (obj.transform.position.y != targetPos.y)
        {
            obj.transform.position = Vector2.MoveTowards(obj.transform.position, targetPos, moveSpeed);
            yield return null;
        }
        obj.transform.position = targetPos;
        yield return null;
    }

}
