using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animFadePanel;
    public Animator animGoalPanel;
    public GameObject panel;

    private void Awake()
    {
        Time.timeScale = 0;
    }
    public void TriggerSlideOut()
    {
        animGoalPanel.SetBool("slideOut",true);
    }
    public void StartAnimation()
    {
        Time.timeScale = 1f;
        animFadePanel.SetBool("fadeIn", true);
    }
    public void ResetAll()
    { 
        animFadePanel.SetBool("fadeIn", false);
        animGoalPanel.SetBool("slideOut", false);
        panel.gameObject.SetActive(false);
    }
    public void DisableObject()
    {
    }
}
