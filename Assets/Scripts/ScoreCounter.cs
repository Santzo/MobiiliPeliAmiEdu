using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    TextMeshPro scoreText;
    int currentScore;

    private void Awake()
    {
        scoreText = transform.GetChild(0).GetComponent<TextMeshPro>();
        EventManager.OnScoreChange += delegate { StopCoroutine(UpdateScore());  StartCoroutine(UpdateScore()); };

    }

    void Start()
    {
        currentScore = Mathf.RoundToInt(GameManager.instance.player.Score);
        GetComponent<TextMeshPro>().text = Colors.Color("Red") + "Score";
        scoreText.text = Colors.Color("White") + " " + currentScore.ToString("D5");
    }

    private IEnumerator UpdateScore()
    {
        int targetScore = Mathf.RoundToInt(GameManager.instance.player.Score);
        while (currentScore < targetScore)
        {
            float diff = (targetScore - currentScore) * 0.05f > 1 ? (targetScore - currentScore) * 0.05f : 1;
            currentScore += Mathf.RoundToInt(diff);
            scoreText.text = Colors.Color("White") + " " + currentScore.ToString("D5");
            yield return null;
        }
        currentScore = targetScore;
        scoreText.text = Colors.Color("White") + " " + currentScore.ToString("D5");

        yield return null;

    }
}
