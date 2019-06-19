using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
   
    [HideInInspector] public float wsW;
    [HideInInspector] public float wsH;

    public static GameManager gm;
    public Sprite backgroundPanel;
    private GameObject background;

    private void Awake()
    {
        wsH = Camera.main.orthographicSize * 2;
        wsW = wsH / Screen.height * Screen.width;

        if (gm == null)
        {
            gm = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Debug.Log(wsH + " " + wsW + " " + (wsW / wsH));
    }

    private void Start()
    {
        background = GameObject.Find("Background");
        SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
        background.transform.localScale = new Vector3(wsW / sr.sprite.bounds.size.x, wsH / sr.sprite.bounds.size.y, 1);
        
    }


}
