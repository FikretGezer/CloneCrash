using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace Assets._ScriptsA
{
    public class ScoreManager : MonoBehaviour
    {
        public TMP_Text scoreText;
        public int score;

        private Board board;

        private void Awake()
        {
            board = FindObjectOfType<Board>();
        }

        public void IncreaseScore(int increaseAmount)
        {
            score += increaseAmount;
            scoreText.text = $"{score:0000}";
            if (board.world.levels[board.level] != null)
            {
                if(score >= board.world.levels[board.level].scoreGoal)
                {                    
                    //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }
    }
}