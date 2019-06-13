using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int color;
    public int x;
    public int y;
    public float xPos;
    public float yPos;
    public GameObject obj;

    public Node(int color, int x, int y, float xPos, float yPos, GameObject obj)
    {
        this.color = color;
        this.x = x;
        this.y = y;
        this.xPos = xPos;
        this.yPos = yPos;
        this.obj = obj;
    }

    public void ActivateNode(bool activate)
    {
        SpriteRenderer clr = obj.GetComponent<SpriteRenderer>();
        clr.color = activate ? new Color(clr.color.r, clr.color.g, clr.color.b, 0.5f) : new Color(clr.color.r, clr.color.g, clr.color.b, 1f);
    }

    public void UpdateColor()
    {
        SpriteRenderer clr = obj.GetComponent<SpriteRenderer>();
        clr.color = ObjectCreator.CreateColor(color);
    }

}
