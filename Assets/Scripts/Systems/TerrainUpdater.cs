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

    private void Awake()
    {
        terrain = GetComponent<TerrainManager>();
        taskManager = GetComponent<TaskManager>();
    }

    private void Start()
    {
        var tm = GetComponent<BlockTextureManager>();
        terrainInstance.opaqueMaterial.mainTexture = tm.MainTexture;
        terrainInstance.alphaMaterial.mainTexture = tm.MainTexture;
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
        yield return new WaitForSeconds(1);

        for (int x = 0; x < 4; x++)
        {
            for (int z = 0; z < 4; z++)
            {
                QueueChunkUpdate(x, z, Chunk.ChunkState.InvalidLighting);
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
                yield return new WaitForSeconds(0.02f);
            }
        }
    }

    private void Update()
    {
        while (dirtyChunks.Count > 0)
        {
            var c = dirtyChunks.First.Value;
            dirtyChunks.RemoveFirst();
            var chunk = terrain.GetChunk(c);
            if (chunk.chunkState == Chunk.ChunkState.NeedsMeshUpdate)
            {
                taskManager.SchaduleTask(new GenerateChunkMeshTask
                {
                    chunkx = c.x,
                    chunkz = c.y,
                    terrain = terrain,
                    terrainInstance = terrainInstance
                });
                chunk.chunkState = Chunk.ChunkState.GeneratingMesh;
            }
            else if (chunk.chunkState < Chunk.ChunkState.NeedsLightUpdate)
            {
                taskManager.SchaduleTask(new GenerateChunkLightingTask
                {
                    chunkx = c.x,
                    chunkz = c.y,
                    terrain = terrain,
                    updater = this,
                    lightUpdates = new Stack<Vector3Int>()
                });
            }
        }
    }

    public void QueueChunkNeighborUpdate(int x, int z, int radius, Chunk.ChunkState state)
    {
        for (int i = x - radius; i < x + radius; i++)
        {
            for (int k = z - radius; k < z + radius; k++)
            {
                QueueChunkUpdate(i, k, state);
            }
        }
    }

    public void QueueChunkUpdate(int x, int z, Chunk.ChunkState state)
    {
        // Debug.Log($"queue update: {x}, {z}");
        Chunk chunk;
        if ((chunk = terrain.GetChunk(x, z)) != null)
        {
            chunk.chunkState = state;
            dirtyChunks.AddLast(new Vector2Int(x, z));
        }
    }

    public void QueueChunkUpdateImmediate(int x, int z)
    {
        Chunk chunk;
        if ((chunk = terrain.GetChunk(x, z)) != null)
        {
            chunk.chunkState = Chunk.ChunkState.InvalidLighting;
            dirtyChunks.AddFirst(new Vector2Int(x, z));
        }
    }

    void GenerateChunk(int x, int z, TerrainGenerator g)
    {
        terrain.SetChunk(x, z, g.GenerateTerrain(x, z));
    }

    class GenerateChunkLightingTask : ITask
    {
        public int chunkx, chunkz;
        public TerrainManager terrain;
        public TerrainUpdater updater;
        public Stack<Vector3Int> lightUpdates;

        public void CallBack()
        {
            updater.QueueChunkUpdate(chunkx, chunkz, Chunk.ChunkState.NeedsMeshUpdate);
        }

        public void Execute()
        {
            var chunk = terrain.GetChunk(chunkx, chunkz);
            if (chunk.chunkState == Chunk.ChunkState.InvalidLighting)
            {
                int startx = chunkx << Chunk.CHUNK_X_SHIFT;
                int startz = chunkz << Chunk.CHUNK_Z_SHIFT;
                for (int z = 0; z < 16; z++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        bool isUnderground = false;
                        int y = 127;
                        while (y > 1)
                        {
                            var cell = chunk[x, y, z];
                            if (isUnderground)
                            {
                                cell.light = 0;
                                chunk[x, y, z] = cell;
                            }
                            else
                            {
                                cell.light = LightSettings.GlobalLightLevel;
                                chunk[x, y, z] = cell;
                                lightUpdates.Push(new Vector3Int(x + startx, y, z + startz));
                            }

                            if (!isUnderground && !BlockManager.blocks[chunk[x, y - 1, z].index].isTransparent)
                            {
                                isUnderground = true;
                                var shift = chunk.GetShiftData(x, z);
                                shift.maxHeight = y - 1;
                                chunk.SetShiftData(x, z, shift);
                            }
                            y--;
                        }
                    }
                }
            }

            while (lightUpdates.Count > 0)
            {
                var pos = lightUpdates.Pop();
                int light = terrain.GetCell(pos).light;

                if (light < 2) continue;
                
                Chunk c11 = terrain.ChunkWithBlock(pos.x, pos.y, pos.z);
                Chunk c21 = terrain.ChunkWithBlock(pos.x + 1, pos.y, pos.z);
                Chunk c01 = terrain.ChunkWithBlock(pos.x - 1, pos.y, pos.z);
                Chunk c12 = terrain.ChunkWithBlock(pos.x, pos.y, pos.z + 1);
                Chunk c10 = terrain.ChunkWithBlock(pos.x, pos.y, pos.z - 1);
                Chunk c02 = terrain.ChunkWithBlock(pos.x - 1, pos.y, pos.z + 1);
                Chunk c22 = terrain.ChunkWithBlock(pos.x + 1, pos.y, pos.z + 1);
                Chunk c00 = terrain.ChunkWithBlock(pos.x - 1, pos.y, pos.z - 1);
                Chunk c20 = terrain.ChunkWithBlock(pos.x + 1, pos.y, pos.z - 1);

                if (c21 != null)
                    ProcessCell(new Vector3Int(pos.x + 1, pos.y, pos.z), c21, light);

                if (c01 != null)
                    ProcessCell(new Vector3Int(pos.x - 1, pos.y, pos.z), c01, light);

                if (c12 != null)
                    ProcessCell(new Vector3Int(pos.x, pos.y, pos.z + 1), c12, light);

                if (c10 != null)
                    ProcessCell(new Vector3Int(pos.x, pos.y, pos.z - 1), c10, light);

                if (pos.y < 127)
                    ProcessCell(new Vector3Int(pos.x, pos.y + 1, pos.z), c11, light);
                
                if (pos.y > 1)
                    ProcessCell(new Vector3Int(pos.x, pos.y - 1, pos.z), c11, light);
            }
        }

        private void ProcessCell(Vector3Int pos, Chunk c, int light)
        {
            var cell = c.GetCellCliped(pos);
            if (BlockManager.blocks[cell.index].isTransparent)
            {
                if (cell.light < light)
                {
                    c.SetCellCliped(pos, BlockData.MakeBlockData(cell.index, light - 1));
                    lightUpdates.Push(pos);
                }
            }
            else
            {
                cell.light = light - LightSettings.GlobalAmbiantLevel;
                c.SetCellCliped(pos, cell);
            }
        }
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
            terrainInstance.UpdateChunk(pos, result.opaque.ToMesh(), result.alpha.ToMesh());
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
                                Block b = BlockManager.blocks[c11[x, y, z].index];
                                b.renderer?.GenerateTerrainVertices(x + bx, y, z + bz, terrain, m);
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
