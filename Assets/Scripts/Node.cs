using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public string name;
    public float scoreValue;
    public float[] comboMultiplier;
    public int x;
    public int y;
    public float xPos;
    public float yPos;
    public GameObject obj;
    public bool active;
    private GameObject background;

    public Node(string name, float scoreValue, float[] comboMultiplier, int x, int y, float xPos, float yPos, GameObject obj, bool _active = true)
    {
        this.name = name;
        this.scoreValue = scoreValue;
        this.comboMultiplier = comboMultiplier;
        this.x = x;
        this.y = y;
        this.xPos = xPos;
        this.yPos = yPos;
        this.obj = obj;
        this.active = _active;
    }

    public void ActivateNode(bool activate)
    {
        if (obj != null)
        {
            Animator anim = obj.GetComponent<Animator>();
            if (activate)
            {
                anim.SetTrigger("Activate");
                background = ObjectPooler.op.Spawn("NodeBackground", new Vector2(xPos, yPos));
                background.transform.localScale = Vector2.one * (Grid.grid.nodeSize - 0.05f);
            }
            else
            {
                background.SetActive(false);
            }
        }
    }


}
