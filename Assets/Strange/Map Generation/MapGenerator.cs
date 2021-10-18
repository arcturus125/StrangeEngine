using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Header("Colour settings:")]
    public Gradient colourGradient;
    public float colourGradientSensitivity = 4f;

    [Header("Global Offsets:")]
    public Vector2 globalOffset;


    public bool autoUpdate = false;

    // this contains all the data neccessary to generate the mesh of the map
    struct MeshData
    {
        public Vector3[] vertices;
        public int[] indices;
        public Vector2[] uvs;
        public Color[] vertexColours;
    }

    // this contains all the data of each layer of noise
    // there can be many octaves of noise layered ontop of each other
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
            octaves[i].noiseMap = Noise.GenerateNoiseMap(noiseMapWidth, noiseMapHeight, octaves[i].frequency, octaves[i].amplitude, octaves[i].offset, globalOffset);
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
        meshData.uvs = new Vector2[noiseMapWidth * noiseMapHeight];

        // loop through noise
        for (int i = 0, y = 0; y < noiseMapHeight; y++)
        {
            for (int x = 0; x < noiseMapWidth; x++)
            {
                // set x and z based on scale
                // set y based on noise * yScale
                if (!useHeightCurve)
                {
                    meshData.vertices[i] = new Vector3(x * size, noiseMap[x, y] * heightMultiplier * size, y * size);
                }
                else
                {
                    meshData.vertices[i] = new Vector3(x * size, heightCurve.Evaluate(noiseMap[x, y]) * heightMultiplier * size, y * size);
                }

                meshData.uvs[i] = new Vector2(x / (float)noiseMapWidth, y / (float)noiseMapHeight);
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




        meshData.vertexColours = new Color[meshData.vertices.Length];

        // ##### calculate ranges #####
        float[] gradientRange = new float[xSize * ySize];
        float minRange = float.MaxValue;
        float maxRange = float.MinValue;
        for (int i = 0, y = 0; y < xSize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                float[] vertices = new float[4];
                vertices[0] = meshData.vertices[i].y;
                vertices[1] = meshData.vertices[i + 1].y;
                vertices[2] = meshData.vertices[i + noiseMapWidth].y;
                vertices[3] = meshData.vertices[i + noiseMapWidth + 1].y;

                float range = vertices.Max() - vertices.Min();
                if (range < minRange) minRange = range;
                if (range > maxRange) maxRange = range;
                gradientRange[i] = range;

                i++;
            }
        }
        // ##### normalise ranges #####
        for (int i = 0; i < gradientRange.Length; i++)
        {
            gradientRange[i] = Mathf.InverseLerp(minRange, maxRange, gradientRange[i]) * colourGradientSensitivity;
        }

        // ##### set colours based on normalised ranges #####
        for (int i = 0; i < gradientRange.Length; i++)
        {
            /* vertex i = gradient i, gradient i-1, gradient - (noisemapwiidth -1 ), i - (noisemapwidth -1)-1
             */
            List<Color> colours = new List<Color>();
            colours.Add (colourGradient.Evaluate(gradientRange[  i  ]));
            if (!(i % xSize == 0))
                colours.Add(colourGradient.Evaluate(gradientRange[  i-1  ]));
            if( i > xSize)
                colours.Add (colourGradient.Evaluate(gradientRange[  i-xSize  ]));
            if( i > xSize + 1)
                colours.Add (colourGradient.Evaluate(gradientRange[  i-xSize  ]));


            meshData.vertexColours[i] = AverageColours(colours);
        }



        return meshData;
    }
    
    Color AverageColours(List<Color> colours)
    {
        float r = 0;
        float g = 0;
        float b = 0;

        for(int i = 0; i < colours.Count; i ++)
        {
            r += colours[i].r;
            g += colours[i].g;
            b += colours[i].b;
        }

        return new Color(r / colours.Count, g / colours.Count, b / colours.Count);
    }


    void GenerateMesh(MeshData meshData)
    {
        mesh.Clear();
        mesh.vertices = meshData.vertices;
        mesh.triangles = meshData.indices;
        //mesh.uv = meshData.uvs;
        mesh.colors = meshData.vertexColours;

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
