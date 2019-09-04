using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBlockRenderer
{
    void GenerateTerrainVertices(int x, int y, int z, TerrainManager terrain, ChunkMesh mesh);
}
