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

    public void CubeBlock(int x, int y, int z, Chunk c)
    {
        if (x == 0 || x == Chunk.CHUNK_SIZE_X - 1 ||
            y == 0 || y == Chunk.CHUNK_SIZE_Y - 1 ||
            z == 0 || z == Chunk.CHUNK_SIZE_Z - 1)
            return;
        
        if (BlockManager.blocks[c[x + 1, y, z]].isTransparent)
        {
            Quad(new Vector3(x + 1, y + 1, z),
                 new Vector3(x + 1, y + 1, z + 1),
                 new Vector3(x + 1, y, z + 1),
                 new Vector3(x + 1, y, z));
        }
        if (BlockManager.blocks[c[x - 1, y, z]].isTransparent)
        {
            Quad(new Vector3(x, y + 1, z + 1),
                 new Vector3(x, y + 1, z),
                 new Vector3(x, y, z),
                 new Vector3(x, y, z + 1));
        }
        if (BlockManager.blocks[c[x, y, z + 1]].isTransparent)
        {
            Quad(new Vector3(x + 1, y + 1, z + 1),
                 new Vector3(x, y + 1, z + 1),
                 new Vector3(x, y, z + 1),
                 new Vector3(x + 1, y, z + 1));
        }
        if (BlockManager.blocks[c[x, y, z - 1]].isTransparent)
        {
            Quad(new Vector3(x, y + 1, z),
                 new Vector3(x + 1, y + 1, z),
                 new Vector3(x + 1, y, z),
                 new Vector3(x, y, z));
        }
        if (BlockManager.blocks[c[x, y + 1, z]].isTransparent)
        {
            Quad(new Vector3(x, y + 1, z),
                 new Vector3(x, y + 1, z + 1),
                 new Vector3(x + 1, y + 1, z + 1),
                 new Vector3(x + 1, y + 1, z));
        }
        if (BlockManager.blocks[c[x, y - 1, z]].isTransparent)
        {
            Quad(new Vector3(x, y, z),
                 new Vector3(x + 1, y, z),
                 new Vector3(x + 1, y, z + 1),
                 new Vector3(x, y, z + 1));
        }
    }
}
