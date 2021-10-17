using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorIntManhattanDistance
{
    public static int manhattanDistance(this Vector2Int vector)
    {
        return Mathf.Abs(vector.x) + Mathf.Abs(vector.y);
    }
}
