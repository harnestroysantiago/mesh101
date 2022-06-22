using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour
{
    
    private static Vector3Int _terrainDimension = new Vector3Int(9,9,9);
    private static int _chunkSize = 3;
    private int _terrainHeight = (int)(_terrainDimension.y * 0.2f);
    private Vector3Int _chunkDimension = new Vector3Int(
        _terrainDimension.x/_chunkSize,
        _terrainDimension.y/_chunkSize, 
        _terrainDimension.z/_chunkSize);
    private BlockDto[,,] _blocks = new BlockDto[_terrainDimension.x,_terrainDimension.y,_terrainDimension.z];
    private List<Vector3> _vertices = new List<Vector3>();
    private List<Vector2> _uvs = new List<Vector2>();
    private List<int> _triangles = new List<int>();

    private Mesh _mesh;
    private MeshFilter _meshFilter;
    
    [SerializeField] 
    private bool _hasTerrain;
    [SerializeField]
    private GameObject _chunkPrefab;
    [SerializeField]
    private Material _chunkMaterial;
    
    void Start()
    {
        Debug.Log("my terrain dimension = " + _terrainDimension);
        Debug.Log("my chunk dimension = " + _chunkDimension);
        Debug.Log("my chunk size = " + _chunkSize);
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
        _uvs.Clear();
    }

    private void GenerateChunks()
    {
        //where are passing the origin of each chunk
        for (int x = 0; x < _chunkDimension.x; x++)
            for (int y = 0; y < _chunkDimension.y; y++)
                for (int z = 0; z < _chunkDimension.z; z++)
                {
                    GenerateChunk(x * _chunkSize,y * _chunkSize,z * _chunkSize);
                    
                    // Debug.Log("passed chunks loc = " 
                    //           + x * _chunkSize + "," 
                    //           + y * _chunkSize + "," 
                    //           + z * _chunkSize);
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
        for (int x = 0; x < _chunkSize; x++)
            for (int y = 0; y < _chunkSize; y++)
                for (int z = 0; z < _chunkSize; z++)
                {
                    GenerateMeshOfEachBlockInThisChunk(mesh,x+chunkRootX,y+chunkRootY,z+chunkRootZ,blockCount);
                    blockCount++;
                }
        
        return mesh;
    }
    
    private void GenerateMeshOfEachBlockInThisChunk(Mesh mesh, int xLocOffset, int yLocOffset, int zLocOffset, int blockCountInThisChunk)
    {
        // initialize our offsets for this block
        Vector3 blockLocOffset = new Vector3(xLocOffset, yLocOffset, zLocOffset);
        BlockDto block = _blocks[xLocOffset, yLocOffset, zLocOffset];
        
        // Debug.Log(
        //     "my type is = " + block.Type +
        //         " || my loc is = " + blockLocOffset + 
        //         " || my parent chunkLoc = " + block.ParentChunkLoc);
        
        
        Vector3[] vP = 
        {
            new Vector3(0, 0, 0), 
            new Vector3(0, 1, 0), 
            new Vector3(1, 1, 0), 
            new Vector3(1, 0, 0), 
            new Vector3(0, 0, 1), 
            new Vector3(0, 1, 1), 
            new Vector3(1, 1, 1), 
            new Vector3(1, 0, 1),
        };

        // Vector2[] uvs =
        // {
        //     new(0,0),
        //     new(0,1),
        //     new(1,1),
        //     new(1,0),
        // };
        
        // this updates the offset of the chunk vertices, so we always go on the
        // blockLoc + the vertex to give it a quad on each face
        for (int i = 0; i < 8; i++)
        {
            vP[i] += blockLocOffset;
        }
        
        // additional uv mapping, please dont delete
        //Bottom [0] = AHE,ADH || 12 14 13 , 12 15 14
        //Top [1] = CBF,CFG || 11 8 9 , 11 9 10  
        
        //Bottom [0] = AHE,ADH || 074 , 037
        //Top [1] = CBF,CFG || 215 , 256  
        //Left [2] = AEF,AFB || 045 , 051 
        //Right [3] = HDC,HCG || 732 , 726
        //Front [4] = EHG,EGF || 476 , 465
        //Back [5] = DAB,DBC || 301 , 312
        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        
        // the triangles indexes needs to be adjusted as well,
        // based on how many quad face we have....
        int vertMultiplier = 0;
        for (int i = 0; i < 6; i++)
        {
            if (block.Side[i])
            {
                switch (i)
                {
                    case (int)Enums.BlockSide.Bottom:
                        vertices.AddRange(new[]{vP[4],vP[0],vP[3],vP[7]});
                        //triangles.AddRange(new[] { 4, 0, 3, 4, 3, 7 });
                        break;
                    case (int)Enums.BlockSide.Top:
                        vertices.AddRange(new[]{vP[1],vP[5],vP[6],vP[2]});
                        //triangles.AddRange(new[] { 1, 5, 6, 1, 6, 2 });
                        break;
                    case (int)Enums.BlockSide.Right:
                        vertices.AddRange(new[]{vP[3],vP[2],vP[6],vP[7]});
                        //triangles.AddRange(new int[]{3,2,6,3,6,7});
                        break;
                    case (int)Enums.BlockSide.Left:
                        vertices.AddRange(new[]{vP[4],vP[5],vP[1],vP[0]});
                        //triangles.AddRange(new int[]{4,5,1,4,1,0});
                        break;
                    case (int)Enums.BlockSide.Front:
                        vertices.AddRange(new[]{vP[5],vP[4],vP[7],vP[6]});
                        //triangles.AddRange(new int[]{5,4,7,5,7,6});
                        break;
                    case (int)Enums.BlockSide.Back:
                        vertices.AddRange(new[]{vP[0],vP[1],vP[2],vP[3]});
                        //triangles.AddRange(new int[]{0,1,2,0,2,3});
                        break;
                }
                vertMultiplier++;
                uvs.AddRange(GetSideUVs(new Vector2Int(0,15)));
                triangles.Add(vertices.Count - 4);
                triangles.Add(vertices.Count - 3);
                triangles.Add(vertices.Count - 2);
                triangles.Add(vertices.Count - 4);
                triangles.Add(vertices.Count - 2);
                triangles.Add(vertices.Count - 1);
            }
        }
        
        // this updates the offset of the chunk triangles, so we always go on the
        // lenght + the vertex index to give it a quad on each face
        for (int i = 0; i < triangles.Count; i++)
        {
            // triangles[i] += blockCountInThisChunk 8 8;
            triangles[i] += _vertices.Count;
        }

        // add to existing _vertex and _triangles
        _vertices.AddRange(vertices);
        _triangles.AddRange(triangles);
        _uvs.AddRange(uvs);
        
        // convert to array
        var vertArray = _vertices.ToArray();
        var triArray = _triangles.ToArray();
        var uvArray = _uvs.ToArray();
        
        // pass the vertices and triangles to our mesh
        mesh.Clear();
        // mesh.name = "meshy...";
        mesh.vertices = vertArray;
        mesh.uv = uvArray;
        mesh.triangles = triArray;
        mesh.RecalculateNormals();
    }
    
    static Vector2[] GetSideUVs(Vector2Int tileCoords)
    {

        Vector2[] uvs = new Vector2[4];

        //TODO: implement SO
        Vector2Int tilePos = new Vector2Int(2, 15);
        float textureOffset = 0.0001f;
        float bias = 1 / 16;
        //-------------------------------
        uvs[0] = new Vector2
        (
            bias * tilePos.x + bias - textureOffset,
            bias * tilePos.y + textureOffset
        );
        uvs[1] = new Vector2
        (
            bias * tilePos.x + bias - textureOffset,
            bias * tilePos.y + bias - textureOffset
        );
        uvs[2] = new Vector2
        (
            bias * tilePos.x + textureOffset,
            bias * tilePos.y + bias - textureOffset
        );
        uvs[3] = new Vector2
        (
            bias * tilePos.x + textureOffset,
            bias * tilePos.y + textureOffset
        );
        
        return uvs;
    }

    
    private void GenerateBlockData()
    {
        for (int x = 0; x < _terrainDimension.x; x++)
            for (int y = 0; y < _terrainDimension.y; y++)
                for (int z = 0; z < _terrainDimension.z; z++)
                    PlaceBlockData(x, y, z, _hasTerrain);
        
        for (int x = 0; x < _terrainDimension.x; x++)
            for (int y = 0; y < _terrainDimension.y; y++)
                for (int z = 0; z < _terrainDimension.z; z++)
                    CalculateSides(_blocks[x, y, z],x,y,z);
        
        for (int x = 0; x < _chunkDimension.x; x++)
            for (int y = 0; y < _chunkDimension.y; y++)
                for (int z = 0; z < _chunkDimension.z; z++)
                    SetParentChunkLoc(x*_chunkSize,y*_chunkSize,z*_chunkSize);
                
    }

    private void PlaceBlockData(int x, int y, int z, bool hasTerrain)
    {
        if(hasTerrain)
        {
            var terrainHeight = _terrainHeight;

            if (y > terrainHeight)
            {
                GenerateBlock(x, y, z, Enums.BlockType.Air);
            }
            else if (y > terrainHeight - (terrainHeight * 0.5f))
            {
                GenerateBlock(x, y, z,
                    UnityEngine.Random.Range(0, 8) == 1 ? Enums.BlockType.Dirt : Enums.BlockType.Air);
            }
            else
            {
                GenerateBlock(x, y, z, Enums.BlockType.Dirt);
            }
        }
        else
        {
            GenerateBlock(x, y, z,
                UnityEngine.Random.Range(0, 4) == 1 ? Enums.BlockType.Dirt : Enums.BlockType.Air);
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

    private void SetParentChunkLoc(int chunkX, int chunkY, int chunkZ)
    {
        for (int x = 0; x < _chunkSize; x++)
            for (int y = 0; y < _chunkSize; y++)
                for (int z = 0; z < _chunkSize; z++)
                    _blocks[x+chunkX,y+chunkY,z+chunkZ].ParentChunkLoc = new Vector3Int(chunkX,chunkY,chunkZ);
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
