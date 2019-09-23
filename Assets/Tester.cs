using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    private void Start()
    {
        Debug.Log(ushort.MaxValue);
        Debug.Log(8 << 11);
        BlockData b = 7 << 11;
        Debug.Log(b.light);
        Debug.Log(b.lightF);
        Debug.Log(TestTriangleDirection(1, 0, 1, 1));
    }

    public bool TestTriangleDirection(float a, float b, float c, float d)
    {
        return a + c < b + d;
    }
}
