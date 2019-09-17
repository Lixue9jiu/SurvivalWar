using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTextureManager : MonoBehaviour
{
    public Texture2D MainTexture
    {
        get
        {
            return mainAtlas;
        }
    }

    public Rect ErrorBlockTexture {get; private set;}

    private Texture2D mainAtlas;
    [SerializeField]
    private Texture2D errorBlock;

    //Texture2D mainTex;
    Dictionary<string, Rect> texCoords = new Dictionary<string, Rect>();

    private void Start()
    {
        var texs = GetComponent<ModManager>().GetBlockTextures();

        mainAtlas = new Texture2D(512, 512, TextureFormat.ARGB32, 6, false);
        mainAtlas.filterMode = FilterMode.Point;
        mainAtlas.mipMapBias = -0.5f;
        //uvs = mainTex.PackTextures(texs, 0, 512, true);
        PackTextures(mainAtlas, texs, true);
    }

    private void PackTextures(Texture2D src, Texture2D[] texs, bool apply)
    {
        int mipmapCount = 6;
        for (int i = 0; i < mipmapCount; i++)
        {
            int texSize = 32 >> i;
            for (int k = 0; k < texs.Length; k++)
            {
                src.SetPixels((k & 15) * texSize, (k >> 4) * texSize, texSize, texSize, texs[k].GetPixels(i), i);
                texCoords[texs[k].name] = GenerateTexCoord(k);
            }
        }
        ErrorBlockTexture = GenerateTexCoord(texs.Length);
        src.Apply(true, apply);
    }

    public Rect FindBlockTexture(string name)
    {
        if (!texCoords.ContainsKey(name))
            throw new System.Exception($"block texture \"{name}\" not found");
        return texCoords[name];
    }

    private static Rect GenerateTexCoord(int index)
    {
        return new Rect(0.0625f * (index & 15), 0.0625f * (index >> 4), 0.0625f, 0.0625f);
    }
}