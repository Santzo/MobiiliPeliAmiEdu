﻿using System.Collections;
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

    private Transform centerPoint;

    private void Awake()
    {
        grid = this;
    }

    void Start()
    {
        centerPoint = transform.GetChild(0);
        nodes = new Node[gridX, gridY];
        nodeSize += (GameManager.gm.wsW * GameManager.gm.wsW) * 0.01f;
        if (GameManager.gm.wsW == 6f) nodeSize -= 0.03f;
        startPos = new Vector2(-(gridX * 0.5f * nodeSize) - Mathf.Abs(centerPoint.position.x), -(gridY * 0.5f * nodeSize) - Mathf.Abs(centerPoint.position.y));
        DrawGrid();
        InitializeField();
    }



    void DrawGrid()
    {
        GameObject panel = new GameObject();
        panel.transform.parent = transform;

        SpriteRenderer panelSr = panel.AddComponent<SpriteRenderer>();
        panelSr.sprite = GameManager.gm.backgroundPanel;
        panelSr.color = new Color(0.277f, 0.277f, 0.277f, 0.57f);

        panelSr.sortingOrder = 1;
        panel.transform.localScale = new Vector3(gridX * nodeSize, gridY * nodeSize, 0f);
        panel.transform.position = new Vector3(startPos.x, startPos.y + gridY * nodeSize, 0f);
        for (int x = 1; x <= gridX - 1; x++)
        {
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
        float currentX = pos.x / grid.nodeSize + Mathf.Abs(grid.centerPoint.position.x);
        float currentY = pos.y / grid.nodeSize + Mathf.Abs(grid.centerPoint.position.y / grid.nodeSize);

        float gridSizeX = grid.gridX / 2f;
        float gridSizeY = grid.gridY / 2f;

        int x = Mathf.FloorToInt(currentX + gridSizeX );
        int y = Mathf.FloorToInt(currentY + gridSizeY );

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
