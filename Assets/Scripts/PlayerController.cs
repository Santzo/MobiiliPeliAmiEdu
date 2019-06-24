using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Camera _camera;
    private Node[] nodeCombo;
    private int nextInCombo = 0;
    private int maximumCombo = 10;
    private LineRenderer lr;
    List<Node> nodesToReplace = new List<Node>();

    private float score = 0f;
    public float Score
    {
        get
        {
            return score;
        }
        set
        {
            if (score == value)
                return;
            score = value;
            EventManager.OnScoreChange();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main.GetComponent<Camera>();
        nodeCombo = new Node[maximumCombo];
        lr = GetComponent<LineRenderer>();
        lr.positionCount = nextInCombo;
        lr.widthMultiplier = lr.startWidth;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
            ClearNodes();

        if (Input.GetMouseButton(0))
            AddPieceToCombo();
        if (Input.GetKeyDown(KeyCode.Space))
            Score += 500;
    }

    private bool IsNeighborNode(Node node, Node targetNode)
    {
        if (Mathf.Abs(node.x - targetNode.x) <= 1 && Mathf.Abs(node.y - targetNode.y) <= 1)
            return true;
        return false;
    }

    private void AddPieceToCombo()
    {
        Vector2 pos = _camera.ScreenToWorldPoint(Input.mousePosition);

        if (nextInCombo > 0)
            lr.SetPosition(nextInCombo, new Vector3(pos.x, pos.y, 5f));

        RaycastHit2D ray = Physics2D.Raycast(pos, Vector2.zero);

        if (ray)
        {
            Node nodeInfo = Grid.ReturnNodeInfo(ray.point);

            if (nodeInfo == null || nextInCombo == 1 && nodeCombo[nextInCombo - 1] == nodeInfo || nextInCombo >= maximumCombo || !nodeInfo.active)
                return;

            //Debug.Log(nodeInfo.x + " "  + nodeInfo.y);
            if (nextInCombo > 1)
            {
                if (nodeInfo == nodeCombo[nextInCombo - 2])
                {
                    nodeCombo[nextInCombo - 1].ActivateNode(false);
                    nodeCombo[nextInCombo - 1] = null;
                    nextInCombo--;
                    lr.positionCount--;
                    return;
                }
                else
                {
                    foreach (Node node in nodeCombo)
                    {
                        if (nodeInfo == node)
                            return;
                    }
                }
            }



            if (nextInCombo > 0)
            {
                if (!IsNeighborNode(nodeCombo[nextInCombo - 1], nodeInfo))
                    return;
                if (nodeInfo.name != nodeCombo[0].name)
                    return;
            }

            if (nodeCombo[nextInCombo] == null)
            {
                nodeCombo[nextInCombo] = nodeInfo;
                nodeCombo[nextInCombo].ActivateNode(true);
                nextInCombo++;
                lr.positionCount = nextInCombo + 1;
                lr.SetPosition(nextInCombo - 1, nodeInfo.obj.transform.position);
                lr.SetPosition(nextInCombo, pos);
            }
            
        }
    }

    private void ClearNodes()
    {
        nodesToReplace.Clear();
        for (int i = 0; i < nodeCombo.Length; i++)
        {
            if (nodeCombo[i] != null)
            {
                nodeCombo[i].ActivateNode(false);
                if (nextInCombo > 1)
                {
                    nodeCombo[i].active = false;
                    nodeCombo[i].obj.SetActive(false);

                    nodesToReplace.Add(nodeCombo[i]);
                }
                nodeCombo[i] = null;
            }
        }

        if (nodesToReplace != null && nodesToReplace.Count > 1)
            ObjectCreator.ReplaceNodes(nodesToReplace);

        nextInCombo = lr.positionCount = 0;
    }
}
