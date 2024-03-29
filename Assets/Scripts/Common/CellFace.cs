﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellFace
{
    public const int Left = 0;
    public const int Up = 1;
    public const int Front = 2;
    public const int Right = 3;
    public const int Down = 4;
    public const int Back = 5;

    public static Vector3Int[] Faces = {
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, 0, -1)
    };
}
