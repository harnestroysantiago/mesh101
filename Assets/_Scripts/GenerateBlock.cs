using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class GenerateBlock : MonoBehaviour
{
    private BlockDto _block;
    private Vector3[] _vertPos;
    private int[] _triangle;

    private Mesh _mesh;
    private MeshFilter _meshFilter;
    
    void Start()
    {
        _mesh = new Mesh();
        _meshFilter = GetComponent<MeshFilter>();
        _meshFilter.mesh = _mesh;
        
        InitializeBlock();
        CreateBlockShape();
    }

    void FixedUpdate()
    {
        UpdateMesh();
    }
    
    void InitializeBlock()
    {
        _block = new BlockDto()
        {
            Type = Enums.BlockType.Grass,
            Side = new []{true, true, true, true, true, true}
        };
    }

    void CreateBlockShape()
    {
        /*
            F ---------- G
            |    B --------- C
            |    |       |   |
            |    |       |   |
            E ---|------ H   |
                 A --------- D
            A is the origin point for the mesh
        */
        
        // A 0 = 0,0,0
        // B 1 = 0,1,0
        // C 2 = 1,1,0
        // D 3 = 1,0,0
        // E 4 = 0,0,1
        // F 5 = 0,1,1
        // G 6 = 1,1,1
        // H 7 = 1,0,1
        
        _vertPos = new Vector3[]
        {
            new (0, 0, 0),
            new (0, 1, 0),
            new (1, 1, 0),
            new (1, 0, 0),
            
            new (0, 0, 1),
            new (0, 1, 1),
            new (1, 1, 1),
            new (1, 0, 1)
        };
        
        // triangle ints are index of your passed vertices,
        // in group of 3, and 6 for a full quad
        _triangle = new int[]
        {
            0,7,4,0,3,7,
            2,1,5,2,5,6,
            0,4,5,0,5,1,
            7,3,2,7,2,6,
            3,0,1,3,1,2,
            4,7,6,4,6,5
        };
        
        //Bottom [0] = AHE,ADH || 074 , 037
        //Top [1] = CBF,CFG || 215 , 256  
        //Left [2] = AEF,AFB || 045 , 051 
        //Right [3] = HDC,HCG || 732 , 726
        //Front [4] = EHG,EGF || 476 , 465
        //Back [5] = DAB,DBC || 301 , 312
    }

    void UpdateMesh()
    {
        _mesh.Clear();

        _mesh.name = "voxelicious";
        _mesh.vertices = _vertPos;
        _mesh.triangles = _triangle;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _mesh.RecalculateTangents();
    }

    private void OnDrawGizmos()
    {
        if(_vertPos == null)
            return;
        
        Gizmos.color = Color.red;
        for (int x = 0; x <= 1; x++)
            for (int y = 0; y <= 1; y++)
                for (int z = 0; z <= 1; z++)
                    Gizmos.DrawSphere(new Vector3(x,y,z) + transform.position, 0.05f);
    }
}
