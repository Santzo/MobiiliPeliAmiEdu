using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Colors
{
    public static string Color (string _color)
    {
        switch (_color)
        {
            case "Red":
                {
                    return "<color=#902111>";
                }
            case "White":
                {
                    return "<color=#FFFFFF>";
                }
        }
       

        return null;
    }
}
