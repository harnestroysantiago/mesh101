using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour
{
    
    private static Vector3Int _terrainDimension = new Vector3Int(8,8,8);
    private static int _chunkSize = 2;
    private Vector3Int _chunkDimension = new Vector3Int(
        _terrainDimension.x/_chunkSize,
        _terrainDimension.y/_chunkSize, 
        _terrainDimension.z/_chunkSize);
    private int _terrainHeight = 4;
    private BlockDto[,,] _block = new BlockDto[_terrainDimension.x,_terrainDimension.y,_terrainDimension.z];
    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();

    private Mesh _mesh;
    private MeshFilter _meshFilter;
    
    // Start is called before the first frame update

    private void Awake()
    {
        // calculations here
    }

    void Start()
    {

    }
}
