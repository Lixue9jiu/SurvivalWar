using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBlockRenderer : IBlockRenderer
{
    public void GenerateTerrainVertices(int x, int y, int z, TerrainManager terrain, ChunkMesh mesh)
    {
        mesh.CubeBlock(x, y, z, terrain);
    }
}
