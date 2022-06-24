using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DistanceCalculationUtility
{
    public static int ManhattanDistance(int x1, int x2, int y1, int y2)
    {
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
    }

    public static int EuclideanDistance(int x1, int x2, int y1, int y2)
    {
        int square = (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
        return square;
    }

    public static int CalculateChebyshevDistance(int x1, int x2, int y1, int y2)
    {
        var dx = Math.Abs(x2 - x1);
        var dy = Math.Abs(y2 - y1);
        return (dx + dy) - Math.Min(dx, dy);
    }
}
