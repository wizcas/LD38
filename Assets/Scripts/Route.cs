/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[ExecuteInEditMode]
public class Route : MonoBehaviour 
{
    public enum Mode
    {
        OneWay,
        PingPong,
        Loop
    }

    public Mode mode;

    [SerializeField]
    private Waypoint[] _waypoints;
    //private int _currentIndex;
    //private int _direction = 1;

    void Awake()
    {
        UpdateWaypoints();
    }
    
    [ContextMenu("Refresh Waypoints")]
    private void UpdateWaypoints()
    {
        _waypoints = GetComponentsInChildren<Waypoint>();        
    }

    public Waypoint Next(ref int curIndex, ref int direction)
    {
        curIndex += direction;
        //Debug.LogFormat("route index: {0}", curIndex);
        if (curIndex >= _waypoints.Length || curIndex < 0)
        {
            switch (mode)
            {
                case Mode.OneWay:
                    return null;
                case Mode.PingPong:
                    direction *= -1;
                    curIndex += direction * 2;
                    break;
                case Mode.Loop:
                    if(direction > 0)
                    {
                        curIndex = 0;
                    }
                    else
                    {
                        curIndex = _waypoints.Length - 1;
                    }
                    break;
            }
        }
        return _waypoints[curIndex];
    }

#if UNITY_EDITOR
    void OnTransformChildrenChanged()
    {
        UpdateWaypoints();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < _waypoints.Length; i++)
        {
            var waypoint = _waypoints[i];
            waypoint.OnDrawGizmosSelected();
            if(i > 0)
            {
                Gizmos.DrawLine(waypoint.transform.position, _waypoints[i - 1].transform.position);
            }
        }
#endif
    }
}
