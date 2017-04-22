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
    private Text _txtName;

    public void Init(Treasure desc)
    {
        _txtName.text = desc.title;
    }
}
