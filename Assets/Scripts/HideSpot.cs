/************************************
** Created by Wizcas (wizcas.me)
************************************/

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSpot : MonoBehaviour 
{
    public bool isInUse;
    private SpriteRenderer _maskRenderer;
    private HideSpotEntrance _entrance;

    public Vector3 EntrancePos
    {
        get
        {
            return _entrance.transform.position;
        }
    }

    void Awake()
    {
        _maskRenderer = GetComponentInChildren<SpriteRenderer>();
        _entrance = GetComponentInChildren<HideSpotEntrance>();

        _entrance.Disable();
        var color = _maskRenderer.color;
        color.a = 0f;
        _maskRenderer.color = color;
    }

    public void Enable()
    {
        _maskRenderer.DOFade(1f, .5f);
    }

    public void Disable()
    {
        _maskRenderer.DOFade(0f, .5f);
        _entrance.Disable();
        isInUse = false;
    }

    void OnMouseEnter()
    {
        Enable();
    }

    void OnMouseExit()
    {
        if (isInUse) return;
        Disable();
    }

    void OnMouseDown()
    {
        isInUse = true;
        _entrance.Enable();
        Messenger.Broadcast("ToHideSpot", this);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + -transform.forward);
    }
}
