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
    private bool _corIsRunning = false;

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
            StartCoroutine(StartHello());
            
        }else if (_started)
        {
            if (!_corIsRunning)
            {
                switch (_animStatus)
                {
                    case 0:
                    
                        StartCoroutine(StandUp());

                        break;
                    case 1:
                        StartCoroutine( WalkTowardsUser());
                        break;
                    case 2:
                        StartCoroutine(ThankYouAndLetsPlay());

                        break;
                    case 3:
                        StartCoroutine(WalkToTable());
                        break;
                    case 4:
                        StartCoroutine(ConversationAtTable());
                        break;
                    default:
                        break;
                }
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
    private IEnumerator StartHello()
    {
    //welcome and start looking at user
        _corIsRunning = true;
        StartCoroutine(LerpGazeWeight(1, 0, 0.7f, gazeRig.weight));
        //TODO Play Audio or insert Text "Hallo"
        _audioSource.PlayOneShot(welcome);
        
        yield return new WaitWhile(() =>_audioSource.isPlaying);
        
        _started = true;
        _corIsRunning = false;
    }
    
    //animStatus = 0
    private IEnumerator StandUp()
    {
        _corIsRunning = true;
        //stand up and stop looking
        StartCoroutine(LerpGazeWeight(1, 0.7f, 0f, gazeRig.weight));
        _agentAnimator.SetTrigger("StandUp");

        yield return new WaitUntil(() =>_agentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));

        
            _animStatus = 1;
            _corIsRunning = false;

    }
    
    //animStatus = 1
    private IEnumerator WalkTowardsUser()
    {
        _corIsRunning = true;
        //walk towards user
        StartCoroutine(LerpGazeWeight(1, 0f, 0.4f, gazeRig.weight));
        _agentAnimator.SetBool("Walk", true);
        MoveToGoal(goal1);
        //TODO change rotation if necessary
        yield return new WaitUntil(GoalReached);
        _agentAnimator.SetBool("Walk", false);
       
        _animStatus = 2;
        _corIsRunning = false;

    }

    /// <summary>
    ///Check whether the goal of the NavMeshAgent was reached
    /// </summary>
    /// <returns>goalReached </returns>
    private bool GoalReached()
    {
        var reached = false;
        
        if(!_agent.pathPending) {
            if(_agent.remainingDistance <= _agent.stoppingDistance) {
                if(!_agent.hasPath || _agent.velocity.sqrMagnitude.Equals(0f))
                {
                    reached = true;
                }
            }
        }

        return reached;
    }
    
    //animStatus = 2
    private IEnumerator ThankYouAndLetsPlay()
    {
        _corIsRunning = true;
        yield return new WaitForSeconds(2);
        //TODO Conversation "Danke, dass du beim Test mitmachst."
        _audioSource.PlayOneShot(thankYou);
        StartCoroutine(LerpGazeWeight(1, 0.4f, 0.7f, gazeRig.weight));
        _agentAnimator.SetTrigger("TalkBothHandsUp");
        yield return new WaitForSeconds(3);
        //yield return new WaitWhile(() => _audioSource.isPlaying);
        //yield return new WaitWhile(() => _agentAnimator.GetCurrentAnimatorStateInfo(_agentAnimator.GetLayerIndex("Talking")).IsName("Talk-both-hands-up"));
        //yield return new WaitUntil(() => _agentAnimator.GetCurrentAnimatorStateInfo(2).IsName("NotTalking"));
        
        
        yield return new WaitForSeconds(2);
        //TODO Conversation "Komm doch erstmal mit rüber, wir spielen eine Runde Memory."
        
        _audioSource.PlayOneShot(letsPlay);
        StartCoroutine(LerpGazeWeight(2, 0.7f, 0f, gazeRig.weight));
        //TODO anderes Rig weight > 0
        //TODO Change Gaze towards Table/Memory and Point towards it
        _agentAnimator.SetTrigger("TalkRightToLeft");
        //yield return new WaitWhile(() => _audioSource.isPlaying);
        yield return new WaitForSeconds(6);
        //yield return new WaitWhile(() => _agentAnimator.GetCurrentAnimatorStateInfo(_agentAnimator.GetLayerIndex("Talking")).IsName("talk-hip-right-to-left"));
        //if (!_audioSource.isPlaying)
        //{
            //if (_agentAnimator.GetCurrentAnimatorStateInfo(_agentAnimator.GetLayerIndex("Talking")).IsName("Not Talking"))
            //{
                _animStatus = 3;
                _corIsRunning = false;
            //}
        //}
            
        

    }
    
    private IEnumerator WalkToTable()
    {
        _corIsRunning = true;
        _agentAnimator.SetBool("Walk", true);
        MoveToGoal(goal2);
        StartCoroutine(LerpGazeWeight(3, 0, 0.7f, 0.05f));
        yield return new WaitUntil(GoalReached);
        _agentAnimator.SetBool("Walk", false);
        
        
        _animStatus = 4;
        _corIsRunning = false;

    }
    
    
    private IEnumerator SwitchLegIdle()
    {
        yield return new WaitForSeconds(60);
        _agentAnimator.SetTrigger("Idle2");
    }
    
    #endregion
    
    

    

    IEnumerator ConversationAtTable()
    {
        _corIsRunning = true;
        //TODO Conversation "Super, dann fangen wir mal an."
        _audioSource.PlayOneShot(thankYou);
        _agentAnimator.SetTrigger("TalkOneHandUp");

        yield return new WaitForSeconds(2);
        _audioSource.PlayOneShot(thankYou);
        //TODO Conversation "Mach den ersten Zug und versuch zwei gleichfarbige Karten aufzudecken"
        _agentAnimator.SetTrigger("HipLeftToRight");
        yield return new WaitForSeconds(5);

        _animStatus = 5;
        _corIsRunning = false;
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
            yield return valueToLerp;
        }

        yield return valueToLerp = endValue;
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
