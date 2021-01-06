using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class AgentBehaviour : MonoBehaviour
{
    public Memory memory;
    // Start is called before the first frame update
    void Start()
    {
       
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.Equals("Agent"))
        {
            //start conversation
        }
    }

    void pickCard()
    {

        int rdmNumber = UnityEngine.Random.Range(0, 19);
        MemoryCard selectedCard = memory.memoryCards[rdmNumber];

        if (!selectedCard.turned)
        {
            selectedCard.TurnCard();
        }

    }

    void startConversation()
    {
        
    }
}
