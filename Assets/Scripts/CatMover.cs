/************************************
** Created by Wizcas (wizcas.me)
************************************/

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CatMover : MonoBehaviour
{
    [SerializeField]
    private CatJumping _catAnim;

    private NavMeshAgent _agent;
    
    private Vector3 _jumpEnd;
    private bool _isJumping;
    private Vector3 _prevDestination;

    private bool isStopped
    {
        get { return _agent.isStopped && !enabled; }
        set
        {
            _agent.isStopped = value;
            enabled = !value;
        }
    }

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        Messenger.AddListener<Vector3>("CatMoveTo", MoveTo);
        Messenger.AddListener("CatStop", Stop);
        Messenger.AddListener("CatResume", Resume);
    }

    // Use this for initialization
    void Start()
    {
        _catAnim.onAnimatorMove += AfterAnim;
    }

    public void MoveTo(Vector3 dest)
    {
        // too near to move
        if (VectorUtils.IsAtSamePosition(dest, transform.position))
        {
            return;
        }
        _prevDestination = dest;
        _agent.destination = dest;
        Resume();
    }

    private void Stop()
    {
        if (isStopped) return;
        isStopped = true;
        _agent.destination = transform.position;
    }

    private void Resume()
    {
        if (!isStopped) return;
        GetComponent<CatAction>().ExitHideSpot();
        isStopped = false;
    }

    void FixedUpdate()
    {
        if (_agent.isStopped) return;

        var offMesh = _agent.currentOffMeshLinkData;
        var pos = transform.position;
        pos.y = 0f;
        var linkStartPos = offMesh.startPos;
        linkStartPos.y = 0f;

        if (!_isJumping)
        {
            if (_agent.isOnOffMeshLink)
            {
                Debug.LogFormat("Link activated:\npos: {0}, nextpos: {1}, start pos: {2}, end pos: {3}", pos, _agent.nextPosition, offMesh.startPos, offMesh.endPos);

                var animTrigger = "Jump";
                if ((offMesh.endPos.y - offMesh.startPos.y) < -.2f)
                    animTrigger = "Drop";

                Debug.LogFormat("anim trigger: {0}, {1}", animTrigger, offMesh.endPos.y - offMesh.startPos.y);
                BeforeJump(offMesh.endPos);
                _catAnim.Anim.SetTrigger(animTrigger);                
            }
            else
            {
                var isMoving = _agent.velocity.magnitude > .1f;
                _agent.updatePosition = true;
                _catAnim.Anim.SetBool("Walk", isMoving);
            }
        }
    }

    void AfterAnim()
    {
        
    }

    void BeforeJump(Vector3 jumpEnd)
    {
        _jumpEnd = jumpEnd;
        _isJumping = true;
        _agent.updatePosition = false;
        //isStopped = true;
        //_agent.destination = transform.position;
    }

    void AfterJump()
    {
        _agent.nextPosition = _jumpEnd;
        _isJumping = false;
        //isStopped = false;
        _agent.destination = _prevDestination;

    }

    void DoJump(float duration)
    {
        DOTween.Sequence()
            .Append(transform.DOMove(_jumpEnd, duration))        
            .OnComplete(()=>
            {
                AfterJump();
            });        
    }
}
