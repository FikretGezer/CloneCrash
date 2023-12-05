using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XpBarController : MonoBehaviour
{
    public TMP_Text _barFillAmount;
    public Image _xpBar;
    private void Update() {
        _barFillAmount.text = ((int)(_xpBar.fillAmount * 100)).ToString();
    }
}
