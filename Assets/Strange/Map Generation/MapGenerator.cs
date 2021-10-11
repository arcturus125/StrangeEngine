using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    Mesh mesh;
    public MeshFilter mapOject;

    public int width;
    public int height;
    public float scaleY;
    public float scale;

    public bool autoUpdate = false;

    Vector3[] vertices;
    int[] triangles;

    [System.Serializable]
    public struct Octave
    {
        public float[,] noiseMap;
        public float amplitude;
        public float frequency;
        public Vector2 offset;
    }
    public Octave[] octaves;




    public void GenerateMap()
    {

        if (mesh)
            mesh.Clear();
        mesh = new Mesh();
        mapOject.mesh = mesh;

        // generate all the octaves
        for (int i = 0; i < octaves.Length; i ++)
        {
            octaves[i].noiseMap = Noise.GenerateNoiseMap(width, height, octaves[i].frequency, octaves[i].amplitude, octaves[i].offset);
        }

        // merge all the octaves
        float[,] mergedOctaves = MergeOctaves();
        // normalise the octaves
        //float[,] normalisedOctaves = Noise.NormalizeNoiseMap(mergedOctaves);
        // display the map
        CreateShape(mergedOctaves);
        GenerateMesh();
    }

    float[,] MergeOctaves()
    {
        float[,] temp = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float value = 0;
                foreach (Octave o in octaves)
                {
                    value += o.noiseMap[x, y];
                }
                temp[x, y] = value;
            }
        }
        return temp;
    }

    void CreateShape(float[,] noiseMap)
    {

        // ####### store Vertex positions
        vertices = new Vector3[width * height];

        // loop through noise
        for (int i = 0, y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // set x and z based on scale
                // set y based on noise * yScale
                vertices[i] = new Vector3(x*scale, noiseMap[x,y]*scaleY * scale, y*scale);
                i++;
            }
        }

        // ######## make triangles
        int xSize = width - 1;
        int ySize = height - 1;
        triangles = new int[(width-1) * (height-1) * 6];
        int currentVertex = 0;
        int currentTriangle = 0;
        for (int z = 0; z < ySize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[currentTriangle + 0] = currentVertex + 0;
                triangles[currentTriangle + 1] = currentVertex + xSize + 1;
                triangles[currentTriangle + 2] = currentVertex + 1;
                triangles[currentTriangle + 3] = currentVertex + 1;
                triangles[currentTriangle + 4] = currentVertex + xSize + 1;
                triangles[currentTriangle + 5] = currentVertex + xSize + 2;


                currentVertex++;
                currentTriangle += 6;
            }
            currentVertex++;
        }



    }

    void GenerateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }

}
