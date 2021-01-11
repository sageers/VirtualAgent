using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using Random = System.Random;


public class AgentBehaviour : MonoBehaviour
{
    #region Variables

    public Memory memory;
    public Rig gazeRig;

    public Transform goal1;
    public Transform goal2;

    public AudioClip welcome;
    public AudioClip thankYou;
    public AudioClip letsPlay;
    
    private Animator _agentAnimator;
    private NavMeshAgent _agent;
    private AudioSource _audioSource;
    private Collider _agentCollider;
    
    private bool _started = false;
    private int _animStatus = 0;

    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        _agentAnimator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();
        
        StartSit();
    }

    private void Update()
    {
        //Start Conversation if any Input by the user is given
        if (!_started && OVRInput.GetDown(OVRInput.Button.Any))
        {
            StartHello();
        }else if (_started)
        {
            switch (_animStatus)
            {
                case 0:
                    
                    StandUp();

                    break;
                case 1:
                    WalkTowardsUser();
                    break;
                case 2:
                    ThankYouAndLetsPlay();

                    break;
                case 3:
                    WalkToTable();
                    break;
                
                default:
                    break;
            }
        }

        if (_agentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            StartCoroutine(SwitchLegIdle());
        }
        
    }

    

    #region AgentAnimation

    //Start of app
    void StartSit()
    {
        //Sit down on couch dont look at user
        gazeRig.weight = 0;
        _agentAnimator.SetTrigger("SitDown");
    }
    
    //before animStatus
    private void StartHello()
    {
    //welcome and start looking at user
        StartCoroutine(LerpGazeWeight(1, 0, 0.7f, gazeRig.weight));
        //TODO Play Audio or insert Text "Hallo"
        _audioSource.PlayOneShot(welcome);
        _started = true;
    }
    
    //animStatus = 0
    private void StandUp()
    {
        if (!_audioSource.isPlaying)
        {
            //stand up and stop looking
            StartCoroutine(LerpGazeWeight(1, 0.7f, 0f, gazeRig.weight));
            _agentAnimator.SetTrigger("StandUp");

            if (_agentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                _animStatus = 1;
            }
        }
    }
    
    //animStatus = 1
    private void WalkTowardsUser()
    {
        //walk towards user
        StartCoroutine(LerpGazeWeight(1, 0f, 0.4f, gazeRig.weight));
        _agentAnimator.SetBool("Walk", true);
        MoveToGoal(goal1);
        //TODO change rotation if necessary
        if (_agent.remainingDistance.Equals(0))
        {
            _animStatus = 2;
        }
    }
    
    //animStatus = 2
    private void ThankYouAndLetsPlay()
    {
        _agentAnimator.SetBool("Walk", false);
        StartCoroutine(LerpGazeWeight(1, 0.4f, 0.7f, gazeRig.weight));
        _agentAnimator.SetTrigger("TalkBothHandsUp");
        _audioSource.PlayOneShot(thankYou);
        //TODO Conversation "Danke, dass du beim Test mitmachst."
        if (!_audioSource.isPlaying)
        {
            _audioSource.PlayOneShot(letsPlay);
            StartCoroutine(LerpGazeWeight(2, 0.7f, 0f, gazeRig.weight));
            //TODO anderes Rig weight > 0
            //TODO Conversation "Komm doch erstmal mit rüber, wir spielen eine Runde Memory."
            //TODO Change Gaze towards Table/Memory and Point towards it
        }

        if (_agentAnimator.GetCurrentAnimatorStateInfo(2).IsName("Not Talking"))
        {
            _animStatus = 3;
        }
    }
    
    private void WalkToTable()
    {
        MoveToGoal(goal2);
        StartCoroutine(LerpGazeWeight(3, 0, 0.7f, 0.05f));
        if (_agent.remainingDistance.Equals(0))
        {
            
            _animStatus = 4;
        }
    }
    
    
    private IEnumerator SwitchLegIdle()
    {
        yield return new WaitForSeconds(60);
        _agentAnimator.SetTrigger("Idle2");
    }
    
    #endregion
    
    

    

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
    /// <param name="valueToLerp">which variable should get the lerped values</param>
    /// <returns></returns>
    /// 
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
    
    /// <summary>
    ///Method for NavMeshAgent to move towards a certain goal
    /// </summary>
    /// <param name="goal">Transform of the goal to move to</param>
    public void MoveToGoal(Transform goal)
    {
        _agent.destination = goal.position;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.Equals("OVRPlayerController"))
        {
            
            //start conversation
        }
    }
}
