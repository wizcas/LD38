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
    private const float EPSILON = .0025f;

    public float speed = 2f;

    private NavMeshAgent _agent;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    // Use this for initialization
    void Start()
    {
        Messenger.AddListener<Vector3>("MoveTo", SetDestination);
    }

    public void SetDestination(Vector3 dest)
    {
        // too near to move
        if (IsAtSamePosition(dest, transform.position))
        {
            return;
        }
        _agent.destination = dest;
    }

    private float SqrDistance(Vector3 a, Vector3 b)
    {
        return (a - b).sqrMagnitude;
    }

    private bool IsAtSamePosition(Vector3 a, Vector3 b)
    {
        return SqrDistance(a, b) <= EPSILON;
    }    
}
