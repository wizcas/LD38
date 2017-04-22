/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorUtils
{
    public const float VEC_EPSILON = .0025f;

    public static float SqrDistance(Vector3 a, Vector3 b)
    {
        return (a - b).sqrMagnitude;
    }

    public static bool IsAtSamePosition(Vector3 a, Vector3 b)
    {
        return SqrDistance(a, b) <= VEC_EPSILON;
    }
}
