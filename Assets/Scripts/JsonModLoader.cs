using System;
using System.Collections.Generic;
using UnityEngine;
using JsonData = LitJson.JsonData;

public class JsonModLoader
{
    static Dictionary<string, object> m_blockRenderers = new Dictionary<string, object>();

    List<Mod> m_mods = new List<Mod>();

    public JsonModLoader()
    {
        JsonDeserializer.SetDeserializer<IBlockRenderer>(ParseBlockRenderer);
        JsonDeserializer.SetDeserializer<Bounds>(ParseBoundingBox);
    }

    public void Load(string json)
    {
        m_mods.Add(JsonDeserializer.Deserialize<Mod>(json));
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

    private object ParseBlockRenderer(JsonData data)
    {
        if (data == null) return null;
        var str = (string)data;
        if (!m_blockRenderers.ContainsKey(str))
        {
            var type = Type.GetType(str, true, true);
            m_blockRenderers[str] = Activator.CreateInstance(type);
        }
        return m_blockRenderers[str];
    }

    private object ParseBoundingBox(JsonData data)
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
