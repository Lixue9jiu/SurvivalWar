using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BlockData
{
    public short value;

    public static implicit operator BlockData(short data) => new BlockData{ value = data };
    public int index => value & 1023;
    public int light => (value >> 11) & 7;
}
