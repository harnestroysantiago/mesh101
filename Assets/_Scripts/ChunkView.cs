using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkView : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private MeshFilter _meshFilter;
    [SerializeField]
    private MeshRenderer _meshRenderer;
    [SerializeField]
    private MeshCollider _meshCollider;

    public void InitializeChunk(Mesh mesh, Material material)
    {
        _meshFilter.mesh = mesh;
        _meshCollider.sharedMesh = mesh;
        _meshRenderer.material = material;
    }
}
