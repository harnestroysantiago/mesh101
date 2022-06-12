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
                Type = Enums.BlockType.Dirt,
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
        if(_vertices.Count == null)
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
