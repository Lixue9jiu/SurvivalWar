using UnityEngine;

public class TerrainGenerator
{
    public Chunk GenerateTerrain(int cx, int cz)
    {
        Chunk c = new Chunk();
        int bx = cx << Chunk.CHUNK_X_SHIFT;
        int bz = cz << Chunk.CHUNK_Z_SHIFT;
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                for (int y = 0; y < 128; y++)
                {
                    int h = GetTerrainHeight(x + bx, z + bz);
                    if (y < h)
                    {
                        c[x, y, z] = 2;
                    }
                    else if (y - 1 < h)
                    {
                        c[x, y, z] = 3;
                    }
                    // if (y < bx + bz)
                    //     c[x, y, z] = 1;
                }
            }
        }
        return c;
    }

    public int GetTerrainHeight(int x, int z)
    {
        return (int)(Mathf.Pow(2, Mathf.PerlinNoise(x / 64f, z / 64f) * 2 - 1) * 32 +
            Mathf.PerlinNoise(x / 32f, z / 32f) * 32 + 
            Mathf.PerlinNoise(x / 16f, z / 16f) * 16);
    }
}