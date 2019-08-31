using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class TerrainUpdater : MonoBehaviour
{
    private void Start() {
        // 在这里布置测试用区块
        Chunk c = new Chunk();
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                for (int y = 0; y < 128; y++)
                {
                    if ((x + y + z) % 2 == 0)
                        c[x, y, z] = 1;
                }
            }
        }

        ChunkMesh m = new ChunkMesh();
        var watch = Stopwatch.StartNew();
        for (int z = 0; z < 16; z++)
        {
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 128; y++)
                {
                    Block b = BlockManager.blocks[c[x, y, z]];
                    b.blockRenderer?.GenerateTerrainVertices(x, y, z, c, m);
                }
            }
        }
        watch.Stop();
        print(watch.ElapsedMilliseconds);
        GetComponent<MeshFilter>().mesh = m.ToMesh();
    }
}
