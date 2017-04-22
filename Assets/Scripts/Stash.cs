/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stash : MonoBehaviour
{
    public List<Treasure> _inventory = new List<Treasure>();
    public float acceptingRange = 1.2f;

    public StashUpdateEvent onStashUpdated;

    public void Store(Treasure treasure)
    {
        _inventory.Add(treasure);
        treasure.transform.SetParent(transform, false);
        treasure.gameObject.SetActive(false);
        onStashUpdated.Invoke(this);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, acceptingRange);
    }
}

[System.Serializable]
public class StashUpdateEvent : UnityEvent<Stash> { }
