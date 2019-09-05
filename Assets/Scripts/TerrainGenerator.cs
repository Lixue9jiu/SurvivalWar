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
                    if (y < Mathf.PerlinNoise((float)(x + bx) / 16f, (float)(z + bz) / 16f) * 16)
                    {
                        c[x, y, z] = 1;
                    }
                    //if (y < bx + bz)
                        //c[x, y, z] = 1;
                }
            }
        }
        return c;
    }
}