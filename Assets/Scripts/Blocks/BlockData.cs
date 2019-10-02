using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BlockData
{
    public ushort data;

    public BlockData(int index, int light)
    {
        data = MakeBlockData(index, light);
    }

    public static ushort MakeBlockData(int index, int light) => (ushort)(index + (light << 11));

    public static implicit operator BlockData(ushort data) => new BlockData{ data = data };
    public int index
    {
        get
        {
            return data & 1023;
        }
        set
        {
            data = (ushort)((data & ~1023) | value);
        }
    }
    public int light
    {
        get
        {
            return (data >> 11) & 7;
        }
        set
        {
            data = (ushort)((data & ~0b_0001_1100_0000_0000) | (value << 11));
        }
    }
    public float lightF => Mathf.Lerp(0.5f, 1f, (float)light / 7f);

    public static bool operator==(BlockData a, BlockData b)
    {
        return a.data == b.data;
    }

    public static bool operator!=(BlockData a, BlockData b)
    {
        return a.data != b.data;
    }

    public override bool Equals(object obj)
    {
        if (obj is BlockData)
            return ((BlockData)obj).data == data;
        return false;
    }
    
    public override int GetHashCode()
    {
        return data;
    }
}
