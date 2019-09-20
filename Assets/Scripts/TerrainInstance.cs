using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainInstance : MonoBehaviour
{
    public Material opaqueMaterial;
    public Material alphaMaterial;
    public GameObject chunkPrefab;

    public bool enableStaticBatching = false;

    Dictionary<Vector2Int, MeshFilter> instances = new Dictionary<Vector2Int, MeshFilter>();

    public void UpdateChunk(Vector2Int pos, Mesh mesh)
    {
        MeshFilter chunk;
        if (instances.ContainsKey(pos))
        {
            chunk = instances[pos];
        }
        else
        {
            var obj = Instantiate(chunkPrefab, new Vector3(pos.x << Chunk.CHUNK_X_SHIFT, 0, pos.y << Chunk.CHUNK_Z_SHIFT), Quaternion.identity, transform);
            chunk = obj.GetComponent<MeshFilter>();
            chunk.GetComponent<MeshRenderer>().sharedMaterials = new Material[]{ opaqueMaterial, alphaMaterial };
            instances[pos] = chunk;
        }
        chunk.mesh = mesh;
        if (enableStaticBatching)
            StaticBatchingUtility.Combine(gameObject);
    }
}
