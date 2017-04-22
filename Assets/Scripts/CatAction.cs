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
    [SerializeField]
    private Animator _anim;

    private Treasure _holdingTreasure;
    public Treasure HoldingTreasure
    {
        get { return _holdingTreasure; }
    }
    private bool _isMeowing;

    public bool IsHoldingTreasure
    {
        get { return _holdingTreasure != null; }
    }

    void Awake()
    {
        Messenger.AddListener<Treasure>("PickUp", PickUp);
        Messenger.AddListener<Stash>("Store", Store);
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(Meow());
        }
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
        stash.Store(_holdingTreasure);
        _holdingTreasure = null;
    }

    private bool ValidateHolding()
    {
        if (!IsHoldingTreasure)
        {
            Debug.LogError("You're not holding any treasure.");
            return false;
        }
        return true;
    }

    private IEnumerator Meow()
    {
        if (_isMeowing) yield break;
        _anim.SetTrigger("Sound");
        while (!_anim.IsInTransition(1))
        {
            yield return null;
        }
        var clipInfos = _anim.GetNextAnimatorClipInfo(1);
        var clip = clipInfos[0].clip;
        _isMeowing = true;
        Messenger.Broadcast("SoundMade", transform);
        yield return new WaitForSeconds(clip.length);
        _isMeowing = false;
    }
}
