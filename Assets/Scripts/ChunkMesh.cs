using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMesh
{
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    public Mesh ToMesh()
    {
        Mesh m = new Mesh();
        m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        m.vertices = vertices.ToArray();
        m.triangles = triangles.ToArray();
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }

    public void Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        int count = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        vertices.Add(d);

        triangles.Add(count);
        triangles.Add(count + 1);
        triangles.Add(count + 3);
        triangles.Add(count + 1);
        triangles.Add(count + 2);
        triangles.Add(count + 3);
    }

    public void CubeBlock(int x, int y, int z, TerrainManager terrain)
    {
        Chunk c11 = terrain.ChunkWithBlock(x, y, z);
        Chunk c21 = terrain.ChunkWithBlock(x + 1, y, z);
        Chunk c01 = terrain.ChunkWithBlock(x - 1, y, z);
        Chunk c12 = terrain.ChunkWithBlock(x, y, z + 1);
        Chunk c10 = terrain.ChunkWithBlock(x, y, z - 1);
        if (BlockManager.blocks[c21[x + 1, y, z]].isTransparent)
        {
            Quad(new Vector3(x + 1, y + 1, z),
                 new Vector3(x + 1, y + 1, z + 1),
                 new Vector3(x + 1, y, z + 1),
                 new Vector3(x + 1, y, z));
        }
        if (BlockManager.blocks[c01[x - 1, y, z]].isTransparent)
        {
            Quad(new Vector3(x, y + 1, z + 1),
                 new Vector3(x, y + 1, z),
                 new Vector3(x, y, z),
                 new Vector3(x, y, z + 1));
        }
        if (BlockManager.blocks[c12[x, y, z + 1]].isTransparent)
        {
            Quad(new Vector3(x + 1, y + 1, z + 1),
                 new Vector3(x, y + 1, z + 1),
                 new Vector3(x, y, z + 1),
                 new Vector3(x + 1, y, z + 1));
        }
        if (BlockManager.blocks[c10[x, y, z - 1]].isTransparent)
        {
            Quad(new Vector3(x, y + 1, z),
                 new Vector3(x + 1, y + 1, z),
                 new Vector3(x + 1, y, z),
                 new Vector3(x, y, z));
        }
        if (BlockManager.blocks[c11[x, y + 1, z]].isTransparent)
        {
            Quad(new Vector3(x, y + 1, z),
                 new Vector3(x, y + 1, z + 1),
                 new Vector3(x + 1, y + 1, z + 1),
                 new Vector3(x + 1, y + 1, z));
        }
        if (BlockManager.blocks[c11[x, y - 1, z]].isTransparent)
        {
            Quad(new Vector3(x, y, z),
                 new Vector3(x + 1, y, z),
                 new Vector3(x + 1, y, z + 1),
                 new Vector3(x, y, z + 1));
        }
    }
}
