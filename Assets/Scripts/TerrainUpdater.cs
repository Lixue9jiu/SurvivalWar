using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

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
                    if (y < Mathf.PerlinNoise((float)x / 16f, (float)z / 16f) * 128)
                    {
                        c[x, y, z] = 1;
                    }
                }
            }
        }

        TerrainManager t = GetComponent<TerrainManager>();
        t.SetChunk(0, 0, c);
        t.SetChunk(0, 1, c);
        t.SetChunk(1, 0, c);
        t.SetChunk(0, -1, c);
        t.SetChunk(-1, 0, c);

        int chunkx = 0;
        int chunkz = 0;
        Chunk c20 = t.GetChunk(chunkx + 1, chunkz - 1);
        Chunk c21 = t.GetChunk(chunkx + 1, chunkz);
        Chunk c22 = t.GetChunk(chunkx + 1, chunkz + 1);
        Chunk c10 = t.GetChunk(chunkx, chunkz - 1);
        Chunk c11 = t.GetChunk(chunkx, chunkz);
        Chunk c12 = t.GetChunk(chunkx, chunkz + 1);
        Chunk c00 = t.GetChunk(chunkx - 1, chunkz - 1);
        Chunk c01 = t.GetChunk(chunkx - 1, chunkz);
        Chunk c02 = t.GetChunk(chunkx - 1, chunkz + 1);
        ChunkMesh m = new ChunkMesh();
        var watch = Stopwatch.StartNew();
        for (int z = 0; z < 16; z++)
        {
            for (int x = 0; x < 16; x++)
            {
                switch (x)
                {
                    case 0:
                        if (c01 == null || z == 0 && c00 == null || z == Chunk.SIZE_Z_MINUS_ONE && c02 == null) continue;
                        goto default;
                    case Chunk.SIZE_X_MINUS_ONE:
                        if (c21 == null || z == 0 && c20 == null || z == Chunk.SIZE_Z_MINUS_ONE && c22 == null) continue;
                        goto default;
                    default:
                        if (z == 0 && c10 == null || z == Chunk.SIZE_Z_MINUS_ONE && c12 == null) continue;

                        // if (x == 0) Debug.Log("0");
                        for (int y = 0; y < 128; y++)
                        {
                            Block b = BlockManager.blocks[c[x, y, z]];
                            b.blockRenderer?.GenerateTerrainVertices(x, y, z, t, m);
                        }
                        break;
                }
            }
        }
        watch.Stop();
        print(watch.ElapsedMilliseconds);
        GetComponent<MeshFilter>().mesh = m.ToMesh();
    }
}
