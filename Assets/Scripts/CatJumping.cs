/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CatJumping : MonoBehaviour 
{
    private Animator _anim;
    public System.Action onAnimatorMove;

    public Animator Anim
    {
        get { return _anim; }
    }

    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

	public void OnAnimationJump(float duration)
    {        
        transform.parent.SendMessage("DoJump", duration);
    }

    void OnAnimatorMove()
    {
        if (onAnimatorMove != null)
            onAnimatorMove();
    }
}
