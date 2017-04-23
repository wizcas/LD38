/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CatMover : MonoBehaviour
{
    [SerializeField]
    private Animator _anim;

    private NavMeshAgent _agent;

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
    }

    public void MoveTo(Vector3 dest)
    {
        // too near to move
        if (VectorUtils.IsAtSamePosition(dest, transform.position))
        {
            return;
        }
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

    void Update()
    {
        if (_agent.isStopped) return;

        var offMesh = _agent.currentOffMeshLinkData;
        var pos = transform.position;
        pos.y = 0f;
        var linkStartPos = offMesh.startPos;
        linkStartPos.y = 0f;

        if (_agent.isOnOffMeshLink && VectorUtils.IsAtSamePosition(pos, linkStartPos))
        {
            Debug.LogFormat("Link activated:\npos: {0}, nextpos: {1}, start pos: {2}, end pos: {3}", pos, _agent.nextPosition, offMesh.startPos, offMesh.endPos);
            _anim.SetTrigger("Jump");
        }
        else
        {
            var isMoving = _agent.velocity.magnitude > .1f;
            _anim.SetBool("Walk", isMoving);
        }
    }
}
