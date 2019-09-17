using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder
{
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    List<Color> colors = new List<Color>();

    public Mesh ToMesh()
    {
        Mesh m = new Mesh();
        m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        m.vertices = vertices.ToArray();
        m.triangles = triangles.ToArray();
        m.uv = uvs.ToArray();
        m.colors = colors.ToArray();
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }

    public void VerticeQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
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

    public void TexQuad(Rect tex)
    {
        uvs.Add(new Vector2(tex.xMin, tex.yMax));
        uvs.Add(tex.max);
        uvs.Add(new Vector2(tex.xMax, tex.yMin));
        uvs.Add(tex.min);
    }

    public void ColorQuad(Color ca, Color cb, Color cc, Color cd)
    {
        colors.Add(ca);
        colors.Add(cb);
        colors.Add(cc);
        colors.Add(cd);
    }

    public void CubeBlock(int x, int y, int z, TerrainManager terrain, Rect[] texCoord, Color color)
    {
        CubeBlock(x, y, z, terrain, texCoord, color, color, color, color);
    }

    public void CubeBlock(int x, int y, int z, TerrainManager terrain, Rect[] texCoord, Color ca, Color cb, Color cc, Color cd)
    {
        Chunk c11 = terrain.ChunkWithBlock(x, y, z);
        Chunk c21 = terrain.ChunkWithBlock(x + 1, y, z);
        Chunk c01 = terrain.ChunkWithBlock(x - 1, y, z);
        Chunk c12 = terrain.ChunkWithBlock(x, y, z + 1);
        Chunk c10 = terrain.ChunkWithBlock(x, y, z - 1);
        int x2 = x & Chunk.SIZE_X_MINUS_ONE;
        int z2 = z & Chunk.SIZE_Z_MINUS_ONE;
        if (BlockManager.blocks[c21[(x + 1) & Chunk.SIZE_X_MINUS_ONE, y, z2]].isTransparent)
        {
            VerticeQuad(new Vector3(x2 + 1, y + 1, z2),
                 new Vector3(x2 + 1, y + 1, z2 + 1),
                 new Vector3(x2 + 1, y, z2 + 1),
                 new Vector3(x2 + 1, y, z2));
            TexQuad(texCoord[CellFace.Left]);
        }
        if (BlockManager.blocks[c01[(x - 1) & Chunk.SIZE_X_MINUS_ONE, y, z2]].isTransparent)
        {
            VerticeQuad(new Vector3(x2, y + 1, z2 + 1),
                 new Vector3(x2, y + 1, z2),
                 new Vector3(x2, y, z2),
                 new Vector3(x2, y, z2 + 1));
            TexQuad(texCoord[CellFace.Right]);            
        }
        if (BlockManager.blocks[c12[x2, y, (z + 1) & Chunk.SIZE_Z_MINUS_ONE]].isTransparent)
        {
            VerticeQuad(new Vector3(x2 + 1, y + 1, z2 + 1),
                 new Vector3(x2, y + 1, z2 + 1),
                 new Vector3(x2, y, z2 + 1),
                 new Vector3(x2 + 1, y, z2 + 1));
            TexQuad(texCoord[CellFace.Front]);
        }
        if (BlockManager.blocks[c10[x2, y, (z - 1) & Chunk.SIZE_Z_MINUS_ONE]].isTransparent)
        {
            VerticeQuad(new Vector3(x2, y + 1, z2),
                 new Vector3(x2 + 1, y + 1, z2),
                 new Vector3(x2 + 1, y, z2),
                 new Vector3(x2, y, z2));
            TexQuad(texCoord[CellFace.Back]);
        }
        if (BlockManager.blocks[c11[x2, y + 1, z2]].isTransparent)
        {
            VerticeQuad(new Vector3(x2, y + 1, z2),
                 new Vector3(x2, y + 1, z2 + 1),
                 new Vector3(x2 + 1, y + 1, z2 + 1),
                 new Vector3(x2 + 1, y + 1, z2));
            TexQuad(texCoord[CellFace.Up]);
        }
        if (BlockManager.blocks[c11[x2, y - 1, z2]].isTransparent)
        {
            VerticeQuad(new Vector3(x2, y, z2),
                 new Vector3(x2 + 1, y, z2),
                 new Vector3(x2 + 1, y, z2 + 1),
                 new Vector3(x2, y, z2 + 1));
            TexQuad(texCoord[CellFace.Down]);
        }
    }
}
