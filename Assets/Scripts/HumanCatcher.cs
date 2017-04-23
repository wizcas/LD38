/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanCatcher : MonoBehaviour 
{
    private Collider _collider;
    void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.enabled = false;
        Messenger.AddListener<HumanThought>(HumanAI.HaveThoughtEvent, HaveThought);
    }

    void HaveThought(HumanThought thought)
    {
        _collider.enabled = thought.state == HumanState.Hostile;
    }

	void OnTriggerEnter(Collider other)
    {
        var cat = other.GetComponent<CatAction>();
        if (cat == null) return;

        Debug.LogFormat("<color=red><size=12>Game Over.</size></color>");
        Messenger.Broadcast("GameOver", false);
    }
}
