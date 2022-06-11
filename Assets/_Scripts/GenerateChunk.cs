using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateChunk : MonoBehaviour
{
    
    private static Vector3Int _chunkDimension = new Vector3Int(4,4,4);
    private int _terrainHeight = 2;
    private BlockDto[,,] _block = new BlockDto[_chunkDimension.x,_chunkDimension.y,_chunkDimension.z];
    private Vector3[] _vertices;
    private List<int> _triangles;

    private Mesh _mesh;
    private MeshFilter _meshFilter;

    private void Start()
    {
        _mesh = new Mesh();
        _meshFilter = GetComponent<MeshFilter>();
        _meshFilter.mesh = _mesh;
        
        GenerateBlockData();
        GenerateMesh(_mesh);
    }

    void GenerateMesh(Mesh mesh)
    {
        BlockDto block = _block[0, 0, 0];
        
        _vertices = new Vector3[]
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
        
        //Bottom [0] = AHE,ADH || 074 , 037
        //Top [1] = CBF,CFG || 215 , 256  
        //Left [2] = AEF,AFB || 045 , 051 
        //Right [3] = HDC,HCG || 732 , 726
        //Front [4] = DAB,DBC || 301 , 312
        //Back [5] = EHG,EGF || 476 , 465

        for (int i = 0; i < 6; i++)
        {
            if (block.Side[i] == true)
            {
                switch (i)
                {
                    case (int)Enums.BlockSide.Bottom:
                        _triangles.AddRange(new int[]{0,7,4,0,3,7});
                        break;
                    case (int)Enums.BlockSide.Top:
                        _triangles.AddRange(new int[]{2,1,5,2,5,6});
                        break;
                    case (int)Enums.BlockSide.Left:
                        _triangles.AddRange(new int[]{0,4,5,0,5,1});
                        break;
                    case (int)Enums.BlockSide.Right:
                        _triangles.AddRange(new int[]{7,3,2,7,2,6});
                        break;
                    case (int)Enums.BlockSide.Front:
                        _triangles.AddRange(new int[]{3,0,1,3,1,2});
                        break;
                    case (int)Enums.BlockSide.Back:
                        _triangles.AddRange(new int[]{4,7,6,4,6,5});
                        break;
                }
            }
        }
        
        mesh.Clear();
        
        mesh.name = "meshilicious";
        mesh.vertices = _vertices;
        mesh.triangles = _triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
    }

    private void Update()
    {
        
    }

    void GenerateBlockData()
    {
        for (int x = 0; x < _chunkDimension.x; x++)
        {
            for (int y = 0; y < _chunkDimension.y; y++)
            {
                for (int z = 0; z < _chunkDimension.z; z++)
                {
                    if (y > _terrainHeight)
                    {
                        _block[x, y, z] = new BlockDto()
                        {
                            Type = Enums.BlockType.Air,
                            Side = new[] { false, false, false, false, false, false }
                        };
                    }
                    else
                    {
                        _block[x, y, z] = new BlockDto()
                        {
                            Type = Enums.BlockType.Grass,
                            Side = new[] { false, false, false, false, false, false }
                        };
                    }
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

    void CalculateSides(BlockDto block, int x, int y, int z)
    {
        block.Side[(int)Enums.BlockSide.Bottom] =
            TryGetValidBlock(new Vector3Int(x, y, z) + Vector3Int.down).Type == Enums.BlockType.Air;
        block.Side[(int)Enums.BlockSide.Top] =
            TryGetValidBlock(new Vector3Int(x, y, z) + Vector3Int.down).Type == Enums.BlockType.Air;
        block.Side[(int)Enums.BlockSide.Left] =
            TryGetValidBlock(new Vector3Int(x, y, z) + Vector3Int.down).Type == Enums.BlockType.Air;
        block.Side[(int)Enums.BlockSide.Right] =
            TryGetValidBlock(new Vector3Int(x, y, z) + Vector3Int.down).Type == Enums.BlockType.Air;
        block.Side[(int)Enums.BlockSide.Front] =
            TryGetValidBlock(new Vector3Int(x, y, z) + Vector3Int.down).Type == Enums.BlockType.Air;
        block.Side[(int)Enums.BlockSide.Back] =
            TryGetValidBlock(new Vector3Int(x, y, z) + Vector3Int.down).Type == Enums.BlockType.Air;
    }
    
    BlockDto TryGetValidBlock(Vector3Int blockLoc)
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
}
