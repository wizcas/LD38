/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class HumanMover : MonoBehaviour
{
    //public Route route;
    public HumanThought thought;

    private NavMeshAgent _agent;
    private float _nextCheckTime;
    private float _volecityCheckInterval = .1f;

    private bool _isGotoTarget;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        Messenger.AddListener<HumanThought>(HumanAI.HaveThoughtEvent, HaveThought);
    }

    void Start()
    {

    }

    public void HaveThought(HumanThought t)
    {
        Debug.Log(string.Format("<color=blue>mover has thought: {0}</color>", t), this);
        thought = t;
    }

    private void MoveAlongRoute(HumanPatrolThought routeThought)
    {
        if (routeThought == null || routeThought.route == null) return;
        //Debug.LogFormat("{0}: to next waypoint", name);
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
            else
            {
                _nextCheckTime = Time.time + _volecityCheckInterval;
                //Debug.Log("move thought is: " + thought);
                if (thought is HumanPatrolThought)
                {
                    if (_agent.remainingDistance < _agent.radius)
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
            }
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
