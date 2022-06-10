using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triangle : MonoBehaviour
{
    private Vector3[] _vertices = new Vector3[3];
    private int[] _triangles = new int[3];
    private Vector2[] _uv = new Vector2[3];

    // Start is called before the first frame update
    void Start(){
        DefineMesh();
    }

    void DefineMesh(){

    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        for (int i = 0; i < 3; i++){
            Gizmos.DrawSphere(Vector3.zero + new Vector3(0,i,0), 0.05f);
        }
    }
}
