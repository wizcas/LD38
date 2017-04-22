/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour 
{
    public Transform target;
    public float panSpeed = 3f;

    private Vector3 _offset;

    void Start()
    {
        _offset = transform.position - target.position;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + _offset, panSpeed * Time.deltaTime);
    }
}
