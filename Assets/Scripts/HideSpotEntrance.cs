/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSpotEntrance : MonoBehaviour
{
    public bool isHidingIn;

    private BoxCollider entranceCollider
    {
        get { return GetComponent<BoxCollider>(); }
    }

    public void Enable()
    {
        entranceCollider.enabled = true;
    }

    public void Disable()
    {
        entranceCollider.enabled = false;
        isHidingIn = false;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (isHidingIn) return;
        
        var cat = collider.GetComponent<CatAction>();
        if (cat == null) return;

        Messenger.Broadcast("EnterHideSpot");
        isHidingIn = true;
    }

    private CatAction CheckCat(Collider collider)
    {
        return collider.GetComponent<CatAction>();
    }

}
