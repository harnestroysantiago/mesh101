using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour
{
    
    private static Vector3Int _terrainDimension = new Vector3Int(8,8,8);
    private static int _chunkSize = 4;
    private Vector3Int _chunkDimension = new Vector3Int(
        _terrainDimension.x/_chunkSize,
        _terrainDimension.y/_chunkSize, 
        _terrainDimension.z/_chunkSize);
    private BlockDto[,,] _blocks = new BlockDto[_terrainDimension.x,_terrainDimension.y,_terrainDimension.z];
    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();

    private Mesh _mesh;
    private MeshFilter _meshFilter;

    [SerializeField]
    private GameObject _chunkPrefab;
    [SerializeField]
    private Material _chunkMaterial;
    
    void Start()
    {
        Debug.Log("my chunk dimension = " + _chunkDimension);
        GenerateBlockData();
        GenerateChunks();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            GenerateBlockData();
            GenerateChunks();
        }
    }

    private void ClearMeshData()
    {
        _vertices.Clear();
        _triangles.Clear();
    }

    private void GenerateChunks()
    {
        //where are passing the origin of each chunk
        for (int x = 0; x < _chunkDimension.x; x++)
            for (int y = 0; y < _chunkDimension.y; y++)
                for (int z = 0; z < _chunkDimension.z; z++)
                {
                    Debug.Log("passed chunks loc = " 
                              + x * _chunkSize + "," 
                              + y * _chunkSize + "," 
                              + z * _chunkSize);
                    GenerateChunk(x * _chunkSize,y * _chunkSize,z * _chunkSize);
                }
    }

    private void GenerateChunk(int chunkRootX, int chunkRootY, int chunkRootZ )
    {
        _mesh = new Mesh();
        _mesh = GenerateMeshesInThisChunk(chunkRootX,chunkRootY,chunkRootZ, _mesh);
        
        Transform chunkHolder;
        ChunkView chunkView = Instantiate(_chunkPrefab, (chunkHolder = transform).position, Quaternion.identity, chunkHolder).GetComponent<ChunkView>();
        
        string gameObjectName = 
            String.Concat(chunkRootX + "," + chunkRootY + "," + chunkRootZ);
        
        _mesh.name = gameObjectName;
        chunkView.gameObject.name = gameObjectName;
        
        chunkView.InitializeChunk(_mesh, _chunkMaterial);

        ClearMeshData();
    }

    Mesh GenerateMeshesInThisChunk(int chunkRootX, int chunkRootY, int chunkRootZ, Mesh mesh)
    {
        int blockCount=0;
        for (int x = 0; x < _chunkDimension.x; x++)
            for (int y = 0; y < _chunkDimension.y; y++)
                for (int z = 0; z < _chunkDimension.z; z++)
                {
                    GenerateMeshOfEachBlockInThisChunk(mesh,x+chunkRootX,y+chunkRootY,z+chunkRootZ,blockCount);
                    blockCount++;
                }
        
        return mesh;
    }
    
    private void GenerateMeshOfEachBlockInThisChunk(Mesh mesh, int xLocOffset, int yLocOffset, int zLocOffset, int blockCountInThisChunk)
    {
        // initialize our offsets for this block
        Vector3Int blockLocOffset = new Vector3Int(xLocOffset, yLocOffset, zLocOffset);
        BlockDto block = _blocks[blockLocOffset.x, blockLocOffset.y, blockLocOffset.z];
        
        Vector3[] vertices = 
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
            vertices[i] += blockLocOffset;
        }
        
        //Bottom [0] = AHE,ADH || 074 , 037
        //Top [1] = CBF,CFG || 215 , 256  
        //Left [2] = AEF,AFB || 045 , 051 
        //Right [3] = HDC,HCG || 732 , 726
        //Front [4] = EHG,EGF || 476 , 465
        //Back [5] = DAB,DBC || 301 , 312
        List<int> triangles = new List<int>();
        
        // the triangles indexes needs to be adjusted as well,
        // based on how many quad face we have....
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
            triangles[i] += blockCountInThisChunk * 8;
        }

        // add to existing _vertex and _triangles
        _vertices.AddRange(vertices);
        _triangles.AddRange(triangles);
        
        // convert to array
        var vertArray = _vertices.ToArray();
        var triArray = _triangles.ToArray();
        
        // pass the vertices and triangles to our mesh
        mesh.Clear();
        mesh.name = "meshy...";
        mesh.vertices = vertArray;
        mesh.triangles = triArray;
        mesh.RecalculateNormals();
    }
    
    private void GenerateBlockData()
    {
        for (int x = 0; x < _terrainDimension.x; x++)
            for (int y = 0; y < _terrainDimension.y; y++)
                for (int z = 0; z < _terrainDimension.z; z++)
                    PlaceBlockData(x, y, z);
        
        for (int x = 0; x < _terrainDimension.x; x++)
            for (int y = 0; y < _terrainDimension.y; y++)
                for (int z = 0; z < _terrainDimension.z; z++)
                    CalculateSides(_blocks[x, y, z],x,y,z);
    }

    private void PlaceBlockData(int x, int y, int z)
    {
        var terrainHeight = 2;
        
        if (y > terrainHeight)
        {
            GenerateBlock(x, y, z, Enums.BlockType.Air);
        }
        else if(y > terrainHeight - 2)
        {
            GenerateBlock(x, y, z, UnityEngine.Random.Range(0, 4) == 1 ? 
                Enums.BlockType.Dirt : Enums.BlockType.Air);
        }
        else
        {
            GenerateBlock(x, y, z, Enums.BlockType.Dirt);
        }
    }

    private void GenerateBlock(int x, int y, int z, Enums.BlockType type)
    {
        if (_blocks != null)
            _blocks[x, y, z] = new BlockDto()
            {
                Type = type,
                Side = new[] { false, false, false, false, false, false }
            };
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
            return _blocks[blockLoc.x, blockLoc.y, blockLoc.z];
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
        // if(_vertices == null)
        //     return;
        //
        // Gizmos.color = Color.magenta;
        // for (int x = 0; x <= _terrainDimension.x; x++)
        // {
        //     for (int y = 0; y <= _terrainDimension.y; y++)
        //     {
        //         for (int z = 0; z <= _terrainDimension.z; z++)
        //         {
        //             Gizmos.DrawSphere(new Vector3(x,y,z) + transform.position, 0.05f);
        //         }
        //     }
        // }
    }
}
