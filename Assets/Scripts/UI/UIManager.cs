/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager> 
{
    public Canvas canvas;

    void Awake()
    {
        foreach(Transform t in transform)
        {
            t.gameObject.SetActive(true);
        }
    }
}
