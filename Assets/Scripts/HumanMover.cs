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

    private Transform _moveTarget;
    private bool _isChasingCat;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        //Messenger.AddListener("HumanStop", Stop);
        //Messenger.AddListener("HumanResume", Resume);
        //Messenger.AddListener<Transform>("HumanChase", ChaseCat);

        Messenger.AddListener<HumanThought>(HumanAI.HaveThoughtEvent, HaveThought);
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
        if (Time.time > _nextCheckTime && !_agent.isStopped)
        {
            _nextCheckTime = Time.time + _volecityCheckInterval;
            //Debug.Log("_agent is stopped");
            if (thought is HumanPatrolThought)
            {
                if (_agent.remainingDistance < _agent.radius)
                    MoveAlongRoute((HumanPatrolThought)thought);
            }
            else if(thought is IThinkGoto)
            {
                //_agent.destination = _moveTarget.position;
                _agent.destination = ((IThinkGoto)thought).MoveTo.position;
            }
        }
    }

    //private void Stop()
    //{
    //    _agent.isStopped = true;
    //}

    //private void Resume()
    //{
    //    if (_isChasingCat)
    //    {
    //        _moveTarget = null;
    //        _isChasingCat = false;
    //    }
    //    if (_agent.isStopped)
    //        _agent.isStopped = false;

    //}

    //private void ChaseCat(Transform cat)
    //{
    //    _isChasingCat = cat != null;
    //    _moveTarget = cat;
    //    _agent.isStopped = false;
    //}

    private void HaveThought(HumanThought t)
    {
        thought = t;
    }
}
