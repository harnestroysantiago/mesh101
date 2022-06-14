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
    
    void Start()
    {
        // functions here.
    }

    private void GenerateChunks()
    {
        GenerateChunk();
    }

    private void GenerateChunk()
    {
        // loop through all chunks here
        // instantiate
        // generate mesh and assign
    }
    
    private void ClearBlockAndMeshData()
    {
        _mesh.Clear();
        _vertices.Clear();
        _triangles.Clear();

        for (int x = 0; x < _chunkDimension.x; x++)
        {
            for (int y = 0; y < _chunkDimension.y; y++)
            {
                for (int z = 0; z < _chunkDimension.z; z++)
                {
                    _block[x, y, z] = new BlockDto()
                    {
                        Type = Enums.BlockType.Air,
                        Side = new[] { false, false, false, false, false, false }
                    };
                }
            }
        }
    }

    IEnumerator GenerateMeshes(Mesh mesh)
    {
        int blockCount=0;
        for (int x = 0; x < _chunkDimension.x; x++)
        {
            for (int y = 0; y < _chunkDimension.y; y++)
            {
                for (int z = 0; z < _chunkDimension.z; z++)
                {
                    GenerateMesh(mesh,x,y,z, blockCount);
                    blockCount++;
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }
    }

    // this works for one chunk, we need to generate the whole chunk data and render them.
    private void GenerateMesh(Mesh mesh, int x, int y, int z, int blockCount)
    {
        Vector3Int blockLoc = new Vector3Int(x, y, z);
        BlockDto block = _block[blockLoc.x, blockLoc.y, blockLoc.z];
        
        Vector3[] vertices = new Vector3[]
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
        
        // this updates the offset of the chunk vertices, so we always go on the
        // blockLoc + the vertex to give it a quad on each face
        for (int i = 0; i < 8; i++)
        {
            vertices[i] += blockLoc;
        }
        
        //Bottom [0] = AHE,ADH || 074 , 037
        //Top [1] = CBF,CFG || 215 , 256  
        //Left [2] = AEF,AFB || 045 , 051 
        //Right [3] = HDC,HCG || 732 , 726
        //Front [4] = EHG,EGF || 476 , 465
        //Back [5] = DAB,DBC || 301 , 312
        List<int> triangles = new List<int>();
        
        // the triangles indexes needs to be adjusted as well, based on how many we have....
        for (int i = 0; i < 6; i++)
        {
            if (block.Side[i])
            {
                switch (i)
                {
                    case (int)Enums.BlockSide.Bottom:
                        triangles.AddRange(new int[]{0,7,4,0,3,7});
                        break;
                    case (int)Enums.BlockSide.Top:
                        triangles.AddRange(new int[]{2,1,5,2,5,6});
                        break;
                    case (int)Enums.BlockSide.Left:
                        triangles.AddRange(new int[]{0,4,5,0,5,1});
                        break;
                    case (int)Enums.BlockSide.Right:
                        triangles.AddRange(new int[]{7,3,2,7,2,6});
                        break;
                    case (int)Enums.BlockSide.Front:
                        triangles.AddRange(new int[]{4,7,6,4,6,5});
                        break;
                    case (int)Enums.BlockSide.Back:
                        triangles.AddRange(new int[]{3,0,1,3,1,2});
                        break;
                }
            }
        }
        
        // this updates the offset of the chunk triangles, so we always go on the
        // lenght + the vertex index to give it a quad on each face
        for (int i = 0; i < triangles.Count; i++)
        {
            triangles[i] += blockCount * 8;
        }

        // add to existing _vertex and _triangles
        _vertices.AddRange(vertices);
        _triangles.AddRange(triangles);
        
        var vertArray = _vertices.ToArray();
        var triArray = _triangles.ToArray();
        
        mesh.Clear();
        
        mesh.name = "mesh...illicious";
        mesh.vertices = vertArray;
        mesh.triangles = triArray;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
    }
    
    private void GenerateBlockData()
    {
        for (int x = 0; x < _chunkDimension.x; x++)
        {
            for (int y = 0; y < _chunkDimension.y; y++)
            {
                for (int z = 0; z < _chunkDimension.z; z++)
                {
                    PlaceBlockData(y, x, z);
                }
            }
        }

        for (int x = 0; x < _chunkDimension.x; x++)
        {
            for (int y = 0; y < _chunkDimension.y; y++)
            {
                for (int z = 0; z < _chunkDimension.z; z++)
                {
                    CalculateSides(_block[x, y, z],x,y,z);
                }
            }
        }
    }

    private void PlaceBlockData(int y, int x, int z)
    {
        if (UnityEngine.Random.Range(0, 2) == 1)
        {
            _block[x, y, z] = new BlockDto()
            {
                Type = Enums.BlockType.Dirt,
                Side = new[] { false, false, false, false, false, false }
            };
        }
        else
        {
            _block[x, y, z] = new BlockDto()
            {
                Type = Enums.BlockType.Air,
                Side = new[] { false, false, false, false, false, false }
            };
        }
    }

    private void CalculateSides(BlockDto block, int x, int y, int z)
    {
        if (block.Type == Enums.BlockType.Air)
            return;
        
        block.Side[(int)Enums.BlockSide.Bottom] =
            TryGetValidBlock(new Vector3Int(x, y, z) + Vector3Int.down).Type == Enums.BlockType.Air;
        block.Side[(int)Enums.BlockSide.Top] =
            TryGetValidBlock(new Vector3Int(x, y, z) + Vector3Int.up).Type == Enums.BlockType.Air;
        block.Side[(int)Enums.BlockSide.Left] =
            TryGetValidBlock(new Vector3Int(x, y, z) + Vector3Int.left).Type == Enums.BlockType.Air;
        block.Side[(int)Enums.BlockSide.Right] =
            TryGetValidBlock(new Vector3Int(x, y, z) + Vector3Int.right).Type == Enums.BlockType.Air;
        block.Side[(int)Enums.BlockSide.Front] =
            TryGetValidBlock(new Vector3Int(x, y, z) + Vector3Int.forward).Type == Enums.BlockType.Air;
        block.Side[(int)Enums.BlockSide.Back] =
            TryGetValidBlock(new Vector3Int(x, y, z) + Vector3Int.back).Type == Enums.BlockType.Air;
    }
    
    private BlockDto TryGetValidBlock(Vector3Int blockLoc)
    {
        try
        {
            return _block[blockLoc.x, blockLoc.y, blockLoc.z];
        }
        catch (Exception)
        {
            // ignored
        }

        return new BlockDto()
        {
            Type = Enums.BlockType.Air,
            Side = new[] { false, false, false, false, false, false }
        };
    }

    private void OnDrawGizmos()
    {
        if(_vertices.Count == 0)
            return;
        
        Gizmos.color = Color.magenta;
        for (int x = 0; x <= _chunkDimension.x; x++)
        {
            for (int y = 0; y <= _chunkDimension.y; y++)
            {
                for (int z = 0; z <= _chunkDimension.z; z++)
                {
                    Gizmos.DrawSphere(new Vector3(x,y,z) + transform.position, 0.05f);
                }
            }
        }
    }
}
