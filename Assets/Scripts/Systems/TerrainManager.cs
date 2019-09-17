using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    TerrainUpdater updater;

    private void Awake() {
        updater = GetComponent<TerrainUpdater>();
    }

    public Chunk GetChunk(int x, int z)
    {
        var key = new Vector2Int(x, z);
        if (chunks.ContainsKey(key))
        {
            return chunks[new Vector2Int(x, z)];
        }
        // Debug.Log($"chunk {x}, {z} not exist");
        return null;
    }

    public void SetChunk(int x, int z, Chunk chunk)
    {
        chunks[new Vector2Int(x, z)] = chunk;
    }

    public void SetCell(int x, int y, int z, short value)
    {
        int chunkx = x >> Chunk.CHUNK_X_SHIFT;
        int chunkz = z >> Chunk.CHUNK_Z_SHIFT;
        GetChunk(chunkx, chunkz)[x & 15, y, z & 15] = value;
        updater.QueueChunkNeighborUpdate(chunkx, chunkz, 1);
    }

    public Chunk ChunkWithBlock(int x, int y, int z)
    {
        return GetChunk(x >> Chunk.CHUNK_X_SHIFT, z >> Chunk.CHUNK_Z_SHIFT);
    }
}
