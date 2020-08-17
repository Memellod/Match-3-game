using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UI
{
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

        private void UpdateText(int points)
        {
            scoreText.text = "Score : " + points;
        }
    }
}