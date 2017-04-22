/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Waypoint : MonoBehaviour
{
    public Vector3 Position
    {
        get { return transform.position; }
    }

#if UNITY_EDITOR
    public void OnDrawGizmosSelected()
    {
        Handles.color = Color.blue;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, 1f);
        Handles.Label(transform.position, name);
    }
#endif
}
