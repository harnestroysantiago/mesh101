using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triangle : MonoBehaviour
{
    private readonly Vector3 _chunkDimension = new Vector3 (4,4,4);
    private readonly Vector3[] _vertices = new Vector3[125];
    private int[] _triangles = new int[3];
    
    private Vector2[] _uv = new Vector2[3];

// Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DefineChunkShape());
    }

    IEnumerator DefineChunkShape()
    {
        for (int i = 0, y = 0; y <= _chunkDimension.x; y++)
        {
            for (int z = 0; z <= _chunkDimension.y; z++)
            {
                for (int x = 0; x <= _chunkDimension.z; x++)
                {
                    _vertices[i] = new Vector3(x, y, z);
                    i++;

                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }
    
    void OnDrawGizmos() 
    {
        if(_vertices.Length == 0)
            return;
        
        Gizmos.color = Color.red;
        foreach (var vertex in _vertices)
        {
            Gizmos.DrawSphere(vertex, 0.05f);
        }
    }
}
