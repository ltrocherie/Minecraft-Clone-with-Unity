using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public struct ChunkPos
{
    public int x, z;

    public ChunkPos(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
}


public class TerrainGeneration : MonoBehaviour
{
    public GameObject chunk;
    public GameObject player;

    public int chunkWidth;
    int chunkDistance = 5;

    //private int xInferiorLimit = -64, xSuperiorLimit = 64, zInferiorLimit = -64, zSuperiorLimit = 64;

    FastNoise noise = new FastNoise();

    public static Dictionary<ChunkPos, Chunk> chunks = new Dictionary<ChunkPos, Chunk>();

    List<Chunk> pooledChunks= new List<Chunk>();

    List<ChunkPos> toGenerate = new List<ChunkPos>();

    ChunkPos currentChunk = new ChunkPos(-1, -1);

    // Start is called before the first frame update
    void Start()
    {

        /*        for(int x = xInferiorLimit; x < xSuperiorLimit; x += chunkWidth - 1)
                {
                    for(int z = zInferiorLimit; z < zSuperiorLimit; z += chunkWidth - 1)
                    {
                        BuildChunk(x, z);
                    }
                }*/

        LoadChunks(true);

    }

    // Update is called once per frame
    private void Update()
    {
        /*        if(player.transform.position.x > ((xInferiorLimit + xSuperiorLimit)/2 + chunkWidth))
                {
                    for (int z = zInferiorLimit; z < zSuperiorLimit; z += chunkWidth - 1)
                    {
                        GameObject chunkInst = Instantiate(chunk, new Vector3(xSuperiorLimit - 1, 0, z), Quaternion.identity);
                        chunkInst.transform.SetParent(gameObject.transform);
                    }
                    xSuperiorLimit += chunkWidth;
                    xInferiorLimit += chunkWidth;
                }*/

        LoadChunks();
    }

    void BuildChunk(int new_x, int new_z)
    {

        Chunk newChunk;

        if(pooledChunks.Count > 0)
        {
            newChunk = pooledChunks[0];
            pooledChunks.RemoveAt(0);
            newChunk.gameObject.SetActive(true);
            newChunk.transform.position = new Vector3(new_x, 0, new_z);
        }
        else
        {
            GameObject chunkInst = Instantiate(chunk, new Vector3(new_x, 0, new_z), Quaternion.identity);
            chunkInst.transform.SetParent(gameObject.transform);
            newChunk = chunkInst.GetComponent<Chunk>();
        }

        for(int x  = 0; x < Chunk.chunkWidth; x++)
        {
            for(int z = 0; z < Chunk.chunkWidth; z++)
            {
                for(int y = 0; y < Chunk.chunkHeight; y++)
                {
                    newChunk.blocks[x, y, z] = newChunk.GetBlockType((int)newChunk.transform.position.x + x, y, (int)newChunk.transform.position.z + z);
                }
            }
        }

        newChunk.BuildMesh();

        chunks.Add(new ChunkPos(new_x, new_z), newChunk);

        return;
    }

    public void LoadChunks(bool instant = false)
    {
        int playerX = Mathf.FloorToInt(player.transform.position.x / chunkWidth) * chunkWidth;
        int playerZ = Mathf.FloorToInt(player.transform.position.z / chunkWidth) * chunkWidth;

        if (playerX != currentChunk.x || playerZ != currentChunk.z)
        {
            currentChunk.x = playerX;
            currentChunk.z = playerZ;

            for(int i = playerX - chunkWidth * chunkDistance; i <= currentChunk.x + chunkWidth * chunkDistance; i += chunkWidth)
            {
                for (int j = playerZ - chunkWidth * chunkDistance; j <= currentChunk.z + chunkWidth * chunkDistance; j += chunkWidth)
                {
                    ChunkPos chunkPos = new ChunkPos(i, j);

                    if(!chunks.ContainsKey(chunkPos) && !toGenerate.Contains(chunkPos))
                    {
                        if (instant)
                        {
                            BuildChunk(i, j);
                        }
                        else
                        {
                            toGenerate.Add(chunkPos);
                        }
                    }   
                }
            }

            List<ChunkPos> toDestroy = new List<ChunkPos>();

            foreach(KeyValuePair<ChunkPos, Chunk> c in chunks)
            {
                ChunkPos chunkPos = c.Key;

                if(Mathf.Abs(playerX - chunkPos.x) > chunkWidth * (chunkDistance + 3) || Mathf.Abs(playerZ - chunkPos.z) > chunkWidth * (chunkDistance + 3))
                {
                    toDestroy.Add(c.Key);
                }
            }

            foreach(ChunkPos chunkPos in toGenerate)
            {
                if (Mathf.Abs(playerX - chunkPos.x) > chunkWidth * (chunkDistance + 3) || Mathf.Abs(playerZ - chunkPos.z) > chunkWidth * (chunkDistance + 3))
                {
                    toGenerate.Remove(chunkPos);
                }
            }

            foreach(ChunkPos chunkPos in toDestroy)
            {
                chunks[chunkPos].gameObject.SetActive(false);
                pooledChunks.Add(chunks[chunkPos]);
                chunks.Remove(chunkPos);
            }

            StartCoroutine(DelayBuildChunk());
        }

        return;
    }

    IEnumerator DelayBuildChunk()
    {
        while(toGenerate.Count > 0)
        {
            BuildChunk(toGenerate[0].x, toGenerate[0].z);
            toGenerate.RemoveAt(0);

            yield return new WaitForSeconds(.2f);
        }
    }


}
