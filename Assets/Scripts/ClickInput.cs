/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickInput : MonoBehaviour
{
    public float raycastDistance = 20f;
    public float fxOffsetY = .5f;

    [SerializeField]
    private ParticleSystem _clickFx;

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    private void PlayClickFx(Vector3 pos)
    {
        pos += Vector3.up * fxOffsetY;
        _clickFx.transform.position = pos;
        _clickFx.Play();
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
                    PlayClickFx(hit.point);
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