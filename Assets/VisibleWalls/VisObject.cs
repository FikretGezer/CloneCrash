using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisObject : MonoBehaviour
{
    public bool reduceAlpha;
    public float transitionSmoothing = 1f;

    private Material mat;
    private Color reducedColor, fullColor;
    private float current = 0f;
    private void Awake() {
        mat = GetComponent<Renderer>().material;

        var clr = mat.color;
        fullColor = clr;
        reducedColor = new Color(clr.r, clr.g, clr.b, 0.1f);
    }
    private void Update() {
        if(reduceAlpha)
        {
            ColorChange(1f);
        }
        else
        {
            ColorChange(0f);
        }
    }
    private void ColorChange(float target)
    {
        var clr = mat.color;
        current = Mathf.MoveTowards(current, target, Time.deltaTime * transitionSmoothing);
        clr = Color.Lerp(fullColor, reducedColor, current);
        mat.color = clr;
    }
}
