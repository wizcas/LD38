/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rigidBody;

    private bool _isPickedUp;
    private bool _showName;

    public bool IsPickedUp
    {
        get { return _isPickedUp; }
        set
        {
            _isPickedUp = value;
            if (_rigidBody != null)
                _rigidBody.useGravity = !value;
            if (value)
                _showName = false;
        }
    }

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    public void OnMouseEnter()
    {
        if (IsPickedUp) { return; }

        _showName = true;
    }

    public void OnMouseExit()
    {
        if (IsPickedUp) { return; }
        _showName = false;
    }

    void OnGUI()
    {
        if (_showName)
        {
            var vp = Camera.main.WorldToViewportPoint(transform.position);
            //Debug.Log(vp);
            var width = 100f;
            var height = 30;
            var offset = new Vector2(5, 5);
            var xPos = Screen.width * vp.x;
            var yPos = Screen.height * (1 - vp.y);
            var screenPos = new Vector2(xPos, yPos) + offset;

            GUI.Box(new Rect(screenPos, new Vector2(width, height)), this.name);
        }
    }
}
