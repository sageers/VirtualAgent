using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Random = System.Random;

public class AgentBehaviour : MonoBehaviour
{
    public Memory memory;

    public MultiAimConstraint gazeConstraint;
    private Animator _agentAnimator;

   
    
    // Start is called before the first frame update
    void Start()
    {

        _agentAnimator = GetComponent<Animator>();
        StartCoroutine(StartConversation());
        

    }

    // Update is called once per frame
    void Update()
    {


    }
    
    IEnumerator StartConversation()
    {
        
        gazeConstraint.weight = 0;
        _agentAnimator.SetTrigger("SitDown");
        yield return new WaitForSeconds(15);

        StartCoroutine(LerpGazeWeight(3, 0, 0.7f, 0.05f));
        
        //Play Audio or insert Text "Hallo"
        
        yield return new WaitForSeconds(2);
        
        _agentAnimator.SetTrigger("StandUp");
        
        //TODO walk towards user
        
        StartCoroutine(LerpGazeWeight(2, 0.7f, 0.1f, 0.05f));
        
        //TODO Conversation "Danke, dass du beim Test mitmachst."
        _agentAnimator.SetTrigger("TalkBothHandsUp");
        
        //TODO Conversation "Komm doch erstmal mit rüber, wir spielen eine Runde Memory."
        
        //TODO Change Gaze towards Table/Memory and Point towards it
        
    }

    IEnumerator WalkToTable()
    {
        yield return new WaitForSeconds(1);
        //TODO Walk towards table
        
        StartCoroutine(LerpGazeWeight(3, 0, 0.7f, 0.05f));
        
    }

    IEnumerator ConversationAtTable()
    {
        //TODO Conversation "Super, dann fangen wir mal an."
        _agentAnimator.SetTrigger("TalkOneHandUp");
        
        //TODO Conversation "Mach den ersten Zug und versuch zwei gleichfarbige Karten aufzudecken"
        _agentAnimator.SetTrigger("HipLeftToRight");
        
        yield return new WaitForSeconds(1);
    }
    


    /// <summary>
    /// Coroutine to Lerp the Weight for the Gaze Animation Rigging
    /// 
    /// https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/#:~:text=What%20is%20Lerp%20in%20Unity,over%20a%20period%20of%20time.
    /// </summary>
    /// <param name="lerpduration">Duration of the Transition</param>
    /// <param name="startValue">Start Weight for the Gaze Animation Rigging</param>
    /// <param name="endValue">End Weight for the Gaze Animation Rigging</param>
    /// <param name="valueToLerp">Steps for Lerp Progress</param>
    /// <returns></returns>
    IEnumerator LerpGazeWeight(float lerpduration, float startValue, float endValue, float valueToLerp)
    {
        float timeElapsed = 0;

        while (timeElapsed< lerpduration)
        {
            valueToLerp = Mathf.Lerp(startValue, endValue, timeElapsed / lerpduration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        valueToLerp = endValue;
    }
    
    
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.Equals("Agent"))
        {
            //start conversation
        }
    }
}
