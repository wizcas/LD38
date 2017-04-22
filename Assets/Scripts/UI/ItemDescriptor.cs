/************************************
** Created by Wizcas (wizcas.me)
************************************/

using UnityEngine;

public class ItemDescriptor : MonoBehaviour
{
    public string title;
    public string description;

    private IItem _item;
    private bool _showName;

    protected virtual bool CanShowLabel
    {
        get { return true; }
    }

    protected virtual void Awake()
    {
    }

    public void OnMouseEnter()
    {
        if (!CanShowLabel) { return; }

        _showName = true;
    }

    public void OnMouseExit()
    {
        if (!CanShowLabel) { return; }
        _showName = false;
    }

    void OnGUI()
    {
        if (_showName)
        {
            var vp = Camera.main.WorldToViewportPoint(transform.position);
            //Debug.Log(vp);
            var width = 200f;
            var height = 60;
            var offset = new Vector2(5, 5);
            var xPos = Screen.width * vp.x;
            var yPos = Screen.height * (1 - vp.y);
            var screenPos = new Vector2(xPos, yPos) + offset;

            string text = string.Format("[{0}]\n", title);
            text += description;
            GUI.Box(new Rect(screenPos, new Vector2(width, height)), text);
        }
    }
}

public interface IItem
{
    bool CanShowLabel { get; }
}
