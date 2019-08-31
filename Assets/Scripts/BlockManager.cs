using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager instance;
    public static Block[] blocks {get; private set;}

    private void Awake() {
        blocks = new Block[] {
            new Block{isTransparent = true},
            new Block{isTransparent = false, blockRenderer = new CubeBlockRenderer()}
        };
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }
}
