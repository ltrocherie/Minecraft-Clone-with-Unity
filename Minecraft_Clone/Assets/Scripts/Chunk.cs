using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {


        Mesh mesh = new Mesh();

        GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = {
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1)
        };

        int [] triangles = { 
            0, 1, 2, 
            1, 3, 2,
            1, 5, 6,
            1, 6, 3
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
