using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager instance;
    public static Block[] blocks {get; private set;}

    public TextAsset blocksConfig;

    private void Awake() {
        if (instance != null)
            Destroy(this);
        else
            instance = this;
        
        LoadBlocks(blocksConfig.text);
        Debug.Log(blocks[0]);
    }

    private void LoadBlocks(string str)
    {
        var modLoader = new JsonModLoader();
        modLoader.Load(str);
        blocks = modLoader.GetBlocks();
    }
}
