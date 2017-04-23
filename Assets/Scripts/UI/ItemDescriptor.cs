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
        Messenger.AddListener<bool>("GameOver", isWin => { ToggleLabel(false); });
    }

    public void ToggleLabel(bool isOn)
    {
        if (!CanShowLabel) { return; }

        _showName = isOn;
    }    

    void OnGUI()
    {
        if (_showName && CanShowLabel)
        {
            var vp = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            //Debug.Log(vp);
            var width = 200f;
            var height = 60;
            var offset = new Vector2(5, 5);
            var xPos = Screen.width * vp.x;
            var yPos = Screen.height * (1 - vp.y);
            var screenPos = new Vector2(xPos, yPos) + offset;

            if(screenPos.x + width > Screen.width)
            {
                screenPos.x -= width + offset.x * 2;
            }
            if(screenPos.y + height > Screen.height)
            {
                screenPos.y -= height + offset.y * 2;
            }

            string text = string.Format("=== {0} ===\n", title.ToUpper());
            text += description;

            GUI.skin = UIManager.Instance.tooltipSkin;
            GUI.Box(new Rect(screenPos, new Vector2(width, height)), text);
            GUI.skin = null;
        }
    }
}

public interface IItem
{
    bool CanShowLabel { get; }
}
