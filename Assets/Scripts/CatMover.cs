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

    public float speed = 2f;

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
}
