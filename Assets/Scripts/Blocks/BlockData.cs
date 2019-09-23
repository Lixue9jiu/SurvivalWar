using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BlockData
{
    public ushort value;

    public BlockData(int index, int light)
    {
        value = MakeBlockData(index, light);
    }

    public static ushort MakeBlockData(int index, int light) => (ushort)(index + (light << 11));

    public static implicit operator BlockData(ushort data) => new BlockData{ value = data };
    public int index => value & 1023;
    public int light => (value >> 11) & 7;
    public float lightF => (float)light / 7f;
}
