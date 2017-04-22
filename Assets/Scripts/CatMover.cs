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

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    // Use this for initialization
    void Start()
    {
        Messenger.AddListener<Vector3>("MoveTo", MoveTo);
    }

    public void MoveTo(Vector3 dest)
    {
        // too near to move
        if (VectorUtils.IsAtSamePosition(dest, transform.position))
        {
            return;
        }
        _agent.destination = dest;
    }

    void Update()
    {
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
