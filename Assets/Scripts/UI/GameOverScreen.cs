/************************************
** Created by Wizcas (wizcas.me)
************************************/

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOverScreen : UIBehaviour 
{
	protected override void Awake()
    {
        Messenger.AddListener("GameOver", GameOver);
    }

    protected override void Start()
    {
        GetComponent<CanvasGroup>().alpha = 0f;
        gameObject.SetActive(false);
    }

    private void GameOver()
    {
        gameObject.SetActive(true);
        GetComponent<CanvasGroup>().DOFade(1f, 1f);
    }
}
