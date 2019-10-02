using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainInstance : MonoBehaviour
{
    public Material opaqueMaterial;
    public Material alphaMaterial;
    public GameObject chunkPrefab;

    public bool enableStaticBatching = false;

    class ChunkInstance
    {
        public MeshFilter opaqueMeshFilter;
        public MeshFilter alphaTestMeshFilter;
        public MeshCollider opaqueMeshCollider;
        public MeshCollider alphaTestMeshCollider;
    }

    Dictionary<Vector2Int, ChunkInstance> instances = new Dictionary<Vector2Int, ChunkInstance>();

    // an array of MeshFilters, with opaque mesh stored in 0; alpha test mesh stored in 1
    public void UpdateChunk(Vector2Int pos, Mesh opaque, Mesh alphaTest)
    {
        ChunkInstance chunk;
        if (instances.ContainsKey(pos))
        {
            chunk = instances[pos];
        }
        else
        {
            chunk = new ChunkInstance();
            instances[pos] = chunk;

            var obj = Instantiate(chunkPrefab, new Vector3(pos.x << Chunk.CHUNK_X_SHIFT, 0, pos.y << Chunk.CHUNK_Z_SHIFT), Quaternion.identity, transform);
            chunk.opaqueMeshFilter = obj.GetComponent<MeshFilter>();
            chunk.opaqueMeshCollider = obj.GetComponent<MeshCollider>();
            obj.GetComponent<MeshRenderer>().sharedMaterial = opaqueMaterial;

            var obj2 = Instantiate(chunkPrefab, new Vector3(pos.x << Chunk.CHUNK_X_SHIFT, 0, pos.y << Chunk.CHUNK_Z_SHIFT), Quaternion.identity, transform);
            chunk.alphaTestMeshFilter = obj2.GetComponent<MeshFilter>();
            chunk.alphaTestMeshCollider = obj2.GetComponent<MeshCollider>();
            obj2.GetComponent<MeshRenderer>().sharedMaterial = alphaMaterial;
        }

        chunk.opaqueMeshFilter.mesh = opaque;
        chunk.alphaTestMeshFilter.mesh = alphaTest;
        chunk.opaqueMeshCollider.sharedMesh = opaque;
        chunk.alphaTestMeshCollider.sharedMesh = alphaTest;
        if (enableStaticBatching)
            StaticBatchingUtility.Combine(gameObject);
    }
}
