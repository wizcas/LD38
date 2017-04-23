/************************************
** Created by Wizcas (wizcas.me)
************************************/
using UnityEngine;

public static class VectorUtils
{
    public const float VEC_EPSILON = .0025f;
    public static readonly Vector3 INVALID_VEC3 = Vector3.one * float.MinValue;

    public static float SqrDistance(Vector3 a, Vector3 b)
    {
        return (a - b).sqrMagnitude;
    }

    public static bool IsAtSamePosition(Vector3 a, Vector3 b)
    {
        return SqrDistance(a, b) <= VEC_EPSILON;
    }

    public static float NormalizeAngle(float angle)
    {
        if(angle > 180f)
        {
            angle = angle - 360f;
        }
        return angle;
    }

    public static void DestroyAllChildren(this Transform transform)
    {
        for(int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(child.gameObject);
            }
            else
            {
                Object.Destroy(child.gameObject);
            }
        }
    }
}


public static class Layers
{
    public const string Environment = "Environment";
    public const string Treasure = "Treasure";
    public const string Stash = "Stash";
    public const string Cat = "Cat";
    public const string Human = "Human";

    public static int GetLayerMask(string layerName)
    {
        return 1 << LayerMask.NameToLayer(layerName);
    }

    public static int GetLayerMasks(params string[] layerNames)
    {
        var ret = 0;
        foreach (var layerName in layerNames)
        {
            ret |= GetLayerMask(layerName);
        }
        return ret;
    }
}
