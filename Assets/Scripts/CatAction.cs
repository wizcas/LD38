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
    public AudioClip meowClip;

    [SerializeField]
    private Transform _mouth;
    [SerializeField]
    private Animator _anim;
    [SerializeField]
    private AudioSource _audioSrc;

    public HideSpot hideSpot;
    public bool isHiding;

    private bool _isMeowing;

    private Treasure _holdingTreasure;
    public Treasure HoldingTreasure
    {
        get { return _holdingTreasure; }
    }

    public bool IsHoldingTreasure
    {
        get { return _holdingTreasure != null; }
    }

    void Awake()
    {
        Messenger.AddListener<Treasure>("PickUp", PickUp);
        Messenger.AddListener<Stash>("Store", Store);
        Messenger.AddListener<HideSpot>("ToHideSpot", ToHideSpot);
        Messenger.AddListener("EnterHideSpot", EnterHideSpot);
    }

    void Start()
    {
        
    }

    public void PickUp(Treasure treasure)
    {
        Messenger.Broadcast("CatMoveTo", treasure.transform.position);
        StopAllCoroutines();
        StartCoroutine(PickUpCo(treasure));
    }

    private IEnumerator PickUpCo(Treasure treasure)
    {
        while (VectorUtils.SqrDistance(transform.position, treasure.transform.position) > pickUpDistance * pickUpDistance)
        {
            yield return null;
        }

        PutDown();

        treasure.transform.SetParent(_mouth, true);
        treasure.transform.localPosition = Vector3.zero;
        _holdingTreasure = treasure;
        _holdingTreasure.IsPickedUp = true;
    }

    public void PutDown()
    {
        if (!ValidateHolding())
            return;
        //StopAllCoroutines();
        _holdingTreasure.transform.SetParent(null);
        _holdingTreasure.IsPickedUp = false;
        _holdingTreasure = null;
    }

    public void Store(Stash stash)
    {
        Messenger.Broadcast("CatMoveTo", stash.transform.position);
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

    private void ToHideSpot(HideSpot spot)
    {
        if (hideSpot != null)
        {
            hideSpot.Disable();
        }

        hideSpot = spot;
        Messenger.Broadcast("CatMoveTo", spot.EntrancePos);
    }

    private void EnterHideSpot()
    {
        Messenger.Broadcast("CatStop");
        _anim.gameObject.SetActive(false);
        isHiding = true;
    }

    public void ExitHideSpot()
    {
        _anim.gameObject.SetActive(true);
        if (hideSpot != null)
        {
            transform.position = hideSpot.EntrancePos;
            hideSpot.Disable();
        }
        hideSpot = null;
        isHiding = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(Meow());
        }
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
        var soundSource = new GameObject("Meow");
        soundSource.transform.position = transform.position;
        Messenger.Broadcast("SoundMade", soundSource.transform);
        _audioSrc.PlayOneShot(meowClip);
        yield return new WaitForSeconds(meowClip.length);
        _isMeowing = false;
    }
}
