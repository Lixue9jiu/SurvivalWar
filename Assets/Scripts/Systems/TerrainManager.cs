﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    TerrainUpdater updater;

    private void Awake() {
        updater = GetComponent<TerrainUpdater>();
    }

    public Chunk GetChunk(Vector2Int pos)
    {
        if (chunks.ContainsKey(pos))
        {
            return chunks[pos];
        }
        // Debug.Log($"chunk {x}, {z} not exist");
        return null;
    }

    public Chunk GetChunk(int x, int z)
    {
        return GetChunk(new Vector2Int(x, z));
    }

    public void SetChunk(int x, int z, Chunk chunk)
    {
        chunks[new Vector2Int(x, z)] = chunk;
    }
    
    public BlockData GetCell(Vector3Int pos)
    {
        return GetCell(pos.x, pos.y, pos.z);
    }

    public BlockData GetCell(int x, int y, int z)
    {
        var pos = new Vector2Int(x >> Chunk.CHUNK_X_SHIFT, z >> Chunk.CHUNK_Z_SHIFT);
        if (chunks.ContainsKey(pos))
        {
            return chunks[pos][x & 15, y & 127, z & 15];
        }
        return 0;
    }

    public void SetCell(Vector3Int pos, BlockData value)
    {
        SetCell(pos.x, pos.y, pos.z, value);
    }

    public void SetCell(int x, int y, int z, BlockData value)
    {
        int chunkx = x >> Chunk.CHUNK_X_SHIFT;
        int chunkz = z >> Chunk.CHUNK_Z_SHIFT;
        int blockx = x & 15;
        int blockz = z & 15;
        var chunk = GetChunk(chunkx, chunkz);
        if (chunk[blockx, y, blockz] == value) return;
        chunk[blockx, y, blockz] = value;
        updater.QueueChunkUpdateImmediate(chunkx, chunkz);
        
        switch (blockx)
        {
            case 0:
                updater.QueueChunkUpdateImmediate(chunkx - 1, chunkz);
                if (blockz == 0)
                    updater.QueueChunkUpdateImmediate(chunkx - 1, chunkz - 1);
                else if (blockz == 15)
                    updater.QueueChunkUpdateImmediate(chunkx - 1, chunkz + 1);
            break;
            case 15:
                updater.QueueChunkUpdateImmediate(chunkx + 1, chunkz);
                if (blockz == 0)
                    updater.QueueChunkUpdateImmediate(chunkx + 1, chunkz - 1);
                else if (blockz == 15)
                    updater.QueueChunkUpdateImmediate(chunkx + 1, chunkz + 1);
            break;
            default:
                if (blockz == 0)
                    updater.QueueChunkUpdateImmediate(chunkx, chunkz - 1);
                else if (blockz == 15)
                    updater.QueueChunkUpdateImmediate(chunkx, chunkz + 1);
            break;
        }
    }

    public Chunk ChunkWithBlock(int x, int y, int z)
    {
        return GetChunk(x >> Chunk.CHUNK_X_SHIFT, z >> Chunk.CHUNK_Z_SHIFT);
    }
}
