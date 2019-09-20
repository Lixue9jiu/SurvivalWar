using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMesh
{
    MeshBuilder m_meshBuilder = new MeshBuilder();

    public MeshBuilder opaque
    {
        get
        {
            m_meshBuilder.useAlphaTest = false;
            return m_meshBuilder;
        }
    }
    public MeshBuilder alpha
    {
        get
        {
            m_meshBuilder.useAlphaTest = true;
            return m_meshBuilder;
        }
    }

    public Mesh GetMesh()
    {
        return m_meshBuilder.ToMesh();
    }
}
