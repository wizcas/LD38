/************************************
** Created by Wizcas (wizcas.me)
************************************/

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : ItemDescriptor
{
    private Rigidbody _rigidBody;

    private bool _isPickedUp;

    public Sprite icon;

    public bool IsPickedUp
    {
        get { return _isPickedUp; }
        set
        {
            _isPickedUp = value;
            if (_rigidBody != null)
                _rigidBody.useGravity = !value;
        }
    }

    protected override bool CanShowLabel
    {
        get
        {
            return !IsPickedUp;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _rigidBody = GetComponent<Rigidbody>();
    }
}
