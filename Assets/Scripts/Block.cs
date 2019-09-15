using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    // 方块是否透光
    public bool isTransparent;
    // 方块用的Renderer
    public IBlockRenderer blockRenderer;
    // 方块的碰撞箱
    public Bounds[] boundingBoxes;
}
