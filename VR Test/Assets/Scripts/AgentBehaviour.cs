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
    public Rig gazeRigTable;
    public Rig pointRig;

    public Transform goal1;
    public Transform goal2;
    public Transform goal3;
    
    public AudioClip welcome;
    public AudioClip thankYou;
    public AudioClip overThere;
    public AudioClip letsPlay;

    public Collider otherCollider;
    
    private Animator _agentAnimator;
    private NavMeshAgent _agent;
    private AudioSource _audioSource;
    private Collider _agentCollider;
    
    private bool _started = false;
    private int _animStatus = 0;
    private bool _corIsRunning = false;
    

    #endregion
    
    // Start is called before the first frame update
    private void Start()
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
                        otherCollider.enabled = true;
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
        //_agent.updateRotation = true;
    }
    
    //before animStatus
    private IEnumerator StartHello()
    {
    //welcome and start looking at user
        _corIsRunning = true;
        StartCoroutine(LerpGazeWeight(1, 0, 0.5f));
        
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
        StartCoroutine(LerpGazeWeight(0.5f, 0.5f, 0f));
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
        StartCoroutine(LerpGazeWeight(1, 0f, 0.4f));
        _agentAnimator.SetBool("Walk", true);
        
        //TODO maybe edit goal1 to be in front of the user no matter where it stands
        MoveToGoal(goal1);
        
        yield return new WaitUntil(GoalReached);
        _agentAnimator.SetBool("Walk", false);
       
        _animStatus = 2;
        _corIsRunning = false;

    }
    
    //animStatus = 2
    private IEnumerator ThankYouAndLetsPlay()
    {
        _corIsRunning = true;
        _audioSource.PlayOneShot(thankYou);
        _agentAnimator.SetTrigger("TalkBothHandsUp");
        StartCoroutine(LerpGazeWeight(1, 0.4f, 0.6f));
        
        yield return new WaitWhile(() => _audioSource.isPlaying);
        
        _audioSource.PlayOneShot(overThere);
        _agentAnimator.SetTrigger("TalkRightToLeft");
        StartCoroutine(LerpGazeWeight(1, 0.6f, 0f));
        
        StartCoroutine(LerpGazeTableWeight(0.5f, 0, 0.2f));
        
        yield return new WaitWhile(() => _audioSource.isPlaying);
        StartCoroutine(LerpGazeTableWeight(0.5f, 0.2f, 0));
        
        
        _animStatus = 3;
        _corIsRunning = false;
       
    }
    
    //animStatus = 3
    private IEnumerator WalkToTable()
    {
        _corIsRunning = true;
        
        _agentAnimator.SetBool("Walk", true);
        MoveToGoal(goal2);
        StartCoroutine(LerpGazeWeight(3, 0, 0.3f));
        yield return new WaitUntil(GoalReached);
        
        StartCoroutine(LerpLookRotation(2,transform.rotation,goal3.rotation));
        MoveToGoal(goal3);
        yield return new WaitUntil(GoalReached);
        _agentAnimator.SetBool("Walk", false);
        
        _animStatus = 4;
        _corIsRunning = false;

    }
    

    //animStatus = 4
    private IEnumerator ConversationAtTable()
    {
        _corIsRunning = true;

        if (collided)
        {
            _audioSource.PlayOneShot(letsPlay);
            _agentAnimator.SetTrigger("TalkOneHandUp");
            yield return new WaitForSeconds(0.3f);
        
            _agentAnimator.SetTrigger("TalkHipLeftToRight");
            yield return new WaitWhile(() =>_audioSource.isPlaying);

            _animStatus = 5;
        }
        
        _corIsRunning = false;
    }
    
    
    private IEnumerator SwitchLegIdle()
    {
        yield return new WaitForSeconds(180);
        _agentAnimator.SetTrigger("Idle2");
    }
    
    #endregion

    #region Lerps

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
    private IEnumerator LerpGazeWeight(float lerpduration, float startValue, float endValue)
    {
        float timeElapsed = 0;

        while (timeElapsed< lerpduration)
        {
            gazeRig.weight = Mathf.Lerp(startValue, endValue, timeElapsed / lerpduration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        gazeRig.weight = endValue;
    }
    
    private IEnumerator LerpGazeTableWeight(float lerpduration, float startValue, float endValue)
    {
        float timeElapsed = 0;

        while (timeElapsed< lerpduration)
        {
            gazeRigTable.weight = Mathf.Lerp(startValue, endValue, timeElapsed / lerpduration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        gazeRigTable.weight = endValue;
    }
    
    
    public IEnumerator LerpPointWeight(float lerpduration, float startValue, float endValue)
    {
        float timeElapsed = 0;

        while (timeElapsed< lerpduration)
        {
            pointRig.weight = Mathf.Lerp(startValue, endValue, timeElapsed / lerpduration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        pointRig.weight = endValue;
    }
    
    private IEnumerator LerpLookRotation(float lerpduration, Quaternion startValue, Quaternion endValue)
    {
        _agent.updateRotation = false;
        float timeElapsed = 0;

        while (timeElapsed< lerpduration)
        {
            _agent.transform.rotation = Quaternion.Slerp(startValue, endValue, timeElapsed / lerpduration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        _agent.transform.rotation = endValue;
        
    }

    #endregion
    
    
    /// <summary>
    ///Method for NavMeshAgent to move towards a certain goal
    /// </summary>
    /// <param name="goal">Transform of the goal to move to</param>
    private void MoveToGoal(Transform goal)
    {
        _agent.SetDestination(goal.position);
        
    }


    public IEnumerator pointToWorld()
    {
        memory.corIsRunning = true;
        
        if (pointRig.weight.Equals(0))
        {
            StartCoroutine(LerpPointWeight(1, 0, 1f));
            _agentAnimator.SetTrigger("Point");
        }
        
        yield return new WaitForSeconds(1.5f);
        memory.corIsRunning = false;

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

    private bool collided = false;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.Equals("ForwardDirection"))
        {

            collided = true;
            //start conversation
        }
    }
}
