using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBlockRenderer : IBlockRenderer
{
    public void GenerateTerrainVertices(int x, int y, int z, Chunk chunk, ChunkMesh mesh)
    {
        mesh.CubeBlock(x, y, z, chunk);
    }
}
