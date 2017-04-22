/************************************
** Created by Wizcas (wizcas.me)
************************************/

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextBubble : UIBehaviour 
{
    public float showAnimLen = .5f;
    public float showDuration = 3f;

    private Text _content;
    public Text Content
    {
        get
        {
            if (_content == null)
                _content = GetComponentInChildren<Text>();
            return _content;
        }
    }

    private CanvasGroup _canvasGroup;
    public CanvasGroup CanvasGroup
    {
        get
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponentInChildren<CanvasGroup>();
            return _canvasGroup;
        }
    }

    private Transform _speaker;

    protected override void Awake() {
        CanvasGroup.alpha = 0f;
    }

    public void Show(Transform speaker, string text)
    {
        _speaker = speaker;
        Content.text = text;
        CanvasGroup.alpha = 0f;
        DOTween.Sequence()
            .Append(CanvasGroup.DOFade(1f, showAnimLen))
            .AppendInterval(showDuration)
            .Append(CanvasGroup.DOFade(0f, showAnimLen))
            .OnComplete(()=> { Destroy(gameObject); });
    }

    void Update()
    {
        if (_speaker)
        {
            var rt = this.GetRectTransform();
            rt.anchoredPosition = UIHelper.GetCanvasCenteredPosition(_speaker.position) + Vector3.up * rt.rect.height;
        }
    }
}
