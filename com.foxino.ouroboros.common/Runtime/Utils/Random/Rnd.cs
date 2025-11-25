using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rnd
{ 
    public static Vector3 RandomVector3(Vector3 bounds)
    {
        return new Vector3(
            Random.Range(-bounds.x, bounds.x),
            Random.Range(-bounds.y, bounds.y),
            Random.Range(-bounds.z, bounds.z));
    }

    public static Vector3 RandomVector3(float x, float y, float z)
    {
        return new Vector3(
            Random.Range(-x, x),
            Random.Range(-y, y),
            Random.Range(-z, z));    
    }

    public static Vector3 RandomVector3()
    {
        return new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f));
    }

    public static Vector3 RandomVector3(float min, float max)
    {
        return new Vector3(
            Random.Range(min, max),
            Random.Range(min, max),
            Random.Range(min, max));
    }


    public static Vector3 InsideElipsoid(float a, float b, float c)
    {
        const int MAX = 20;
        int i = 0;
        float d;
        Vector3 p;

        do
        {
            ++i;

            p = new Vector3(Random.Range(-a, a), Random.Range(-b, b), Random.Range(-c, c));

            d = p.x * p.x / (a * a) + p.y * p.y / (b * b) + p.z * p.z / (c * c);

        } while (d > 1f && i < MAX);

        return p;
    }

    public static float BellCurve()
    {
        float sum = 0f;

        for (int i = 0; i < 3; ++i)
        {
            sum += Random.Range(-1f, 1f);
        }

        return sum / 3;
    }

    public static Vector3 InsideTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        float r1 = Mathf.Sqrt(Random.value);
        float r2 = Random.value;

        return (1 - r1) * a + (r1 * (1 - r2)) * b + r2 * r1 * c;
    }

}
