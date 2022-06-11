using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triangle : MonoBehaviour
{
    private Vector3[] _vertices;
    private int[] _triangles = new int[3];
    private Vector2[] _uv = new Vector2[3];

    private Vector3 _chunkDimension = new Vector3 (4,4,4);

// Start is called before the first frame update
void Start(){
        DefineChunkShape();
    }

    void DefineChunkShape(){
        var vertCount = 125;
        for (int i = 0; i < vertCount; i++)
        {
            for (int x = 0; x < _chunkDimension.x; x++)
            {
                for (int y = 0; y < _chunkDimension.y; y++)
                {
                    for (int z = 0; z < _chunkDimension.z; z++)
                    {
                        _vertices[i] = new Vector3( x, y, z );
                    }
                }
            }
        }
    }

    void DefineMesh(){

    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        for (int i = 0; i < _vertices.Length; i++){
            Gizmos.DrawSphere(_vertices[i], 0.05f);
        }
    }
}
