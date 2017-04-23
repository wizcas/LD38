/************************************
** Created by Wizcas (wizcas.me)
************************************/

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : UIBehaviour 
{
    [SerializeField]
    private CollectionItem _itemPrefab;

    [SerializeField]
    private GameObject _winTitle;
    [SerializeField]
    private GameObject _failTitle;

    [SerializeField]
    private Text _txtCollectionTitle;
    [SerializeField]
    private Text _txtNothing;

    [SerializeField]
    private Transform _collectionRoot;

    [SerializeField]
    private Stash _stash;

    [SerializeField]
    private GameObject _btnRestart;

	protected override void Awake()
    {
        Messenger.AddListener<bool>("GameOver", GameOver);
    }

    protected override void Start()
    {
        gameObject.SetActive(false);
        GetComponent<CanvasGroup>().alpha = 0f;
        SetAlpha(_txtCollectionTitle, 0f);
        SetAlpha(_txtNothing, 0f);
        _btnRestart.SetActive(false);
    }

    private void SetAlpha(Graphic g, float alpha)
    {
        var color = g.color;
        color.a = 0f;
        g.color = color;
    }

    private void GameOver(bool isWin)
    {
        _winTitle.SetActive(isWin);
        _failTitle.SetActive(!isWin);
        gameObject.SetActive(true);
        GetComponent<CanvasGroup>().DOFade(1f, 1f).OnComplete(()=>
        {
            StartCoroutine(Statistic());
        });
    }

    private IEnumerator Statistic()
    {
        yield return _txtCollectionTitle.DOFade(1f, 1f).WaitForCompletion();
        if (_stash.inventory.Count > 0)
        {
            foreach (var i in _stash.inventory)
            {
                var item = Instantiate(_itemPrefab, _collectionRoot, false);
                item.Init(i);
                yield return item.Show().WaitForCompletion();
            }
        }
        else
        {
            yield return _txtNothing.DOFade(1f, 1f).WaitForCompletion();
        }
        _btnRestart.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
