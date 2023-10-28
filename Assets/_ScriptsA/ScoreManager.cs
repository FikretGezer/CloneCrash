using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Assets._ScriptsA
{
    public class ScoreManager : MonoBehaviour
    {
        public TMP_Text scoreText;
        public int score;

        [Header("Goals")]
        public GameObject goalPrefab;
        public Transform goalHolder;

        private int[] goalCounts;
        private Image[] goalImages;
        private GameObject[] goalCountTexts;

        private Board board;
        private Level lvl;
        private int level;

        private void Start()
        {
            board = FindObjectOfType<Board>();
            if (PlayerPrefs.HasKey("Selected Level"))
                level = PlayerPrefs.GetInt("Selected Level");

            lvl = board.world.levels[level];
            if(lvl != null)
            {
                goalCounts = new int[lvl.goals.Length];
                goalCountTexts = new GameObject[lvl.goals.Length];
                goalImages = new Image[lvl.goals.Length];

                for (int i = 0; i < lvl.goals.Length; i++)
                {
                    //Instantiate goal holder
                    var goalHold = Instantiate(goalPrefab, goalHolder);

                    var spr = lvl.goals[i].goalSprite;
                    var count = lvl.goals[i].goalCount;

                    //Assign goals                    
                    goalHold.transform.GetChild(0).GetComponent<Image>().sprite = spr;
                    goalHold.transform.GetChild(1).GetComponent<TMP_Text>().text = count.ToString();

                    //Assign for changing
                    goalCounts[i] = count;
                    goalImages[i] = goalHold.transform.GetChild(0).GetComponent<Image>();
                    goalCountTexts[i] = goalHold.transform.GetChild(1).gameObject;
                }
            }
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
        public void CheckObjectives(GameObject piece)
        {
            if (lvl != null)
            {
                if(lvl.goals.Length > 0)
                {
                    for (int i = 0; i < lvl.goals.Length; i++)
                    {
                        if (piece.CompareTag(lvl.goals[i].tag))//Currently works for only 1 objective
                        {
                            if (goalCounts[i] > 0)
                            {
                                goalCounts[i]--;                                
                                goalCountTexts[i].GetComponent<TMP_Text>().text = $"{goalCounts[i]}";
                            }
                            if (CheckGoals())
                            {
                                GameData.Instance.IncreaseLastLevel(board);
                                SceneManager.LoadScene("Menu");
                            }
                        }
                    }
                }
            }
            
        }
        private bool CheckGoals()
        {
            foreach (var goal in goalCounts)
            {
                if (goal > 0) return false;
            }
            return true;
        }
    }
}