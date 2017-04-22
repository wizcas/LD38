using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public static class UIHelper
{
    /// <summary>
    /// 获取当前鼠标位置下的UI射线检测结果
    /// </summary>
    /// <returns>位于当前鼠标位置下的所有UI元素的射线检测结果</returns>
    public static RaycastResult[] RaycastUI()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.ToArray();
    }

    /// <summary>
    /// 检测当前是否在场景中某GameObject的子级UI上（例如检测是否在大地图某城市的信息UI上）
    /// </summary>
    /// <param name="parentGo">所属的GameObject</param>
    /// <param name="callback">若在指定GameObject下的UI上则返回TRUE，否则返回FALSE</param>
    /// <param name="isTopOnly">是否只检测排在顶层的对象</param>
    public static bool IsOnGameObjectUI(GameObject parentGo, bool isTopOnly)
    {
        var results = RaycastUI();
        if (results.Length == 0)
            return false;

        if (isTopOnly)
        {
            return IsHitOfParent(results[0], parentGo);
        }
        else {
            foreach (var result in results)
            {
                if (IsHitOfParent(result, parentGo))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public static bool IsHitOfParent(RaycastResult hit, GameObject parentGo)
    {
        return hit.isValid && hit.gameObject.transform.IsChildOf(parentGo.transform);
    }

    public static Vector3 GetCanvasCenteredPosition( Vector3 worldPos)
    {
        var vp = Camera.main.WorldToViewportPoint(worldPos);
        vp = new Vector2(vp.x, vp.y) - Vector2.one * .5f;

        var canvasT = UIManager.Instance.canvas.transform as RectTransform;
        var pos = new Vector3(canvasT.rect.size.x * vp.x, canvasT.rect.size.y * vp.y);

        return pos;
    }

    public static Rect GetCanvasCenteredRect(Vector2 size, Vector3 worldPos)
    {
        var pos = GetCanvasCenteredPosition(worldPos);
        return new Rect(pos, size);
    }

    public static bool IsBoundsInScreen(Bounds bounds)
    {
        float minX = bounds.min.x;
        float maxX = bounds.max.x;
        float minZ = bounds.min.z;
        float maxZ = bounds.max.z;

        float maxScreenZ = Camera.main.orthographicSize;
        float minScreenZ = -maxScreenZ;
        float maxScreenX = maxScreenZ * (float)Screen.width / (float)Screen.height;
        float minScreenX = -maxScreenX;

        return minX >= minScreenX && maxX <= maxScreenX
            && minZ >= minScreenZ && maxZ <= maxScreenZ;
    }

    public static Vector2 WorldBounds2ScreenSize(Bounds bounds)
    {
        var bsize = bounds.size;

        var h = (UIManager.Instance.canvas.transform as RectTransform).rect.height;


        var pixelsPerWorldUnit = h * .5f / Camera.main.orthographicSize;

        Debug.LogFormat("Screen height: {0}, PPWU: {1}", h, pixelsPerWorldUnit);


        var screenSize = new Vector2(bsize.x, bsize.z) * pixelsPerWorldUnit;
        return screenSize;
    }

    public static RectTransform GetRectTransform(this UIBehaviour ui)
    {
        return ui.transform as RectTransform;
    }
}
