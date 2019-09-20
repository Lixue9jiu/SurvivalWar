using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

public class TerrainUpdater : MonoBehaviour
{
    public TerrainInstance terrainInstance;
    TerrainManager terrain;
    TaskManager taskManager;
    // List<Vector2Int> dirtyChunks = new List<Vector2Int>(10);
    LinkedList<Vector2Int> dirtyChunks = new LinkedList<Vector2Int>();
    // Queue<Vector2Int> immediateDirtyChunks = new Queue<Vector2Int>(10);

    private void Awake() {
        terrain = GetComponent<TerrainManager>();
        taskManager = GetComponent<TaskManager>();
    }

    private void Start() {
        var tm = GetComponent<BlockTextureManager>();
        terrainInstance.opaqueMaterial.mainTexture = tm.MainTexture;
        terrainInstance.alphaMaterial.mainTexture = tm.MainTexture;
        TerrainGenerator g = new TerrainGenerator();
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                GenerateChunk(x, z, g);
            }
        }
        StartCoroutine(DelayTest());
    }

    private IEnumerator DelayTest()
    {
        yield return new WaitForSeconds(1);

        var m = GetComponent<TaskManager>();
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
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
            var chunk = terrain.GetChunk(c);
            if (chunk.chunkState != Chunk.ChunkState.NeedsMeshUpdate) continue;
            taskManager.SchaduleTask(new GenerateChunkMeshTask
            {
                chunkx = c.x,
                chunkz = c.y,
                terrain = terrain,
                terrainInstance = terrainInstance
            });
            chunk.chunkState = Chunk.ChunkState.GeneratingMesh;
        }
        dirtyChunks.Clear();
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
        Chunk chunk;
        if ((chunk = terrain.GetChunk(x, z)) != null)
        {
            chunk.chunkState = Chunk.ChunkState.NeedsMeshUpdate;
            dirtyChunks.AddLast(new Vector2Int(x, z));
        }
    }

    public void QueueChunkUpdateImmediate(int x, int z)
    {
        Chunk chunk;
        if ((chunk = terrain.GetChunk(x, z)) != null)
        {
            chunk.chunkState = Chunk.ChunkState.NeedsMeshUpdate;
            dirtyChunks.AddFirst(new Vector2Int(x, z));
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
        public TerrainInstance terrainInstance;
        public ChunkMesh result;
        // public Dictionary<Vector2Int, ChunkInstance> outputMap;

        public void Execute()
        {
            result = GenerateTerrainMesh();
        }

        public void CallBack()
        {
            var pos = new Vector2Int(chunkx, chunkz);
            terrain.GetChunk(pos).chunkState = Chunk.ChunkState.Good;
            terrainInstance.UpdateChunk(pos, result.GetMesh());
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
            // var watch = Stopwatch.StartNew();
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
                                Block b = BlockManager.blocks[c11[x, y, z].index];
                                b.renderer?.GenerateTerrainVertices(x + bx, y, z + bz, terrain, m);
                            }
                            break;
                    }
                }
            }
            // watch.Stop();
            // print(watch.ElapsedMilliseconds);
            return m;
        }
    }
}
