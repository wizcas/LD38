/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StashedItem : MonoBehaviour 
{
    [SerializeField]
    private Image _imgIcon;

    public void Init(Treasure treasure)
    {
        _imgIcon.sprite = treasure.icon;
    }
}
