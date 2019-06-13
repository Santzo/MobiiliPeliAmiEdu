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
        replacedNodes = replacedNodes.OrderByDescending(o => o.y).ToList();
        foreach (Node node in replacedNodes)
        {
            for (int y = node.y; y < Grid.grid.gridY; y++)
            {
                if (y < Grid.grid.gridY - 1)
                {
                    if (!Grid.grid.nodes[node.x, y + 1].obj.activeSelf)
                    {
                        Grid.grid.nodes[node.x, y].obj.SetActive(false);
                        break;
                    }
                    Grid.grid.nodes[node.x, y].color = Grid.grid.nodes[node.x, y + 1].color;
                    Grid.grid.nodes[node.x, y].obj.SetActive(true);
                    Grid.grid.nodes[node.x, y].UpdateColor();
                }
                else
                    Grid.grid.nodes[node.x, y].obj.SetActive(false);
            }
        }
    }

}
