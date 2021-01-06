using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = System.Random;

public class Memory : MonoBehaviour
{
    public List<MemoryCard> memoryCards = new List<MemoryCard>(20);

    public List<Color> colors = new List<Color>(){Color.blue, Color.red, Color.yellow, Color.cyan, Color.gray, Color.green, Color.magenta, Color.black, new Color(), new Color()};
    

    public void SetColors()
    {
        Shuffle(memoryCards);

        for (int i = 0; i < memoryCards.Count; i++)
        {
            memoryCards[i].color = colors[i % 10];
        }
    }

    

    
    
    //Fisher Yates Shuffle
    private static Random rng = new Random();
    public static void Shuffle<T>(IList<T> list)  
    {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }

    public void Start()
    {
        SetColors();
    }
}
