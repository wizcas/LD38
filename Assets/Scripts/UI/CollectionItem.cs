/************************************
** Created by Wizcas (wizcas.me)
************************************/

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionItem : MonoBehaviour 
{
    [SerializeField]
    private Image _imgIcon;
    [SerializeField]
    private Text _txtName;

    public void Init(Treasure treasure)
    {
        _txtName.text = treasure.title;
        _imgIcon.sprite = treasure.icon;
        GetComponent<CanvasGroup>().alpha = 0f;
    }

    public Tween Show()
    {
        return GetComponent<CanvasGroup>().DOFade(1f, 1f);
    }
}
