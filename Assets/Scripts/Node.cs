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
            SpriteRenderer clr = obj.GetComponent<SpriteRenderer>();
            clr.color = activate ? new Color(clr.color.r, clr.color.g, clr.color.b, 0.5f) : new Color(clr.color.r, clr.color.g, clr.color.b, 1f);
        }
    }

    public void UpdateColor()
    {
        SpriteRenderer clr = obj.GetComponent<SpriteRenderer>();
    }

}
