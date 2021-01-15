using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = System.Random;

public class Memory : MonoBehaviour
{
    public List<MemoryCard> memoryCards = new List<MemoryCard>(20);

    public List<Color> colors = new List<Color>();

    public List<MemoryCard> currentSelected = new List<MemoryCard>(2);

    public int turnedCards;
    public int pairedCards;

    public Transform memoryTarget;
    public AgentBehaviour agent;

    public Text txt;
    public Text txt2;
    public Text txt3;
    public Text txt4;

    //0 = User 1 = Agent
    private bool UserTurn = true;
    
    private void Update()
    {
        
        //NEU

        turnedCards = 0;
        
//        if (currentSelected.Count >= 1)
//            currentSelected.Clear();

        if (UserTurn)
        {

            if(currentSelected.Count < 2)
            {
                //Clear before adding back in
                currentSelected.Clear();
                
                foreach (var memoryCard in memoryCards)
                {
                    if (memoryCard.turned && !memoryCard.paired)
                    {
                        currentSelected.Add(memoryCard);
                        memoryCard.gameObject.GetComponent<EventTrigger>().enabled = false;
                        //turnedCards ++;
                    }
                    else
                    {
                        memoryCard.gameObject.GetComponent<EventTrigger>().enabled = true;
                    }
                }
            }
            

        }
        else
        {
            foreach (var memoryCard in memoryCards)
            {
                memoryCard.gameObject.GetComponent<EventTrigger>().enabled = false;
            }
            
            PickCards();
        }
        
        if (currentSelected.Count == 2)
        {
            foreach (var memoryCard in memoryCards)
            {
                memoryCard.gameObject.GetComponent<EventTrigger>().enabled = false;
            }

            StartCoroutine(WaitCompare(2));

        }
        
        
        //ALT
        
        //txt.text = currentSelected.Count.ToString();
        
//        if (currentSelected.Count >= 1)
//            currentSelected.Clear();
        
        // txt2.text = currentSelected.Count.ToString();
        
        
//        foreach (var memoryCard in memoryCards.Where(memoryCard => (memoryCard.turned && (!memoryCard.paired))))
//        {
//            currentSelected.Add(memoryCard);
//            turnedCards ++;
//        }
        
        //txt3.text = currentSelected.Count.ToString();
        

//        if (currentSelected.Count == 2)
//        {
//            StartCoroutine(WaitCompare(2));
//        }
        
        //txt4.text = currentSelected.Count.ToString();
        
        
//        if (UserTurn)
//        {
            //wenn nur eine umgedreht ist, darf weitere aufgedeckt werden, ansonsten auf vergleich warten im nächsten Frame
//            if (turnedCards % 2 == 0 && turnedCards != 0)
//            {
//                foreach (var memoryCard in memoryCards)
//                {
//                    memoryCard.GetComponent<EventTrigger>().enabled = false;
//                }
//            }
//            else
//            {
//                foreach (var memoryCard in memoryCards.Where(memoryCard => !memoryCard.paired))
//                {
//                    memoryCard.GetComponent<EventTrigger>().enabled = true;
//                }
//            }
            
//        }
//        else 
//        {
//            
//            pickCard();
//            
//        }
        
    }

    private IEnumerator WaitCompare(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        
        if (CompareSelected())
        {
            foreach (MemoryCard memoryCard in currentSelected)
            {
                memoryCard.paired = true;
                pairedCards ++;
            }

        }else
        {
            TurnBack();
            UserTurn = !UserTurn;
        }
        currentSelected.Clear();
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
    private void PickCards()
    {
        while (currentSelected.Count < 2)
        {
            var rdmNumber = UnityEngine.Random.Range(0, 19);
            var selectedCard = memoryCards[rdmNumber];

            if (!selectedCard.turned)
            {
                
                selectedCard.TurnCard();
                currentSelected.Add(selectedCard);
                memoryTarget.position = selectedCard.gameObject.transform.position;
                StartCoroutine(agent.pointToWorld());
                
            }
            
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
            turnedCards -= 1;
        }
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
