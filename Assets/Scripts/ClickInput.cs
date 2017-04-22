/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickInput : MonoBehaviour
{
    public float raycastDistance = 20f;
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, raycastDistance, 1 << LayerMask.NameToLayer("Environment")))
        {
            Debug.LogFormat("hit on: {0}", hit.collider.name);
            Messenger.Broadcast<Vector3>("MoveTo", hit.point);
        }
    }
}
