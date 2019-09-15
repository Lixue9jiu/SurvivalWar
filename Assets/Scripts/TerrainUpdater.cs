using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

public class TerrainUpdater : MonoBehaviour
{
    class ChunkInstance
    {
        public Matrix4x4 transform;
        public Mesh mesh;
    }

    public Material material;

    TerrainManager terrain;
    TaskManager taskManager;
    List<ChunkInstance> toRender = new List<ChunkInstance>();
    Dictionary<Vector2Int, ChunkInstance> liveInstances = new Dictionary<Vector2Int, ChunkInstance>();
    List<Vector2Int> dirtyChunks = new List<Vector2Int>(10);

    private void Awake() {
        terrain = GetComponent<TerrainManager>();
        taskManager = GetComponent<TaskManager>();
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
        StartCoroutine(DelayTest());
    }

    private IEnumerator DelayTest()
    {
        yield return new WaitForSeconds(5);

        var m = GetComponent<TaskManager>();
        for (int x = 0; x < 4; x++)
        {
            for (int z = 0; z < 4; z++)
            {
                QueueChunkUpdate(x, z);
            }
        }

        yield return DelayTest2();
    }

    private IEnumerator DelayTest2()
    {
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                // Debug.Log("set cell");
                terrain.SetCell(x, 100, z, 1);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private void Update() {
        foreach (var c in dirtyChunks)
        {
            taskManager.SchaduleTask(new GenerateChunkMeshTask
            {
                chunkx = c.x,
                chunkz = c.y,
                terrain = terrain,
                output = toRender,
                outputMap = liveInstances
            });
        }
        dirtyChunks.Clear();
        foreach (var m in toRender)
        {
            Graphics.DrawMesh(m.mesh, m.transform, material, 0);
        }
    }

    public void QueueChunkNeighborUpdate(int x, int z, int radius)
    {
        for (int i = x - radius; i < x + radius; i++)
        {
            for (int k = z - radius; k < z + radius; k++)
            {
                QueueChunkUpdate(i, k);
            }
        }
    }

    public void QueueChunkUpdate(int x, int z)
    {
        // Debug.Log($"queue update: {x}, {z}");
        if (terrain.GetChunk(x, z) != null)
            dirtyChunks.Add(new Vector2Int(x, z));
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
        public Dictionary<Vector2Int, ChunkInstance> outputMap;

        public void Execute()
        {
            result = GenerateTerrainMesh();
        }

        public void CallBack()
        {
            var pos = new Vector2Int(chunkx, chunkz);
            if (outputMap.ContainsKey(pos))
            {
                outputMap[pos].mesh = result.ToMesh();
            }
            else
            {
                var ci = new ChunkInstance { mesh = result.ToMesh(), transform = Matrix4x4.Translate(new Vector3(chunkx << Chunk.CHUNK_X_SHIFT, 0, chunkz << Chunk.CHUNK_Z_SHIFT)) };
                outputMap[pos] = ci;
                output.Add(ci);
            }
        }

        ChunkMesh GenerateTerrainMesh()
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
            // print(watch.ElapsedMilliseconds);
            return m;
        }
    }
}
