using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Unity.Jobs;
using Stopwatch = System.Diagnostics.Stopwatch;

public class TerrainUpdater : MonoBehaviour
{
    struct ChunkInstance
    {
        public Matrix4x4 transform;
        public Mesh mesh;
    }

    public Material material;

    TerrainManager terrain;
    List<ChunkInstance> toRender = new List<ChunkInstance>();

    private void Awake() {
        terrain = GetComponent<TerrainManager>();
    }

    private void Start() {
        TerrainGenerator g = new TerrainGenerator();
        for (int x = 0; x < 4; x++)
        {
            for (int z = 0; z < 4; z++)
            {
                GenerateChunk(x, z, g);
            }
        }
        var m = GetComponent<TaskManager>();
        for (int x = 0; x < 4; x++)
        {
            for (int z = 0; z < 4; z++)
            {
                m.SchaduleTask(new GenerateChunkMeshTask
                {
                    chunkx = x,
                    chunkz = z,
                    terrain = terrain,
                    output = toRender
                });
            }
        }
    }

    private void Update() {
        foreach (var m in toRender)
        {
            Graphics.DrawMesh(m.mesh, m.transform, material, 0);
        }
    }

    void GenerateChunk(int x, int z, TerrainGenerator g)
    {
        terrain.SetChunk(x, z, g.GenerateTerrain(x, z));
    }

    class GenerateChunkMeshTask : ITask
    {
        public int chunkx, chunkz;
        public TerrainManager terrain;
        public ChunkMesh result;
        public List<ChunkInstance> output;

        public void Execute()
        {
            result = GenerateTerrainMesh(chunkx, chunkz);
        }

        public void CallBack()
        {
            output.Add(new ChunkInstance { mesh = result.ToMesh(), transform = Matrix4x4.Translate(new Vector3(chunkx << Chunk.CHUNK_X_SHIFT, 0, chunkz << Chunk.CHUNK_Z_SHIFT)) });
        }

        ChunkMesh GenerateTerrainMesh(int chunkx, int chunkz)
        {
            int bx = chunkx << Chunk.CHUNK_X_SHIFT;
            int bz = chunkz << Chunk.CHUNK_Z_SHIFT;
            Chunk c20 = terrain.GetChunk(chunkx + 1, chunkz - 1);
            Chunk c21 = terrain.GetChunk(chunkx + 1, chunkz);
            Chunk c22 = terrain.GetChunk(chunkx + 1, chunkz + 1);
            Chunk c10 = terrain.GetChunk(chunkx, chunkz - 1);
            Chunk c11 = terrain.GetChunk(chunkx, chunkz);
            Chunk c12 = terrain.GetChunk(chunkx, chunkz + 1);
            Chunk c00 = terrain.GetChunk(chunkx - 1, chunkz - 1);
            Chunk c01 = terrain.GetChunk(chunkx - 1, chunkz);
            Chunk c02 = terrain.GetChunk(chunkx - 1, chunkz + 1);
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
                            for (int y = 1; y < 127; y++)
                            {
                                Block b = BlockManager.blocks[c11[x, y, z]];
                                b.blockRenderer?.GenerateTerrainVertices(x + bx, y, z + bz, terrain, m);
                            }
                            break;
                    }
                }
            }
            watch.Stop();
            print(watch.ElapsedMilliseconds);
            return m;
        }
    }
}
