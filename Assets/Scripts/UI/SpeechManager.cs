/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpeechManager : UIBehaviour 
{
    [SerializeField]
    private TextBubble _bubblePrefab;

    void Awake()
    {
        Messenger.AddListener<HumanThought>(HumanAI.HaveThoughtEvent, HaveThought);
    }

    private void HaveThought(HumanThought thought)
    {
        var speech = thought.RandomSpeech();
        if (string.IsNullOrEmpty(speech)) return;

        Show(thought.owner.transform, speech);
    }

    public void Show(Transform target, string content)
    {
        var bubble = Instantiate(_bubblePrefab, transform, false);
        bubble.Show(target, content);
    }    
}
