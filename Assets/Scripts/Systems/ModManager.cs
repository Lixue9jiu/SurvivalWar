using System.IO;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;

public class ModManager : MonoBehaviour
{
    const string testing_mod_path = "Assets/CoreMod";
    static Dictionary<string, Type> m_blockRenderers = new Dictionary<string, Type>();

    List<Mod> m_mods = new List<Mod>();

    private void Awake() {
        JsonDeserializer.SetDeserializer<IBlockRenderer>(ParseBlockRenderer);
        JsonDeserializer.SetDeserializer<Bounds>(ParseBoundingBox);
    }

    private void Start() {
        LoadMod(testing_mod_path);
    }

    public void LoadMod(string path)
    {
        using (TextReader reader = File.OpenText(Path.Combine(path, "config.json")))
        {
            var mod = JsonDeserializer.Deserialize<Mod>(reader.ReadToEnd());
            mod.path = path;
            m_mods.Add(mod);
        }
    }

    public Block[] GetBlocks()
    {
        List<Block> blocks = new List<Block>();
        foreach (Mod mod in m_mods)
        {
            blocks.AddRange(mod.blocks);
        }
        return blocks.ToArray();
    }

    public Texture2D[] GetBlockTextures()
    {
        List<Texture2D> textures = new List<Texture2D>();
        foreach (Mod mod in m_mods)
        {
            if (!string.IsNullOrEmpty(mod.blockTextureDirectory))
            {
                foreach (string path in Directory.GetFiles(Path.Combine(mod.path, mod.blockTextureDirectory)))
                {
                    if (path.EndsWith(".png"))
                    {
                        Texture2D tex = new Texture2D(32, 32, TextureFormat.ARGB32, 6, false);
                        ImageConversion.LoadImage(tex, File.ReadAllBytes(path));
                        tex.name = Path.GetFileNameWithoutExtension(path);
                        textures.Add(tex);
                    }
                }
            }
        }
        return textures.ToArray();
    }

    private static object ParseBlockRenderer(JsonData data)
    {
        if (data == null) return null;
        var str = (string)data["name"];
        if (!m_blockRenderers.ContainsKey(str))
        {
            m_blockRenderers[str] = Type.GetType(str, true, true);
        }
        var result = JsonDeserializer.Deserialize(m_blockRenderers[str], data["data"]);
        return result;
    }

    private static object ParseBoundingBox(JsonData data)
    {
        var strNums = ((string)data).Split(' ');
        if (strNums.Length != 6) throw new Exception("cannot read bounding box: " + data);
        var nums = new float[6];
        for (int k = 0; k < 6; k++)
        {
            nums[k] = (float)double.Parse(strNums[k]);
        }
        return new Bounds(new Vector3(nums[0], nums[1], nums[2]), new Vector3(nums[3], nums[4], nums[5]));
    }
}
