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
    public int vertexWidth;
    public int vertexHeight;

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
        // if mesh already exists, clear it before we make a new one
        if (mesh)
            mesh.Clear();
        // generate a mesh and set/get the required components
        mesh = new Mesh();
        mapGameObject.mesh = mesh;
        mapCollider.sharedMesh = mesh;

        // generate all the octaves' noisemaps
        for (int i = 0; i < octaves.Length; i ++)
        {
            octaves[i].noiseMap = Noise.GenerateNoiseMap(vertexWidth, vertexHeight, octaves[i].frequency, octaves[i].amplitude, octaves[i].offset, globalOffset);
        }

        // merge all the octaves together into one noisemap
        float[,] noiseMap = MergeOctaves();
        // normalise the noisemap if required
        if(normalize)
            noiseMap = Noise.NormalizeNoiseMap(noiseMap);
        // process the noisemap into mesh data (vertices, indices, and vertex colours)
        MeshData meshData = GenerateMeshData(noiseMap);
        // display the mesh on the screen
        GenerateMesh(meshData);
    }

    /// <summary>
    /// <para> merges multiple noise maps into 1 </para>
    /// Warning: output noise map will not range between 0 and 1 and may need normalizing
    /// </summary>
    float[,] MergeOctaves()
    {
        float[,] temp = new float[vertexWidth, vertexHeight];

        for (int y = 0; y < vertexHeight; y++)
        {
            for (int x = 0; x < vertexWidth; x++)
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

    /// <summary>
    /// takes in a noisemap and generates meshData
    /// <para>a vertex and index array which describes how to conect the vertices into triangles</para>
    /// <para>UV coodrinates in case the user wants to layer a texture on the map</para>
    /// <para> Vertex Colours based on the steepness of hills</para>
    /// </summary>
    /// <param name="noiseMap"> a 2d array of floats, generated via layering perlean noise</param>
    /// <returns>Meshdata: a struct containing all the details needed to compile a mesh during runtime</returns>
    MeshData GenerateMeshData(float[,] noiseMap)
    {
        MeshData meshData = new MeshData();

        // ####### generate Vertex positions #######
        meshData.vertices = new Vector3[vertexWidth * vertexHeight];
        meshData.uvs = new Vector2[vertexWidth * vertexHeight];

        // loop through noise
        for (int i = 0, y = 0; y < vertexHeight; y++)
        {
            for (int x = 0; x < vertexWidth; x++)
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

                meshData.uvs[i] = new Vector2(x / (float)vertexWidth, y / (float)vertexHeight);
                i++;
            }
        }

        // ####### generate indices #######
        int facesWidth = vertexWidth - 1;                                            // Imortant note:
        int facesHeight = vertexHeight - 1;                                          //  -  vertexWidth refers to the number of vertices along the x axis
        meshData.indices = new int[(vertexWidth-1) * (vertexHeight-1) * 6];          //  -  facesWidth refers to the number of faces along the x axis
        int currentVertex = 0;                                                       //  
        int currentTriangle = 0;                                                     //  use vertexWidth/vertexHeight when working with vertices
        for (int z = 0; z < facesHeight; z++)                                        //  use facesWidth/facesHeight when working with faces
        {                                                                            //  this helps a lot with confusion
            for (int x = 0; x < facesWidth; x++)
            {
                meshData.indices[currentTriangle + 0] = currentVertex + 0;                //  +    +
                meshData.indices[currentTriangle + 1] = currentVertex + facesWidth + 1;   //  | \
                meshData.indices[currentTriangle + 2] = currentVertex + 1;                //  +----+

                meshData.indices[currentTriangle + 3] = currentVertex + 1;                //  +----+
                meshData.indices[currentTriangle + 4] = currentVertex + facesWidth + 1;   //    \  |
                meshData.indices[currentTriangle + 5] = currentVertex + facesWidth + 2;   //  +    +


                currentVertex++;
                currentTriangle += 6;
            }
            currentVertex++;
        }




        meshData.vertexColours = new Color[meshData.vertices.Length];

        // ##### calculate ranges #####
        float[,] gradientRange = new float[vertexWidth , vertexHeight];
        float minRange = float.MaxValue;
        float maxRange = float.MinValue;
        for (int i = 0, y = 0; y < vertexHeight; y++)
        {
            for (int x = 0; x < vertexWidth; x++)
            {
                List<float> vertices = new List<float>();
                // center
                vertices.Add(meshData.vertices[i].y);
                // right  
                if (x < vertexWidth - 1)                     
                    vertices.Add(meshData.vertices[i + 1].y);
                // left
                if (x > 0)
                    vertices.Add(meshData.vertices[i - 1].y);
                // top left
                if (y < vertexHeight - 1 && x > 0)
                    vertices.Add(meshData.vertices[i + vertexWidth - 1].y);
                // up
                if(y < vertexHeight - 1)
                    vertices.Add(meshData.vertices[i + vertexWidth].y);
                // top right
                if (y < vertexHeight - 1 && x < vertexWidth - 1)
                    vertices.Add(meshData.vertices[i + vertexWidth + 1].y);
                // bottom left
                if (y > 0 && x > 0)
                    vertices.Add(meshData.vertices[i - vertexWidth - 1].y);
                // bottom
                if(y > 0)
                    vertices.Add(meshData.vertices[i - vertexWidth].y);
                // bottom right
                if(y > 0 && x < vertexWidth - 1)
                    vertices.Add(meshData.vertices[i - vertexWidth + 1].y);   

                float range = vertices.Max() - vertices.Min();
                if (range < minRange) minRange = range;
                if (range > maxRange) maxRange = range;
                gradientRange[x,y] = range;

                i++;
            }
        }
        // ##### normalise ranges #####
        for (int y = 0; y < gradientRange.GetLength(1); y++)
        {
            for (int x = 0; x < gradientRange.GetLength(0); x++)
            {
                gradientRange[x,y] = Mathf.InverseLerp(minRange, maxRange, gradientRange[x,y]) * colourGradientSensitivity;
            }
        }

        // ##### set colours based on normalised ranges #####
        for (int i=0, y = 0; y < vertexHeight; y++)
        {
            for (int x = 0; x < vertexWidth; x++)
            {

                meshData.vertexColours[i] = colourGradient.Evaluate(gradientRange[x, y]);
                i++;
            }
            //i++;
        }

        return meshData;
    }
    
    /// <summary>
    /// processes a list of colours and returns the average of all these colours
    /// </summary>
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

    /// <summary>
    /// using meshdata it compiles a mesh.  can be done at runtime
    /// </summary>
    /// <param name="meshData"></param>
    void GenerateMesh(MeshData meshData)
    {
        mesh.Clear();
        mesh.vertices = meshData.vertices;
        mesh.triangles = meshData.indices;
        //mesh.uv = meshData.uvs;
        mesh.colors = meshData.vertexColours;

        mesh.RecalculateNormals();
    }

    /// <summary>
    /// if any inspector inputs go out of range, this will set them back inside their range
    /// <para> for example: it is impossible to have a negative map width</para>
    /// </summary>
    private void OnValidate()
    {
        if (vertexWidth < 1) vertexWidth = 1;
        if (vertexHeight < 1) vertexHeight = 1;
        if (vertexHeight > 255) vertexHeight = 255;
        if (vertexWidth > 255) vertexWidth = 255;
        if (heightMultiplier < 0) heightMultiplier = 0;
        if (size < 0) size = 0;
        if (useHeightCurve && !normalize) normalize = true;
        if (colourGradientSensitivity < 0) colourGradientSensitivity = 0;

        for(int i = 0; i < octaves.Length;i++)
        {
            if (octaves[i].amplitude < 0) octaves[i].amplitude = 0;
            if (octaves[i].frequency < 0) octaves[i].frequency = 0;
        }
    }

}
