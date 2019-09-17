using System;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public static class JsonDeserializer
{
    static Dictionary<Type, Func<JsonData, object>> m_deserializers = new Dictionary<Type, Func<JsonData, object>>();

    static JsonDeserializer()
    {
        SetDeserializer<bool>((obj) => (bool)obj);
        SetDeserializer<int>((obj) => (int)obj);
        SetDeserializer<long>((obj) => (long)obj);
        SetDeserializer<float>((obj) => (float)obj);
        SetDeserializer<double>((obj) => (double)obj);
        SetDeserializer<string>((obj) => (string)obj);
        SetDeserializer<JsonData>((obj) => obj);
    }

    public static T Deserialize<T>(string json)
    {
        return Deserialize<T>(JsonMapper.ToObject(json));
    }

    public static T Deserialize<T>(JsonData json)
    {
        return (T) Deserialize(typeof(T), json);
    }

    public static void SetDeserializer<T>(Func<JsonData, object> func)
    {
        m_deserializers[typeof(T)] = func;
    }

    public static object Deserialize(Type type, JsonData data)
    {
        if (m_deserializers.ContainsKey(type))
        {
            return m_deserializers[type](data);
        }
        if (type.IsArray && data.IsArray)
        {
            var array = (Array)Activator.CreateInstance(type, new object[]{ data.Count });
            for (int i = 0; i < data.Count; i++)
            {
                array.SetValue(Deserialize(type.GetElementType(), data[i]), i);
            }
            return array;
        }
        if (data.IsObject)
            return DeserializeMap(type, data);
        throw new Exception("cannot deserialize: " + data.ToString());
    }

    static object DeserializeMap(Type type, JsonData map)
    {
        object obj = Activator.CreateInstance(type);
        foreach (string key in map.Keys)
        {
            var f = type.GetField(key);
            if (f != null)
            {
                f.SetValue(obj, Deserialize(f.FieldType, map[key]));
            }
        }
        return obj;
    }

    // void ResolveDependencies(JsonData map)
    // {
    //     // resolve dependencies
    //     Dictionary<string, JsonData> archetypes = new Dictionary<string, JsonData>();
    //     if (map.ContainsKey("archetypes"))
    //     {
    //         foreach (JsonData d in map["archetypes"])
    //         {
    //             archetypes[(string)d["name"]] = d;
    //         }
    //     }
    //     foreach (string key in map)
    //     {
    //         if (key != "archetypes")
    //             map[key] = ResolveDependency(map[key], archetypes);
    //     }
    // }

    // public JsonData ResolveDependency(JsonData data, Dictionary<string, JsonData> archetypes)
    // {
    //     if (data.IsArray)
    //     {
    //         for (int i = 0; i < data.Count; i++)
    //         {
    //             data[i] = ResolveDependency(data[i], archetypes);
    //         }
    //     }
    //     else if (data.IsObject && data.ContainsKey("archetype"))
    //     {
    //         var archName = (string)data["archetype"];
    //         var arch = archetypes[archName];
            
    //         var result = new JsonData();
    //         result.SetJsonType(JsonType.Object);
    //         foreach (string key in data.Keys)
    //         {
    //             if (key != "archetype")
    //                 result[key] = data[key];
    //         }
    //         foreach (string key in arch["content"].Keys)
    //         {
    //             if (!result.ContainsKey(key))
    //                 result[key] = arch[key];
    //         }
    //         data = result;
    //     }
    //     return data;
    // }
}
