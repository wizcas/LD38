/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(OffMeshLink))]
public class JumpSpot : MonoBehaviour 
{
    void Awake()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        if(renderers != null)
        {
            foreach(var ren in renderers)
            {
                ren.enabled = false;
            }
        }
    }

	void OnDrawGizmos()
    {
        //DrawEndpointLabel(_link.startTransform);
        //DrawEndpointLabel(_link.endTransform);
    }

    void DrawEndpointLabel(Transform endpoint)
    {
        string text = string.Format("{0}/{1}/{2}", transform.parent.name, name, endpoint.name);
        var pos = endpoint.position;
        GizmoHelper.Label(pos, text, GizmoHelper.LabelSize.Small);
    }
}
