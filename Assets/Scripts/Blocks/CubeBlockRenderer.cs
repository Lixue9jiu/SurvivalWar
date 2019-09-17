using UnityEngine;

public class CubeBlockRenderer : IBlockRenderer
{
    public string texture;
    public string[] textures;
    public bool useAlpha = false;
    private Rect[] texCoords;

    public void Initialize()
    {
        texCoords = new Rect[6];
        var tm = GlobalObject.Get<BlockTextureManager>();
        if (textures != null)
        {
            for (int i = 0; i < 6; i++)
            {
                texCoords[i] = tm.FindBlockTexture(textures[i]);
            }
        }
        else
        {
            Rect r = string.IsNullOrEmpty(texture) ? tm.ErrorBlockTexture : tm.FindBlockTexture(texture);
            for (int i = 0; i < 6; i++)
            {
                texCoords[i] = r;
            }
        }
    }

    public void GenerateTerrainVertices(int x, int y, int z, TerrainManager terrain, ChunkMesh mesh)
    {
        (useAlpha ? mesh.alpha : mesh.opaque).CubeBlock(x, y, z, terrain, texCoords, Color.white);
    }
}
