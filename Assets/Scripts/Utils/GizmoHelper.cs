using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class GizmoHelper
{
    private static Texture2D labelTex;
    private static GUIStyle defaultLabelStyle;
    private static GUIStyle smallLabelStyle;
    private static GUIStyle largeLabelStyle;

    public static readonly Color DefaultBackgroundColor = new Color(42 / 255f, 192 / 255f, 217 / 255f, 0.5f);
    public static readonly Color DefaultTextColor = Color.black;
    
#if UNITY_EDITOR
    static GizmoHelper()
    {
        labelTex = Resources.Load<Texture2D>("Gizmos/LabelTex");

        defaultLabelStyle = new GUIStyle
        {
            normal =
            {
                background = labelTex,
                textColor = DefaultTextColor
            },
            margin = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(0, 0, 0, 0),
            alignment = TextAnchor.MiddleCenter,
            border = new RectOffset(6, 6, 6, 6),
            fontSize = 14
        };

        smallLabelStyle = new GUIStyle(defaultLabelStyle);
        smallLabelStyle.fontSize = 9;

        largeLabelStyle = new GUIStyle(defaultLabelStyle);
        largeLabelStyle.fontSize = 16;
    }
#endif

    public static void Label(Vector3 position, string label, LabelSize size = LabelSize.Normal)
    {
        Label(position, label, DefaultBackgroundColor, DefaultTextColor, size);
    }

    public static void Label(Vector3 position, string label, Color bgColor, Color textColor, LabelSize size = LabelSize.Normal)
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(label)) return;
        var color = GUI.color;
        GUI.color = Color.white;

        var backgroundColor = GUI.backgroundColor;
        GUI.backgroundColor = bgColor;

        GUIStyle style;
        switch (size)
        {
            case LabelSize.Small:
                style = new GUIStyle(smallLabelStyle);
                break;
            case LabelSize.Large:
                style = new GUIStyle(largeLabelStyle);
                break;
            default:
                style = new GUIStyle(defaultLabelStyle);
                break;
        }

        style.normal.textColor = textColor;
        
        Handles.Label(position, label, style);

        GUI.backgroundColor = backgroundColor;
        GUI.color = color;
#endif
    }
    
    public static void ArrowEnd(Vector3 pos, float size = .2f)
    {
        Gizmos.DrawLine(pos + Vector3.forward * size, pos + Vector3.back * size);
    }

    public static void Arrow(Vector3 pos, ArrowDirection dir, float size = .3f)
    {
    //    Vector3 upper = pos + new Vector3(size * (int)side, 0f, size);
    //    Vector3 lower = pos + new Vector3(size * (int)side, 0f, -size);

        Vector3 p0 = Vector3.zero, p1 = Vector3.zero;
        switch (dir)
        {
            case ArrowDirection.Up:
                p0 = new Vector3(-size, 0f, -size);
                p1 = new Vector3(size, 0f, -size);
                break;
            case ArrowDirection.Down:
                p0 = new Vector3(-size, 0f, size);
                p1 = new Vector3(size, 0f, size);
                break;
            case ArrowDirection.Left:
                p0 = new Vector3(size, 0f, -size);
                p1 = new Vector3(size, 0f, size);
                break;
            case ArrowDirection.Right:
                p0 = new Vector3(-size, 0f, -size);
                p1 = new Vector3(-size, 0f, size);
                break;
        }
        Gizmos.DrawLine(pos, pos + p0);
        Gizmos.DrawLine(pos, pos + p1);
    }

    public static Vector3 OffsetBySceneViewPos(Vector3 worldPoint, Vector3 offsetInScreenCoordinates)
    {
        Camera camera;
#if UNITY_EDITOR
        camera = SceneView.currentDrawingSceneView.camera;
#else
        camera = Camera.main;
#endif
        return camera.ScreenToWorldPoint(camera.WorldToScreenPoint(worldPoint) + offsetInScreenCoordinates);
    }
    
    public enum LabelSize
    {
        Small,
        Normal, 
        Large
    }

    public enum Plane
    {
        X, Y, Z
    }

    public enum ArrowDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}
