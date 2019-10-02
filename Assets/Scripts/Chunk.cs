using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public enum ChunkState : byte
    {
        InvalidLighting,
        NeedsLightUpdate,
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
    public const int SIZE_Y_MINUS_ONE = CHUNK_SIZE_Y - 1;
    public const int SIZE_Z_MINUS_ONE = CHUNK_SIZE_Z - 1;

    public Mesh chunkMesh;
    public ChunkState chunkState;

    BlockData[] cellData = new BlockData[CHUNK_SIZE_X * CHUNK_SIZE_Y * CHUNK_SIZE_Z];
    ShiftData[] shiftData = new ShiftData[CHUNK_SIZE_X * CHUNK_SIZE_Z];

    public BlockData this[int x, int y, int z]
    {
        get
        {
#if DEBUG
            try
            {
                return cellData[GetIndex(x, y, z)];
            }
            catch (System.IndexOutOfRangeException)
            {
                Debug.Log($"{x}, {y}, {z}");
            }
            return 0;
#else
            return cellData[GetIndex(x, y, z)];
#endif
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

    public BlockData GetCellCliped(Vector3Int pos)
    {
        return GetCellCliped(pos.x, pos.y, pos.z);
    }

    public BlockData GetCellCliped(int x, int y, int z)
    {
        return this[x & SIZE_X_MINUS_ONE, y & SIZE_Y_MINUS_ONE, z & SIZE_Z_MINUS_ONE];
    }

    public void SetCellCliped(Vector3Int pos, BlockData data)
    {
        SetCellCliped(pos.x, pos.y, pos.z, data);
    }

    public void SetCellCliped(int x, int y, int z, BlockData data)
    {
        this[x & SIZE_X_MINUS_ONE, y & SIZE_Y_MINUS_ONE, z & SIZE_Z_MINUS_ONE] = data;
    }

    public ShiftData GetShiftData(int x, int z)
    {
        return shiftData[GetShiftIndex(x, z)];
    }

    public void SetShiftData(int x, int z, ShiftData data)
    {
        shiftData[GetShiftIndex(x, z)] = data;
    }

    public static int GetIndex(int x, int y, int z)
    {
        return y + (x << CHUNK_Y_SHIFT) + (z << (CHUNK_Y_SHIFT + CHUNK_X_SHIFT));
    }

    public static int GetShiftIndex(int x, int z)
    {
        return x + (z << CHUNK_X_SHIFT);
    }
}
