using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectCreator
{
    public static Color CreateColor(int color)
    {
        switch (color)
        {
            case 1:
                {
                    return Color.blue;
                }
            case 2:
                {
                    return Color.red;
                }
            case 3:
                {
                    return Color.green;
                }
            case 4:
                {
                    return Color.yellow;
                }
            case 5:
                {
                    return Color.magenta;
                }
        }

        return Color.white;

    }

    public static void ReplaceNodes(List<Node> replacedNodes)
    {
        List<Node> nodesToMove = new List<Node>();
        replacedNodes = replacedNodes.OrderByDescending(o => o.y).ToList();

        foreach (Node node in replacedNodes)
        {
            for (int y = node.y; y < Grid.grid.gridY; y++)
            {
                if (y < Grid.grid.gridY - 1)
                {
                    if (Grid.grid.nodes[node.x, y + 1].active == false)
                    {
                        Grid.grid.nodes[node.x, y].active = false;
                        break;
                    }

                    Grid.grid.nodes[node.x, y].obj = Grid.grid.nodes[node.x, y + 1].obj;
                    Grid.grid.nodes[node.x, y].color = Grid.grid.nodes[node.x, y + 1].color;

                    //if (!nodesToMove.Contains(Grid.grid.nodes[node.x, y]))
                    //    nodesToMove.Add(Grid.grid.nodes[node.x, y]);
                    //else
                    //{
                    //    nodesToMove.Remove(Grid.grid.nodes[node.x, y]);
                    //    nodesToMove.Add(Grid.grid.nodes[node.x, y]);
                    //}
    
                    Grid.grid.nodes[node.x, y].UpdateColor();
                    Grid.grid.nodes[node.x, y].active = true;
                    //Grid.grid.nodes[node.x, y].obj.transform.position = new Vector2(Grid.grid.nodes[node.x, y].xPos, Grid.grid.nodes[node.y, y].yPos);
                }
                else
                {
                    Grid.grid.nodes[node.x, y].active = false;
                }
            }
        }
        foreach (Node node in Grid.grid.nodes)
        {
            if (node.obj.transform.position.y != node.yPos && node.active)
                nodesToMove.Add(node);
        }

        foreach (Node node in nodesToMove)
        {
            Grid.grid.StartCoroutine(Grid.MoveNode(node.obj.transform, new Vector2(node.xPos, node.yPos)));
        }
    }



}
