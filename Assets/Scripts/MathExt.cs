using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathExt
{
    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    public static bool Roll(int count)
    {
        int i = Random.Range(0, count);
        return (i == count-1);
    }

    public static bool Roll(int count, out int result)
    {
        result = Random.Range(0, count);
        return (result == count - 1);
    }

    public static bool Between(float a, float b, float c)
    {
        return a > b ? (a > b && a < c) : (a > c && a < b);
    }

    public static Vector3 Direction(Vector3 from, Vector3 to, bool normalised = true)
    {
        Vector3 dir = to - from;
        if (normalised)
            dir = dir.normalized;
        return dir;
    }

    public static Vector3 FlattenVector3(Vector3 vector)
    {
        vector.y = 0;
        return vector;
    }

    public static Vector3 VectorYToZ(Vector2 vector2)
    {
        return new Vector3(vector2.x, 0, vector2.y);
    }
    public static Vector2 VectorZToY(Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.z);
    }

    /// <summary>
    /// If n falls below min or above max, it will wrap to the other value
    /// </summary>
    /// <param name="n"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int WrapIndex(int n, int min, int max)
    {
        if (n < min)
        {
            n = max;
            return n;
        }
        if (n > max)
        {
            n = min;
            return n;
        }
        return n;
    }
}
