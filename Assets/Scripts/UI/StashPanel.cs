/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StashPanel : MonoBehaviour
{
    [SerializeField]
    private StashedItem _itemPrefab;

    public void Clear()
    {
        for(int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i).gameObject;
            Destroy(child);
        }
    }

    public void UpdateItems(Stash stash)
    {
        Clear();
        foreach(var treasure in stash.inventory)
        {
            InstantiateItem(treasure);
        }
    }

    public void InstantiateItem(Treasure treasure)
    {
        var item = Instantiate(_itemPrefab, transform, false);
        item.Init(treasure);
    }
}
