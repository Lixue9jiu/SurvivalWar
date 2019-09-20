using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainInstance : MonoBehaviour
{
    public const int TerrainMeshCount = 2;

    public Material opaqueMaterial;
    public Material alphaMaterial;
    public GameObject chunkPrefab;

    public bool enableStaticBatching = false;

    Dictionary<Vector2Int, MeshFilter[]> instances = new Dictionary<Vector2Int, MeshFilter[]>();

    // an array of MeshFilters, with opaque mesh stored in 0; alpha test mesh stored in 1
    public void UpdateChunk(Vector2Int pos, Mesh opaque, Mesh alphaTest)
    {
        MeshFilter[] chunk;
        if (instances.ContainsKey(pos))
        {
            chunk = instances[pos];
        }
        else
        {
            chunk = new MeshFilter[TerrainMeshCount];
            instances[pos] = chunk;

            var obj = Instantiate(chunkPrefab, new Vector3(pos.x << Chunk.CHUNK_X_SHIFT, 0, pos.y << Chunk.CHUNK_Z_SHIFT), Quaternion.identity, transform);
            chunk[0] = obj.GetComponent<MeshFilter>();
            obj.GetComponent<MeshRenderer>().sharedMaterial = opaqueMaterial;

            var obj2 = Instantiate(chunkPrefab, new Vector3(pos.x << Chunk.CHUNK_X_SHIFT, 0, pos.y << Chunk.CHUNK_Z_SHIFT), Quaternion.identity, transform);
            chunk[1] = obj2.GetComponent<MeshFilter>();
            obj2.GetComponent<MeshRenderer>().sharedMaterial = alphaMaterial;
        }

        chunk[0].mesh = opaque;
        chunk[1].mesh = alphaTest;
        if (enableStaticBatching)
            StaticBatchingUtility.Combine(gameObject);
    }
}
