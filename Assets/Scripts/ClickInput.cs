/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickInput : MonoBehaviour
{
    public float raycastDistance = 20f;
    public float fxOffsetY = .5f;

    [SerializeField]
    private ParticleSystem _clickFx;

    private float _mouseHoverInterval = .3f;
    private float _nextMouseHoverCheck;

    private List<ItemDescriptor> _showingTipList = new List<ItemDescriptor>();


    void Start()
    {
        _nextMouseHoverCheck = Time.time;
    }

    public void Update()
    {
        CheckMouseHover();
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    private void CheckMouseHover()
    {
        if (Time.time < _nextMouseHoverCheck) return;

        foreach (var item in _showingTipList) {
            item.ToggleLabel(false);
        }
        _showingTipList.Clear();

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = new RaycastHit[3];
        int count = Physics.RaycastNonAlloc(ray, hits, raycastDistance, Layers.GetLayerMasks(Layers.Treasure, Layers.Stash));
        for(int i = 0; i < count; i++)
        {
            var hit = hits[i];
            var item = hit.collider.GetComponent<ItemDescriptor>();
            if (item == null) continue;
            item.ToggleLabel(true);
            _showingTipList.Add(item);
        }

        _nextMouseHoverCheck = Time.time + _mouseHoverInterval;
    }

    private void PlayClickFx(Vector3 pos)
    {
        pos += Vector3.up * fxOffsetY;
        _clickFx.transform.position = pos;
        _clickFx.Play();
    }

    private void HandleClick()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, raycastDistance,
            Layers.GetLayerMasks(Layers.Environment, Layers.Treasure, Layers.Stash)
            ))
        {
            Debug.LogFormat("hit on: {0}", hit.collider.name);
            var hitLayer = LayerMask.LayerToName(hit.collider.gameObject.layer);
            switch (hitLayer)
            {
                case Layers.Environment:
                    PlayClickFx(hit.point);
                    Messenger.Broadcast("MoveTo", hit.point);
                    break;
                case Layers.Treasure:
                    var treasure = hit.collider.GetComponent<Treasure>();
                    if (treasure == null)
                    {
                        Debug.LogErrorFormat("'{0}' is not a Treasure", hit.collider.name);
                    }
                    else
                    {
                        Messenger.Broadcast("PickUp", treasure);
                    }
                    break;
                case Layers.Stash:
                    var stash = hit.collider.GetComponent<Stash>();
                    if(stash == null)
                    {
                        Debug.LogErrorFormat("'{0}' is not a Stash", hit.collider.name);
                    }
                    else
                    {
                        Messenger.Broadcast("Store", stash);
                    }
                    break;
            }
        }
    }
}