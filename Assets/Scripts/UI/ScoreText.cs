using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour
{
    Text scoreText;
    GameManager gm;
    private void Awake()
    {
        scoreText = GetComponent<Text>();
        gm = FindObjectOfType<GameManager>();
    }
    private void Start()
    {
        gm.PointsChanged += UpdateText;
    }
    void UpdateText(int points)
    {
        scoreText.text = "Score : " + points.ToString();
    }

    private void OnDisable()
    {
        gm.PointsChanged -= UpdateText;
    }
}