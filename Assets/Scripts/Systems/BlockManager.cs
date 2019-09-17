using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager instance;
    public static Block[] blocks {get; private set;}

    private void Start() {
        if (instance != null)
            Destroy(this);
        else
            instance = this;
        
        blocks = GetComponent<ModManager>().GetBlocks();
        foreach (Block b in blocks)
        {
            b.renderer?.Initialize();
        }
    }
}
