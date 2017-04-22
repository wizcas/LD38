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
        if (Physics.Raycast(ray, out hit, raycastDistance,
            Layers.GetLayerMasks(Layers.Environment, Layers.Treasure, Layers.Stash)
            ))
        {
            Debug.LogFormat("hit on: {0}", hit.collider.name);
            var hitLayer = LayerMask.LayerToName(hit.collider.gameObject.layer);
            switch (hitLayer)
            {
                case Layers.Environment:
                    Messenger.Broadcast("MoveTo", hit.point);
                    break;
                case Layers.Treasure:
                    var treasure = hit.collider.GetComponent<Treasure>();
                    if (treasure == null)
                    {
                        Debug.LogErrorFormat("'{0}' is not a Treasure", hit.collider.name);
                    }
                    else
                    {
                        Messenger.Broadcast("PickUp", treasure);
                    }
                    break;
                case Layers.Stash:
                    var stash = hit.collider.GetComponent<Stash>();
                    if(stash == null)
                    {
                        Debug.LogErrorFormat("'{0}' is not a Stash", hit.collider.name);
                    }
                    else
                    {
                        Messenger.Broadcast("Store", stash);
                    }
                    break;
            }
        }
    }
}

public static class Layers
{
    public const string Environment = "Environment";
    public const string Treasure = "Treasure";
    public const string Stash = "Stash";

    public static int GetLayerMask(string layerName)
    {
        return 1 << LayerMask.NameToLayer(layerName);
    }

    public static int GetLayerMasks(params string[] layerNames)
    {
        var ret = 0;
        foreach (var layerName in layerNames)
        {
            ret |= GetLayerMask(layerName);
        }
        return ret;
    }
}
