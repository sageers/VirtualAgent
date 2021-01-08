using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryCard : MonoBehaviour
{
    public bool turned = false;
    public Color color;
    private Color _lerpedColor;
    public Material materialToChange;

    public void Start()
    {
        materialToChange = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        
    }

    public void TurnCard()
    {
        turned = true;
        StartCoroutine(LerpColor(color, 1));
    }
    public void TurnCard(Color endColor)
    {
        turned = true;
        StartCoroutine(LerpColor(endColor, 1));
    }

    IEnumerator LerpColor(Color endValue, float duration)
    {
        float time = 0;
        Color startValue = materialToChange.color;
        while (time<duration)
        {
            materialToChange.color = Color.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        materialToChange.color = endValue;
    }
}
