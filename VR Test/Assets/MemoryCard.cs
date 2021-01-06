using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryCard : MonoBehaviour
{
    public bool turned = false;
    public Color color;

    public void TurnCard()
    {
        turned = true;
        GetComponent<Renderer>().material.color = color;
    }
}
