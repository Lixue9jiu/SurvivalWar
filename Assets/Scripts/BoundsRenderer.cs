using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class BoundsRenderer : MonoBehaviour
{
    const float bounding_box_expand = 0.01f;
    private Vector3 min = Vector3.zero;
    private Vector3 max = Vector3.one;

    public Material material;
    public Bounds bounds
    {
        set
        {
            value.Expand(bounding_box_expand);
            min = value.min;
            max = value.max;
        }
    }

    private void OnRenderObject() {
        material.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);

        GL.Begin(GL.LINES);
        GL.Vertex(min);
        GL.Vertex3(max.x, min.y, min.z);
        GL.Vertex(min);
        GL.Vertex3(min.x, max.y, min.z);
        GL.Vertex(min);
        GL.Vertex3(min.x, min.y, max.z);

        GL.Vertex(max);
        GL.Vertex3(min.x, max.y, max.z);
        GL.Vertex(max);
        GL.Vertex3(max.x, min.y, max.z);
        GL.Vertex(max);
        GL.Vertex3(max.x, max.y, min.z);

        GL.Vertex3(min.x, max.y, min.z);
        GL.Vertex3(min.x, max.y, max.z);
        GL.Vertex3(min.x, max.y, min.z);
        GL.Vertex3(max.x, max.y, min.z);
        GL.Vertex3(max.x, min.y, max.z);
        GL.Vertex3(min.x, min.y, max.z);
        GL.Vertex3(max.x, min.y, max.z);
        GL.Vertex3(max.x, min.y, min.z);
        GL.Vertex3(max.x, min.y, min.z);
        GL.Vertex3(max.x, max.y, min.z);
        GL.Vertex3(min.x, min.y, max.z);
        GL.Vertex3(min.x, max.y, max.z);
        GL.End();

        GL.PopMatrix();
    }
}
