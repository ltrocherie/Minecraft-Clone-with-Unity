using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public const int chunkWidth = 16;
    public const int chunkHeight = 64;

    FastNoise noise = new FastNoise();

    public BlockType[,,] blocks = new BlockType[chunkWidth + 2, chunkHeight, chunkWidth + 2];


    // Start is called before the first frame update
    void Start()
    {


        for (int x = 0; x < chunkWidth; x++)
        {
            for (int z = 0; z < chunkWidth; z++)
            {
                for(int y = 0; y < chunkHeight; y++)
                {
                    blocks[x, y, z] = GetBlockType((int)transform.position.x + x, y, (int)transform.position.z + z);

                    /*if (blocks[x, y, z] == BlockType.Ground)
                    {
                        GameObject cubeInst = Instantiate(cube, new Vector3(x, y, z), Quaternion.identity);
                        cubeInst.transform.SetParent(gameObject.transform);
                    }*/

                }

                buildMesh();
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    BlockType GetBlockType(int x, int y, int z)
    {
        BlockType blockType = BlockType.Air;

        int new_y = (int)Math.Ceiling(noise.GetSimplex(x * .8f, z * .8f) * 20);

        if (chunkHeight/2 + new_y == y)
            blockType = BlockType.Ground;
        

        return blockType;
    }

    void buildMesh()
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int x = 1; x < chunkWidth; x++)
        {
            for (int z = 1; z < chunkWidth; z++)
            {
                for (int y = 0; y < chunkHeight; y++)
                {
                    if(blocks[x, y, z] != BlockType.Air)
                    {
                        Vector3 blockPos = new Vector3(x, y, z);
                        int numberOfFaces = 0;

                        // no block above, need to create a mesh
                        if(y < chunkHeight - 1 && blocks[x, y + 1, z] == BlockType.Air)
                        {
                            vertices.Add(blockPos + new Vector3(0, 1, 0));
                            vertices.Add(blockPos + new Vector3(0, 1, 1));
                            vertices.Add(blockPos + new Vector3(1, 1, 1));
                            vertices.Add(blockPos + new Vector3(1, 1, 0));
                            numberOfFaces++;
                        }

                        // no land underneath
                        if(y > 0 && blocks[x, y - 1, z] == BlockType.Air)
                        {
                            vertices.Add(blockPos + new Vector3(0, 0, 0));
                            vertices.Add(blockPos + new Vector3(1, 0, 0));
                            vertices.Add(blockPos + new Vector3(1, 0, 1));
                            vertices.Add(blockPos + new Vector3(1, 0, 1));
                            numberOfFaces++;
                        }


                        // no land in front
                        if (blocks[x, y, z - 1] == BlockType.Air)
                        {
                            vertices.Add(blockPos + new Vector3(0, 0, 0));
                            vertices.Add(blockPos + new Vector3(0, 1, 0));
                            vertices.Add(blockPos + new Vector3(1, 1, 1));
                            vertices.Add(blockPos + new Vector3(1, 0, 0));
                            numberOfFaces++;
                        }

                        // no land on the right
                        if (blocks[x + 1, y, z] == BlockType.Air)
                        {
                            vertices.Add(blockPos + new Vector3(1, 0, 0));
                            vertices.Add(blockPos + new Vector3(1, 1, 0));
                            vertices.Add(blockPos + new Vector3(1, 1, 1));
                            vertices.Add(blockPos + new Vector3(1, 0, 1));
                            numberOfFaces++;
                        }

                        // no land in back
                        if (blocks[x, y, z + 1] == BlockType.Air)
                        {
                            vertices.Add(blockPos + new Vector3(1, 0, 1));
                            vertices.Add(blockPos + new Vector3(1, 1, 1));
                            vertices.Add(blockPos + new Vector3(0, 1, 1));
                            vertices.Add(blockPos + new Vector3(0, 0, 1));
                            numberOfFaces++;
                        }

                        // no land on the left
                        if (blocks[x - 1, y, z] == BlockType.Air)
                        {
                            vertices.Add(blockPos + new Vector3(0, 0, 1));
                            vertices.Add(blockPos + new Vector3(0, 1, 1));
                            vertices.Add(blockPos + new Vector3(0, 1, 0));
                            vertices.Add(blockPos + new Vector3(0, 0, 0));
                            numberOfFaces++;
                        }


                        int trianglesLength = vertices.Count - 4 * numberOfFaces;
                        for(int i = 0; i < numberOfFaces; i++)
                        {
                            triangles.AddRange(new int[] { trianglesLength + i * 4, trianglesLength + i * 4 + 1, trianglesLength + i * 4 + 2, trianglesLength + i * 4, trianglesLength + i * 4 + 2, trianglesLength + i * 4 + 3 });
                        }
                    }
                }
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }


}
