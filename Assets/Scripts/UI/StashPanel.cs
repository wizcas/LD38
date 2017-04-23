/************************************
** Created by Wizcas (wizcas.me)
************************************/

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StashPanel : MonoBehaviour
{
    [SerializeField]
    private StashedItem _itemPrefab;

    [SerializeField]
    private Text _tip;

    void Start()
    {
        _tip.gameObject.SetActive(false);
        _tip.transform.localScale = Vector3.zero;
    }

    public void Clear()
    {
        var items = GetComponentsInChildren<StashedItem>();
        foreach (var item in items)
        {
            DestroyImmediate(item.gameObject);
        }
    }

    public void UpdateItems(Stash stash)
    {
        Clear();
        foreach (var treasure in stash.inventory)
        {
            InstantiateItem(treasure);
        }
        _tip.transform.DOScale(1f, 2f).SetEase(Ease.OutElastic)
            .OnStart(() =>
            {
                _tip.gameObject.SetActive(true);
            })
            .OnComplete(() =>
            {
                _tip.gameObject.SetActive(false);
                _tip.transform.localScale = Vector3.zero;
            });
    }

    public void InstantiateItem(Treasure treasure)
    {
        var item = Instantiate(_itemPrefab, transform, false);
        item.Init(treasure);
    }
}
