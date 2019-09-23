using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder
{
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    List<Color> colors = new List<Color>();

    public bool useAlphaTest = false;

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

    // see TestTriangleDirection for the input of triangleDirection
    public void ShadedVerticeQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float la, float lb, float lc, float ld)
    {
        int count = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        vertices.Add(d);

        if (TestTriangleDirection(la, lb, lc, ld))
        {
            triangles.Add(count);
            triangles.Add(count + 1);
            triangles.Add(count + 2);
            triangles.Add(count + 2);
            triangles.Add(count + 3);
            triangles.Add(count);
        }
        else
        {
            triangles.Add(count);
            triangles.Add(count + 1);
            triangles.Add(count + 3);
            triangles.Add(count + 1);
            triangles.Add(count + 2);
            triangles.Add(count + 3);
        }
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

    /*
        return false when:
          a ***** b
            *   *
            **  *
            * * *
            *  **
          d ***** c
        return true when:
          a ***** b
            *   *
            *  **
            * * *
            **  *
          d ***** c
     */
    public bool TestTriangleDirection(float a, float b, float c, float d)
    {
        return Mathf.Abs(a - c) < Mathf.Abs(b - d);
        // return a + c > b + d;
    }

    public void CubeBlock(int x, int y, int z, TerrainManager terrain, Rect[] texCoord, Color color)
    {
        CubeBlock(x, y, z, terrain, texCoord, color, color, color, color);
    }

    /*
        c02 c12 c22
        c01 c11 c21
        c00 c10 c20
     */
    public void CubeBlock(int x, int y, int z, TerrainManager terrain, Rect[] texCoord, Color ca, Color cb, Color cc, Color cd)
    {
        Chunk c11 = terrain.ChunkWithBlock(x, y, z);

        Chunk c21 = terrain.ChunkWithBlock(x + 1, y, z);
        Chunk c01 = terrain.ChunkWithBlock(x - 1, y, z);
        Chunk c12 = terrain.ChunkWithBlock(x, y, z + 1);
        Chunk c10 = terrain.ChunkWithBlock(x, y, z - 1);

        Chunk c02 = terrain.ChunkWithBlock(x - 1, y, z + 1);
        Chunk c22 = terrain.ChunkWithBlock(x + 1, y, z + 1);
        Chunk c00 = terrain.ChunkWithBlock(x - 1, y, z - 1);
        Chunk c20 = terrain.ChunkWithBlock(x + 1, y, z - 1);

        int x2 = x & Chunk.SIZE_X_MINUS_ONE;
        int z2 = z & Chunk.SIZE_Z_MINUS_ONE;

        // l is short for light
        // light intencity at a, b, c, d
        float la, lb, lc, ld;

        // cell face: x positive
        if (BlockManager.blocks[c21[(x + 1) & Chunk.SIZE_X_MINUS_ONE, y, z2].index].isTransparent)
        {
            la = CalculateLighting(c20[ClipX(x2 + 1), y, ClipZ(z2 - 1)].lightF, c20[ClipX(x2 + 1), y + 1, ClipZ(z2 - 1)].lightF, c21[ClipX(x2 + 1), y + 1, z2].lightF, c21[ClipX(x2 + 1), y, z2].lightF);
            lb = CalculateLighting(c21[ClipX(x2 + 1), y, z2].lightF, c21[ClipX(x2 + 1), y + 1, z2].lightF, c22[ClipX(x2 + 1), y + 1, ClipZ(z2 + 1)].lightF, c22[ClipX(x2 + 1), y, ClipZ(z2 + 1)].lightF);
            lc = CalculateLighting(c21[ClipX(x2 + 1), y - 1, z2].lightF, c21[ClipX(x2 + 1), y, z2].lightF, c22[ClipX(x2 + 1), y, ClipZ(z2 + 1)].lightF, c22[ClipX(x2 + 1), y - 1, ClipZ(z2 + 1)].lightF);
            ld = CalculateLighting(c20[ClipX(x2 + 1), y - 1, ClipZ(z2 - 1)].lightF, c20[ClipX(x2 + 1), y, ClipZ(z2 - 1)].lightF, c21[ClipX(x2 + 1), y, z2].lightF, c21[ClipX(x2 + 1), y - 1, z2].lightF);
            ShadedVerticeQuad(new Vector3(x2 + 1, y + 1, z2),
                 new Vector3(x2 + 1, y + 1, z2 + 1),
                 new Vector3(x2 + 1, y, z2 + 1),
                 new Vector3(x2 + 1, y, z2),
                 la, lb, lc, ld);
            TexQuad(texCoord[CellFace.Left]);
            ColorQuad(
                ca * la,
                cb * lb,
                cc * lc,
                cd * ld);
        }
        // cell face: x negative
        if (BlockManager.blocks[c01[(x - 1) & Chunk.SIZE_X_MINUS_ONE, y, z2].index].isTransparent)
        {
            la = CalculateLighting(c01[ClipX(x2 - 1), y, z2].lightF, c01[ClipX(x2 - 1), y + 1, z2].lightF, c02[ClipX(x2 - 1), y + 1, ClipZ(z2 + 1)].lightF, c02[ClipX(x2 - 1), y, ClipZ(z2 + 1)].lightF);
            lb = CalculateLighting(c00[ClipX(x2 - 1), y, ClipZ(z2 - 1)].lightF, c00[ClipX(x2 - 1), y + 1, ClipZ(z2 - 1)].lightF, c01[ClipX(x2 - 1), y + 1, z2].lightF, c01[ClipX(x2 - 1), y, z2].lightF);
            lc = CalculateLighting(c00[ClipX(x2 - 1), y - 1, ClipZ(z2 - 1)].lightF, c00[ClipX(x2 - 1), y, ClipZ(z2 - 1)].lightF, c01[ClipX(x2 - 1), y, z2].lightF, c01[ClipX(x2 - 1), y - 1, z2].lightF);
            ld = CalculateLighting(c01[ClipX(x2 - 1), y - 1, z2].lightF, c01[ClipX(x2 - 1), y, z2].lightF, c02[ClipX(x2 - 1), y, ClipZ(z2 + 1)].lightF, c02[ClipX(x2 - 1), y - 1, ClipZ(z2 + 1)].lightF);
            ShadedVerticeQuad(new Vector3(x2, y + 1, z2 + 1),
                 new Vector3(x2, y + 1, z2),
                 new Vector3(x2, y, z2),
                 new Vector3(x2, y, z2 + 1),
                 la, lb, lc, ld);
            TexQuad(texCoord[CellFace.Right]);
            ColorQuad(
                ca * la,
                cb * lb,
                cc * lc,
                cd * ld);
        }
        // cell face: z positive
        if (BlockManager.blocks[c12[x2, y, (z + 1) & Chunk.SIZE_Z_MINUS_ONE].index].isTransparent)
        {
            la = CalculateLighting(c22[ClipX(x2 + 1), y + 1, ClipZ(z2 + 1)].lightF, c12[x2, y + 1, ClipZ(z2 + 1)].lightF, c22[ClipX(x2 + 1), y, ClipZ(z2 + 1)].lightF, c12[x2, y, ClipZ(z2 + 1)].lightF);
            lb = CalculateLighting(c12[x2, y + 1, ClipZ(z2 + 1)].lightF, c02[ClipX(x2 - 1), y + 1, ClipZ(z2 + 1)].lightF, c12[x2, y, ClipZ(z2 + 1)].lightF, c02[ClipX(x2 - 1), y, ClipZ(z2 + 1)].lightF);
            lc = CalculateLighting(c12[x2, y, ClipZ(z2 + 1)].lightF, c02[ClipX(x2 - 1), y, ClipZ(z2 + 1)].lightF, c12[x2, y - 1, ClipZ(z2 + 1)].lightF, c02[ClipX(x2 - 1), y - 1, ClipZ(z2 + 1)].lightF);
            ld = CalculateLighting(c22[ClipX(x2 + 1), y, ClipZ(z2 + 1)].lightF, c12[x2, y, ClipZ(z2 + 1)].lightF, c22[ClipX(x2 + 1), y - 1, ClipZ(z2 + 1)].lightF, c12[x2, y - 1, ClipZ(z2 + 1)].lightF);
            ShadedVerticeQuad(new Vector3(x2 + 1, y + 1, z2 + 1),
                 new Vector3(x2, y + 1, z2 + 1),
                 new Vector3(x2, y, z2 + 1),
                 new Vector3(x2 + 1, y, z2 + 1),
                 la, lb, lc, ld);
            TexQuad(texCoord[CellFace.Front]);
            ColorQuad(
                ca * la,
                cb * lb,
                cc * lc,
                cd * ld);
        }
        // cell face: z negative
        if (BlockManager.blocks[c10[x2, y, (z - 1) & Chunk.SIZE_Z_MINUS_ONE].index].isTransparent)
        {
            la = CalculateLighting(c10[x2, y + 1, ClipZ(z2 - 1)].lightF, c00[ClipX(x2 - 1), y + 1, ClipZ(z2 - 1)].lightF, c10[x2, y, ClipZ(z2 - 1)].lightF, c00[ClipX(x2 - 1), y, ClipZ(z2 - 1)].lightF);
            lb = CalculateLighting(c20[ClipX(x2 + 1), y + 1, ClipZ(z2 - 1)].lightF, c10[x2, y + 1, ClipZ(z2 - 1)].lightF, c20[ClipX(x2 + 1), y, ClipZ(z2 - 1)].lightF, c10[x2, y, ClipZ(z2 - 1)].lightF);
            lc = CalculateLighting(c20[ClipX(x2 + 1), y, ClipZ(z2 - 1)].lightF, c10[x2, y, ClipZ(z2 - 1)].lightF, c20[ClipX(x2 + 1), y - 1, ClipZ(z2 - 1)].lightF, c10[x2, y - 1, ClipZ(z2 - 1)].lightF);
            ld = CalculateLighting(c10[x2, y, ClipZ(z2 - 1)].lightF, c00[ClipX(x2 - 1), y, ClipZ(z2 - 1)].lightF, c10[x2, y - 1, ClipZ(z2 - 1)].lightF, c00[ClipX(x2 - 1), y - 1, ClipZ(z2 - 1)].lightF);
            ShadedVerticeQuad(new Vector3(x2, y + 1, z2),
                 new Vector3(x2 + 1, y + 1, z2),
                 new Vector3(x2 + 1, y, z2),
                 new Vector3(x2, y, z2),
                 la, lb, lc, ld);
            TexQuad(texCoord[CellFace.Back]);
            ColorQuad(
                ca * la,
                cb * lb,
                cc * lc,
                cd * ld);
        }
        // cell face: y positive
        if (BlockManager.blocks[c11[x2, y + 1, z2].index].isTransparent)
        {
            la = CalculateLighting(c00[ClipX(x2 - 1), y + 1, ClipZ(z2 - 1)].lightF, c01[ClipX(x2 - 1), y + 1, z2].lightF, c11[x2, y + 1, z2].lightF, c10[x2, y + 1, ClipZ(z2 - 1)].lightF);
            lb = CalculateLighting(c01[ClipX(x2 - 1), y + 1, z2].lightF, c02[ClipX(x2 - 1), y + 1, ClipZ(z2 + 1)].lightF, c12[x2, y + 1, ClipZ(z2 + 1)].lightF, c11[x2, y + 1, z2].lightF);
            lc = CalculateLighting(c11[x2, y + 1, z2].lightF, c12[x2, y + 1, ClipZ(z2 + 1)].lightF, c22[ClipX(x2 + 1), y + 1, ClipZ(z2 + 1)].lightF, c21[ClipX(x2 + 1), y + 1, z2].lightF);
            ld = CalculateLighting(c10[x2, y + 1, ClipZ(z2 - 1)].lightF, c11[x2, y + 1, z2].lightF, c21[ClipX(x2 + 1), y + 1, z2].lightF, c20[ClipX(x2 + 1), y + 1, ClipZ(z2 - 1)].lightF);
            ShadedVerticeQuad(new Vector3(x2, y + 1, z2),
                 new Vector3(x2, y + 1, z2 + 1),
                 new Vector3(x2 + 1, y + 1, z2 + 1),
                 new Vector3(x2 + 1, y + 1, z2),
                 la, lb, lc, ld);
            TexQuad(texCoord[CellFace.Up]);
            ColorQuad(
                ca * la,
                cb * lb,
                cc * lc,
                cd * ld);
        }
        // cell face: y negative
        if (BlockManager.blocks[c11[x2, y - 1, z2].index].isTransparent)
        {
            la = CalculateLighting(c01[ClipX(x2 - 1), y - 1, z2].lightF, c02[ClipX(x2 - 1), y - 1, ClipZ(z2 + 1)].lightF, c12[x2, y - 1, ClipZ(z2 + 1)].lightF, c11[x2, y - 1, z2].lightF);
            lb = CalculateLighting(c00[ClipX(x2 - 1), y - 1, ClipZ(z2 - 1)].lightF, c01[ClipX(x2 - 1), y - 1, z2].lightF, c11[x2, y - 1, z2].lightF, c10[x2, y - 1, ClipZ(z2 - 1)].lightF);
            lc = CalculateLighting(c10[x2, y - 1, ClipZ(z2 - 1)].lightF, c11[x2, y - 1, z2].lightF, c21[ClipX(x2 + 1), y - 1, z2].lightF, c20[ClipX(x2 + 1), y - 1, ClipZ(z2 - 1)].lightF);
            ld = CalculateLighting(c11[x2, y - 1, z2].lightF, c12[x2, y - 1, ClipZ(z2 + 1)].lightF, c22[ClipX(x2 + 1), y - 1, ClipZ(z2 + 1)].lightF, c21[ClipX(x2 + 1), y - 1, z2].lightF);
            ShadedVerticeQuad(new Vector3(x2, y, z2),
                 new Vector3(x2 + 1, y, z2),
                 new Vector3(x2 + 1, y, z2 + 1),
                 new Vector3(x2, y, z2 + 1),
                 la, lb, lc, ld);
            TexQuad(texCoord[CellFace.Down]);
            ColorQuad(
                ca * la,
                cb * lb,
                cc * lc,
                cd * ld);
        }
    }

    public static int ClipX(int x)
    {
        return x & Chunk.SIZE_X_MINUS_ONE;
    }

    public static int ClipZ(int z)
    {
        return z & Chunk.SIZE_Z_MINUS_ONE;
    }

    public static float CalculateLighting(float a, float b, float c, float d)
    {
        return a * b * c * d;
        // return Mathf.Min(a, b, c, d);
    }
}
