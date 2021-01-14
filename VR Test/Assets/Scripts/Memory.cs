using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Random = System.Random;

public class Memory : MonoBehaviour
{
    public List<MemoryCard> memoryCards = new List<MemoryCard>(20);

    public List<Color> colors = new List<Color>(){Color.blue, Color.red, Color.yellow, Color.cyan, Color.gray, Color.green, Color.magenta, Color.black, new Color(), new Color()};

    public List<MemoryCard> currentSelected = new List<MemoryCard>(2);

    public int turnedCards;
    public int pairedCards;

    private void Update()
    {
        foreach (var memoryCard in memoryCards.Where(memoryCard => !memoryCard.paired && memoryCard.turned))
        {
            currentSelected.Add(memoryCard);
            turnedCards += 2;
        }

        if (currentSelected.Count == 2)
        {
            if (CompareSelected())
            {
                foreach (MemoryCard memoryCard in currentSelected)
                {
                    memoryCard.paired = true;
                    pairedCards += 2;
                    memoryCard.gameObject.GetComponent<EventTrigger>().enabled = false;
                }
            }else
            {
                TurnBack();
            }
        }
    }

    private void SetColors()
    {
        Shuffle(memoryCards);

        for (int i = 0; i < memoryCards.Count; i++)
        {
            memoryCards[i].color = colors[i % 10];
        }
    }
    /// <summary>
    /// Shuffle the Cards via Fisher Yates Shuffle
    /// </summary>
    private static Random rng = new Random();
    private static void Shuffle<T>(IList<T> list)  
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

    /// <summary>
    /// Pick two random card and turn it
    /// </summary>
    void pickCard()
    {
        while (true)
        {
            int rdmNumber = UnityEngine.Random.Range(0, 19);
            MemoryCard selectedCard = memoryCards[rdmNumber];

            if (!selectedCard.turned)
            {
                //currentSelected.Add(selectedCard);
                selectedCard.TurnCard();
            }
            else
            {
                continue;
            }

            break;
        }
    }
    
    /// <summary>
    /// Compare two selected Cards
    /// </summary>
    /// <returns>true for Match; false for no match</returns>
    bool CompareSelected()
    {
        return currentSelected[0].color == currentSelected[1].color;
    }

    private void TurnBack()
    {
        foreach (var card in currentSelected)
        {
            card.TurnCard(Color.white);
            card.turned = false;
            turnedCards -= 2;
        }
        currentSelected.Clear();
    }

    public void Start()
    {
        foreach (Transform child in transform)
        {
            memoryCards.Add(child.gameObject.GetComponent<MemoryCard>());
        }
        
        SetColors();
    }
}
