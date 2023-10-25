using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets._ScriptsA
{
    public class ScoreManager : MonoBehaviour
    {
        public TMP_Text scoreText;
        public int score;

        public void IncreaseScore(int increaseAmount)
        {
            score += increaseAmount;
            scoreText.text = $"{score:0000}";
        }
    }
}