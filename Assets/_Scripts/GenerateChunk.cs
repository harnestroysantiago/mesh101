using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateChunk : MonoBehaviour
{
    
    private static Vector3Int _chunkDimension = new Vector3Int(4,4,4);
    private int _terrainHeight = 2;
    private BlockDto[,,] _block = new BlockDto[_chunkDimension.x,_chunkDimension.y,_chunkDimension.z];

    private void Start()
    {
        GenerateBlockData();
    }

    void GenerateMesh()
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
