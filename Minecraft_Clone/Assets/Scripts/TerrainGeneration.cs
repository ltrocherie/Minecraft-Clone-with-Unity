using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    public GameObject cube;

    FastNoise noise = new FastNoise();

    // Start is called before the first frame update
    void Start()
    {

        int limit = 50;

        for(int x = -limit; x < limit; x++)
        {
            for(int z = -limit; z < limit; z++)
            {
                //print(noise.GetSimplex(x, z));
                float y = (float)Math.Floor(noise.GetSimplex(x * .8f, z * .8f) * 20);
                GameObject cubeInst = Instantiate(cube, new Vector3(x, y, z), Quaternion.identity);
                cubeInst.transform.SetParent(gameObject.transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
