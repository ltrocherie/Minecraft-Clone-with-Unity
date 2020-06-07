using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    public GameObject chunk;

    FastNoise noise = new FastNoise();

    // Start is called before the first frame update
    void Start()
    {

        int limit = 64;

        for(int x = -limit; x < limit; x += 15)
        {
            for(int z = -limit; z < limit; z += 15)
            {
                //print(noise.GetSimplex(x, z));
                GameObject chunkInst = Instantiate(chunk, new Vector3(x, 0, z), Quaternion.identity);
                chunkInst.transform.SetParent(gameObject.transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
