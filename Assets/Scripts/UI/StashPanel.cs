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
        transform.DestroyAllChildren();
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
