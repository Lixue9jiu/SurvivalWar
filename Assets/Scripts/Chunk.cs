using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public enum ChunkState : byte
    {
        NeedsMeshUpdate,
        GeneratingMesh,
        Good
    }

    public const int CHUNK_X_SHIFT = 4;
    public const int CHUNK_Y_SHIFT = 7;
    public const int CHUNK_Z_SHIFT = 4;
    public const int CHUNK_SIZE_X = 1 << CHUNK_X_SHIFT;
    public const int CHUNK_SIZE_Y = 1 << CHUNK_Y_SHIFT;
    public const int CHUNK_SIZE_Z = 1 << CHUNK_Z_SHIFT;

    public const int SIZE_X_MINUS_ONE = CHUNK_SIZE_X - 1;
    public const int CHUNK_SIZE_Y_MINUS_ONE = CHUNK_SIZE_Y - 1;
    public const int SIZE_Z_MINUS_ONE = CHUNK_SIZE_Z - 1;

    public Mesh chunkMesh;
    public ChunkState chunkState;

    BlockData[] cellData = new BlockData[CHUNK_SIZE_X * CHUNK_SIZE_Y * CHUNK_SIZE_Z];

    public BlockData this[int x, int y, int z]
    {
        get
        {
            return cellData[GetIndex(x, y, z)];
        }
        set
        {
            cellData[GetIndex(x, y, z)] = value;
        }
    }
    public BlockData this[int index]
    {
        get
        {
            return cellData[index];
        }
        set
        {
            cellData[index] = value;
        }
    }
    public static int GetIndex(int x, int y, int z)
    {
        return y + (x << CHUNK_Y_SHIFT) + (z << (CHUNK_Y_SHIFT + CHUNK_X_SHIFT));
    }
}
