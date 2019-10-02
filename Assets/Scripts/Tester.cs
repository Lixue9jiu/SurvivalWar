using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    private void Start()
    {
        TestBlock();
    }

    public bool TestTriangleDirection(float a, float b, float c, float d)
    {
        return a + c < b + d;
    }

    public void TestBlock()
    {
        BlockData b = BlockData.MakeBlockData(10, 7);
        Debug.Log(b.index);
        Debug.Log(b.light);
        b.index = 12;
        Debug.Log(b.index);
        b.light = 4;
        Debug.Log(b.light);

        ShiftData s = 10;
        Debug.Log(s.maxHeight);
        s.maxHeight = 13;
        Debug.Log(s.maxHeight);
    }
}
