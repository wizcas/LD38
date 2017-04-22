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
    public Route route;

    private NavMeshAgent _agent;
    private float _nextCheckTime;
    private float _volecityCheckInterval = .1f;
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        MoveAlongRoute();
    }

    private void MoveAlongRoute()
    {
        if (route == null) return;
        Debug.LogFormat("{0}: to next waypoint", name);
        var wp = route.Next();
        if (wp == null) return;
        _agent.destination = wp.Position;
    }

    void Update()
    {
        if (Time.time > _nextCheckTime && _agent.velocity == Vector3.zero)
        {
            _nextCheckTime = Time.time + _volecityCheckInterval;
            Debug.Log("_agent is stopped");
            MoveAlongRoute();
        }
    }
}
