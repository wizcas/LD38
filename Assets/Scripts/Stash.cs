/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stash : ItemDescriptor
{
    [SerializeField]
    private Transform _treasureRoot;
    public List<Treasure> inventory = new List<Treasure>();
    public List<Treasure> collectibles = new List<Treasure>();
    public float acceptingRange = 1.2f;

    public StashUpdateEvent onStashUpdated;

    void Start()
    {
        if(_treasureRoot != null)
        {
            collectibles.AddRange(_treasureRoot.GetComponentsInChildren<Treasure>());
        }
    }

    protected override bool CanShowLabel
    {
        get
        {
            return true;
        }
    }

    public void Store(Treasure treasure)
    {
        inventory.Add(treasure);
        treasure.transform.SetParent(transform, false);
        treasure.gameObject.SetActive(false);
        onStashUpdated.Invoke(this);

        if (collectibles.Contains(treasure))
        {
            collectibles.Remove(treasure);
        }
        if(collectibles.Count == 0)
        {
            Messenger.Broadcast("GameOver", true);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, acceptingRange);
    }
}

[System.Serializable]
public class StashUpdateEvent : UnityEvent<Stash> { }
