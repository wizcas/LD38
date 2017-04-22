/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAction : MonoBehaviour
{
    public float pickUpDistance = .4f;
    public float stashDistance = 1.2f;

    [SerializeField]
    private Transform _mouth;

    public Treasure _holdingTreasure;

    void Start()
    {
        Messenger.AddListener<Treasure>("PickUp", PickUp);
        Messenger.AddListener<Stash>("Store", Store);
    }

    public void PickUp(Treasure treasure)
    {
        Messenger.Broadcast("MoveTo", treasure.transform.position);
        StopAllCoroutines();
        StartCoroutine(PickUpCo(treasure));        
    }

    private IEnumerator PickUpCo(Treasure treasure)
    {
        while (VectorUtils.SqrDistance(transform.position, treasure.transform.position) > pickUpDistance * pickUpDistance)
        {
            yield return null;
        }
        treasure.transform.SetParent(_mouth, true);
        treasure.transform.localPosition = Vector3.zero;
        _holdingTreasure = treasure;
        _holdingTreasure.IsPickedUp = true;
    }

    public void PutDown()
    {
        if (!ValidateHolding())
            return;
        StopAllCoroutines();
        _holdingTreasure.transform.SetParent(null);
        _holdingTreasure.IsPickedUp = false;
        _holdingTreasure = null;
    }

    public void Store(Stash stash)
    {
        Messenger.Broadcast("MoveTo", stash.transform.position);
        if (!ValidateHolding())
            return;
        StopAllCoroutines();
        StartCoroutine(StoreCo(stash));
    }

    private IEnumerator StoreCo(Stash stash)
    {
        while (VectorUtils.SqrDistance(transform.position, stash.transform.position) > stashDistance * stashDistance)
        {
            yield return null;
        }
        Debug.Log("do stash");
        stash.Store(_holdingTreasure);
        _holdingTreasure = null;
    }

    private bool ValidateHolding()
    {
        if (_holdingTreasure == null)
        {
            Debug.LogError("You're not holding any treasure.");
            return false;
        }
        return true;
    }
}
