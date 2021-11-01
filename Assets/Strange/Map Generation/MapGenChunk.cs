using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenChunk : MonoBehaviour
{

    public GameObject chunkObject;
    public int x;
    public int y;

    // this contains all the data neccessary to generate the mesh of the map
    struct MeshData
    {
        public Vector3[] vertices;
        public int[] indices;
        public Vector2[] uvs;
        public Color[] vertexColours;
    }

    private MapGenerator mapGen;

    public MapGenChunk(MapGenerator mapGenerator)
    {
        mapGen = mapGenerator;
    }
    public void GenerateMapChunk()
    {
        // if the chunk gameobject already exists, no need to load the prefab
        if (!chunkObject)
            chunkObject = Instantiate(mapGen.MapPrefab, mapGen.mapParent);
        chunkObject.transform.SetPositionAndRotation(new Vector3(x * (mapGen.vertexWidth-1) * mapGen.size, 0, y * (mapGen.vertexHeight-1) * mapGen.size), Quaternion.identity);

        // calculate position of chunk: global offset + chunk's local space
        Vector2 chunkOffset = mapGen.globalOffset + new Vector2((mapGen.vertexWidth-1) * x, (mapGen.vertexHeight-1) * y);



        Mesh mesh = new Mesh();
        chunkObject.GetComponent<MeshFilter>().mesh = mesh;
        MeshCollider chunkCollider = chunkObject.GetComponent<MeshCollider>();
        chunkCollider.sharedMesh = mesh;

        // generate all the octaves' noisemaps
        for (int i = 0; i < mapGen.octaves.Length; i++)
        {
            mapGen.octaves[i].noiseMap = Noise.GenerateNoiseMap(mapGen.vertexWidth, mapGen.vertexHeight, mapGen.octaves[i].frequency, mapGen.octaves[i].amplitude, mapGen.octaves[i].offset, chunkOffset);
        }

        // merge all the octaves together into one noisemap
        float[,] noiseMap = MergeOctaves();
        // normalise the noisemap if required
        if (mapGen.normalize)
        {
                noiseMap = Noise.NormalizeNoiseMap(noiseMap); // get the min and max of the homechunk
        }
        // process the noisemap into mesh data (vertices, indices, and vertex colours)
        MeshData meshData = GenerateMeshData(noiseMap);
        // display the mesh on the screen
        GenerateMesh(meshData, mesh);
    }

    /// <summary>
    /// <para> merges multiple noise maps into 1 </para>
    /// Warning: output noise map will not range between 0 and 1 and may need normalizing
    /// </summary>
    float[,] MergeOctaves()
    {
        float[,] temp = new float[mapGen.vertexWidth, mapGen.vertexHeight];

        for (int y = 0; y < mapGen.vertexHeight; y++)
        {
            for (int x = 0; x < mapGen.vertexWidth; x++)
            {
                float value = 0;
                foreach (MapGenerator.Octave o in mapGen.octaves)
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
        meshData.vertices = new Vector3[mapGen.vertexWidth * mapGen.vertexHeight];
        meshData.uvs = new Vector2[mapGen.vertexWidth * mapGen.vertexHeight];

        // loop through noise
        for (int i = 0, y = 0; y < mapGen.vertexHeight; y++)
        {
            for (int x = 0; x < mapGen.vertexWidth; x++)
            {
                // set x and z based on scale
                // set y based on noise * yScale
                if (!mapGen.useHeightCurve)
                {
                    meshData.vertices[i] = new Vector3(x * mapGen.size, noiseMap[x, y] * mapGen.heightMultiplier * mapGen.size, y * mapGen.size);
                }
                else
                {
                    meshData.vertices[i] = new Vector3(x * mapGen.size, mapGen.heightCurve.Evaluate(noiseMap[x, y]) * mapGen.heightMultiplier * mapGen.size, y * mapGen.size);
                }

                meshData.uvs[i] = new Vector2(x / (float)mapGen.vertexWidth, y / (float)mapGen.vertexHeight);
                i++;
            }
        }

        // ####### generate indices #######
        int facesWidth = mapGen.vertexWidth - 1;                                            // Imortant note:
        int facesHeight = mapGen.vertexHeight - 1;                                          //  -  vertexWidth refers to the number of vertices along the x axis
        meshData.indices = new int[(mapGen.vertexWidth - 1) * (mapGen.vertexHeight - 1) * 6];          //  -  facesWidth refers to the number of faces along the x axis
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
        float[,] gradientRange = new float[mapGen.vertexWidth, mapGen.vertexHeight];
        float minRange = float.MaxValue;
        float maxRange = float.MinValue;
        for (int i = 0, y = 0; y < mapGen.vertexHeight; y++)
        {
            for (int x = 0; x < mapGen.vertexWidth; x++)
            {
                List<float> vertices = new List<float>();
                // center
                vertices.Add(meshData.vertices[i].y);
                // right  
                if (x < mapGen.vertexWidth - 1)
                    vertices.Add(meshData.vertices[i + 1].y);
                // left
                if (x > 0)
                    vertices.Add(meshData.vertices[i - 1].y);
                // top left
                if (y < mapGen.vertexHeight - 1 && x > 0)
                    vertices.Add(meshData.vertices[i + mapGen.vertexWidth - 1].y);
                // up
                if (y < mapGen.vertexHeight - 1)
                    vertices.Add(meshData.vertices[i + mapGen.vertexWidth].y);
                // top right
                if (y < mapGen.vertexHeight - 1 && x < mapGen.vertexWidth - 1)
                    vertices.Add(meshData.vertices[i + mapGen.vertexWidth + 1].y);
                // bottom left
                if (y > 0 && x > 0)
                    vertices.Add(meshData.vertices[i - mapGen.vertexWidth - 1].y);
                // bottom
                if (y > 0)
                    vertices.Add(meshData.vertices[i - mapGen.vertexWidth].y);
                // bottom right
                if (y > 0 && x < mapGen.vertexWidth - 1)
                    vertices.Add(meshData.vertices[i - mapGen.vertexWidth + 1].y);

                float range = vertices.Max() - vertices.Min();
                if (range < minRange) minRange = range;
                if (range > maxRange) maxRange = range;
                gradientRange[x, y] = range;

                i++;
            }
        }
        // ##### normalise ranges #####
        for (int y = 0; y < gradientRange.GetLength(1); y++)
        {
            for (int x = 0; x < gradientRange.GetLength(0); x++)
            {
                gradientRange[x, y] = Mathf.InverseLerp(minRange, maxRange, gradientRange[x, y]) * mapGen.colourGradientSensitivity;
            }
        }

        // ##### set colours based on normalised ranges #####
        for (int i = 0, y = 0; y < mapGen.vertexHeight; y++)
        {
            for (int x = 0; x < mapGen.vertexWidth; x++)
            {

                meshData.vertexColours[i] = mapGen.colourGradient.Evaluate(gradientRange[x, y]);
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

        for (int i = 0; i < colours.Count; i++)
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
    void GenerateMesh(MeshData meshData, Mesh mesh)
    {
        mesh.Clear();
        mesh.vertices = meshData.vertices;
        mesh.triangles = meshData.indices;
        //mesh.uv = meshData.uvs;
        mesh.colors = meshData.vertexColours;

        mesh.RecalculateNormals();
    }
}
