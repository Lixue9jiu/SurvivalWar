using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainInstance : MonoBehaviour
{
    public Material opaqueMaterial;
    public Material alphaMaterial;
    public GameObject chunkPrefab;

    public bool enableStaticBatching = false;

    Dictionary<Vector2Int, ChunkInstance> instances = new Dictionary<Vector2Int, ChunkInstance>();

    public void UpdateChunk(Vector2Int pos, Mesh opaque, Mesh alphaTest)
    {
        ChunkInstance chunk;
        if (instances.ContainsKey(pos))
        {
            chunk = instances[pos];
            chunk.opaqueMesh.mesh = opaque;
            chunk.alphaTestMesh.mesh = alphaTest;
        }
        else
        {
            chunk = Instantiate(chunkPrefab, new Vector3(pos.x << Chunk.CHUNK_X_SHIFT, 0, pos.y << Chunk.CHUNK_Z_SHIFT), Quaternion.identity, transform).GetComponent<ChunkInstance>();
            chunk.opaque.sharedMaterial = opaqueMaterial;
            chunk.alphaTest.sharedMaterial = alphaMaterial;
            instances[pos] = chunk;
        }
        chunk.opaqueMesh.mesh = opaque;
        chunk.alphaTestMesh.mesh = alphaTest;
        if (enableStaticBatching)
            StaticBatchingUtility.Combine(gameObject);
    }
}
