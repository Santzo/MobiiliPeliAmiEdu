using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectCreator
{

    public static void ReplaceNodes(List<Node> replacedNodes)
    {
        CalculateScore(replacedNodes); 
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
                    Grid.grid.nodes[node.x, y].name = Grid.grid.nodes[node.x, y + 1].name;

                
                    Grid.grid.nodes[node.x, y].UpdateColor();
                    Grid.grid.nodes[node.x, y].active = true;
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
            Grid.grid.StartCoroutine(Grid.MoveNode(node.obj.transform, new Vector3(node.xPos, node.yPos, node.obj.transform.position.z)));
        }
    }

    public static void CalculateScore(List<Node> replacedNodes)
    {
        int combo = replacedNodes.Count;
        float score = replacedNodes[0].scoreValue;
        float comboMultiplier = replacedNodes.Count - 1 > replacedNodes[0].comboMultiplier.Length ? replacedNodes[0].comboMultiplier[replacedNodes[0].comboMultiplier.Length - 1] : replacedNodes[0].comboMultiplier[replacedNodes.Count - 2];
        int addedScore = Mathf.RoundToInt(score * combo * comboMultiplier);
        CreateScoreText(addedScore, new Vector2(replacedNodes[replacedNodes.Count - 1].xPos, replacedNodes[replacedNodes.Count - 1].yPos));
        GameManager.instance.player.Score += addedScore;
    }

    public static void CreateScoreText(int score, Vector2 pos)
    {
        GameObject text = ObjectPooler.op.Spawn("ScoreText", pos);
        text.GetComponent<Score>().text.text = "+" + score;
    }


}
