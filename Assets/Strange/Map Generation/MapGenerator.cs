using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    Mesh mesh;
    public MeshFilter mapGameObject;
    public MeshCollider mapCollider;

    [Header("NoiseMap Size settings:")]
    public int noiseMapWidth;
    public int noiseMapHeight;
    [Header("Mesh Size Settings:")]
    public float heightMultiplier;
    public float size;
    [Header("Additional Settings:")]
    public bool normalize = false;
    public AnimationCurve heightCurve;
    public bool useHeightCurve = false;

    public bool autoUpdate = false;

    struct MeshData
    {
        public Vector3[] vertices;
        public int[] indices;
    }

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
        mapGameObject.mesh = mesh;
        mapCollider.sharedMesh = mesh;

        // generate all the octaves
        for (int i = 0; i < octaves.Length; i ++)
        {
            octaves[i].noiseMap = Noise.GenerateNoiseMap(noiseMapWidth, noiseMapHeight, octaves[i].frequency, octaves[i].amplitude, octaves[i].offset);
        }

        // merge all the octaves
        float[,] noiseMap = MergeOctaves();
        // normalise the octaves
        if(normalize)
            noiseMap = Noise.NormalizeNoiseMap(noiseMap);
        // display the map
        MeshData meshData = GenerateMeshData(noiseMap);
        GenerateMesh(meshData);
    }

    /// <summary>
    /// merges multiple noise maps into 1
    /// Warning: output noise map will not range between 0 and 1 and may need normalizing
    /// </summary>
    float[,] MergeOctaves()
    {
        float[,] temp = new float[noiseMapWidth, noiseMapHeight];

        for (int y = 0; y < noiseMapHeight; y++)
        {
            for (int x = 0; x < noiseMapWidth; x++)
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

    MeshData GenerateMeshData(float[,] noiseMap)
    {
        MeshData meshData = new MeshData();

        // ####### generate Vertex positions #######
        meshData.vertices = new Vector3[noiseMapWidth * noiseMapHeight];

        // loop through noise
        for (int i = 0, y = 0; y < noiseMapHeight; y++)
        {
            for (int x = 0; x < noiseMapWidth; x++)
            {
                // set x and z based on scale
                // set y based on noise * yScale
                if(!useHeightCurve)
                    meshData.vertices[i] = new Vector3(x*size, noiseMap[x,y]*heightMultiplier * size, y*size);
                else
                    meshData.vertices[i] = new Vector3(x * size, heightCurve.Evaluate(noiseMap[x, y]) * heightMultiplier * size, y * size);
                i++;
            }
        }

        // ####### generate triangles #######
        int xSize = noiseMapWidth - 1;
        int ySize = noiseMapHeight - 1;
        meshData.indices = new int[(noiseMapWidth-1) * (noiseMapHeight-1) * 6];
        int currentVertex = 0;
        int currentTriangle = 0;
        for (int z = 0; z < ySize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                meshData.indices[currentTriangle + 0] = currentVertex + 0;           //  +    +
                meshData.indices[currentTriangle + 1] = currentVertex + xSize + 1;   //  | \
                meshData.indices[currentTriangle + 2] = currentVertex + 1;           //  +----+

                meshData.indices[currentTriangle + 3] = currentVertex + 1;           //  +----+
                meshData.indices[currentTriangle + 4] = currentVertex + xSize + 1;   //    \  |
                meshData.indices[currentTriangle + 5] = currentVertex + xSize + 2;   //  +    +


                currentVertex++;
                currentTriangle += 6;
            }
            currentVertex++;
        }


        return meshData;
    }

    void GenerateMesh(MeshData meshData)
    {
        mesh.Clear();
        mesh.vertices = meshData.vertices;
        mesh.triangles = meshData.indices;

        mesh.RecalculateNormals();
    }

    private void OnValidate()
    {
        if (noiseMapWidth < 1) noiseMapWidth = 1;
        if (noiseMapHeight < 1) noiseMapHeight = 1;
        if (noiseMapHeight > 255) noiseMapHeight = 255;
        if (noiseMapWidth > 255) noiseMapWidth = 255;
        if (heightMultiplier < 0) heightMultiplier = 0;
        if (size < 0) size = 0;
        if (useHeightCurve && !normalize) normalize = true;

        for(int i = 0; i < octaves.Length;i++)
        {
            if (octaves[i].amplitude < 0) octaves[i].amplitude = 0;
            if (octaves[i].frequency < 0) octaves[i].frequency = 0;
        }
    }

}
