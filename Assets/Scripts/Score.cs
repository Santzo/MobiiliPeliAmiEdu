using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public TextMeshPro text;
    float moveSpeed = 0f;
    float moveDirection = 0f;
    float fadeSpeed = 0.015f;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }

    private void OnEnable()
    {
        moveSpeed = Random.Range(1.5f, 3f);
        moveDirection = Random.Range(-3f, 3f);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
    }

    void Update()
    {
        transform.position = new Vector2(transform.position.x + moveDirection * Time.deltaTime, transform.position.y + moveSpeed * Time.deltaTime);
        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - fadeSpeed);
        if (text.color.a <= 0f)
            gameObject.SetActive(false);
    }
}
