﻿/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class HumanMover : MonoBehaviour
{
    public HumanThought thought;
    public float defaultSpeed = 2f;

    private NavMeshAgent _agent;
    private float _nextCheckTime;
    private float _volecityCheckInterval = .1f;

    private bool isThoughtInit;

    private bool _isGotoTarget;

    public bool IsNearDestination
    {
        get
        {
            return _agent.remainingDistance < _agent.radius;
        }
    }

    public bool IsStandingStill
    {
        get
        {
            return _agent.velocity == Vector3.zero;
        }
    }
    
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = defaultSpeed;
        Messenger.AddListener<HumanThought>(HumanAI.HaveThoughtEvent, HaveThought);
    }

    void Start()
    {

    }

    public void HaveThought(HumanThought t)
    {
        Debug.Log(string.Format("<color=blue>mover has thought: {0}</color>", t), this);
        thought = t;
        isThoughtInit = true;

        if (t.speed >= 0)
        {
            _agent.speed = t.speed;
        }
        else
        {
            _agent.speed = defaultSpeed;
        }
    }

    private void MoveAlongRoute(HumanPatrolThought routeThought)
    {
        if (routeThought == null || routeThought.route == null) return;
        var wp = routeThought.NextWaypoint();
        if (wp == null) return;
        _agent.destination = wp.Position;
    }

    void Update()
    {
        if (Time.time > _nextCheckTime)
        {
            if (_agent.isStopped)
            {
                Resume();
            }
            _nextCheckTime = Time.time + _volecityCheckInterval;
            if (thought is HumanPatrolThought)
            {
                if (isThoughtInit || IsNearDestination)
                    MoveAlongRoute((HumanPatrolThought)thought);
            }
            else if (thought is IThinkGoto)
            {
                Goto(((IThinkGoto)thought).MoveTo);
            }
            else
            {
                Stop();
            }
            isThoughtInit = false;
        }
    }

    private void Stop()
    {
        _agent.isStopped = true;
    }

    private void Resume()
    {
        if (_isGotoTarget)
        {
            _isGotoTarget = false;
        }
        if (_agent.isStopped)
            _agent.isStopped = false;

    }

    private void Goto(Transform target)
    {
        _isGotoTarget = target != null;
        _agent.destination = target.position;
        _agent.isStopped = false;
    }

}
