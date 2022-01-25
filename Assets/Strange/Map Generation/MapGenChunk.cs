using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenChunk : ScriptableObject
{
    [HideInInspector]
    public MapGenerator mapGenSettings;
    public int x;
    public int y;


    ChunkComponent component;
    public float[,] finalNoiseMap;
    private Vector3 chunkPos;
    private Mesh mesh;

    // this contains all the data neccessary to generate the mesh of the map
    [System.Serializable]
    public struct MeshData
    {
        public Vector3[] vertices;
        public int[] indices;
        public Vector2[] uvs;
        public Color[] vertexColours;
    }
    public MeshData meshData;

    public MapGenChunk(MapGenerator mapGenerator)
    {
        mapGenSettings = mapGenerator;
    }


    public void GenerateMapChunk(GameObject chunkObject = null)
    {
        chunkObject = GameObject.Find($"Chunk {x} {y}");
        if (chunkObject == null)
            chunkObject = Instantiate(mapGenSettings.MapPrefab, mapGenSettings.mapParent);

        chunkObject.name = $"Chunk {x} {y}";
        chunkPos = new Vector3(x * (mapGenSettings.vertexWidth - 1) * mapGenSettings.size, 0, y * (mapGenSettings.vertexHeight - 1) * mapGenSettings.size);
        chunkObject.transform.SetPositionAndRotation(chunkPos, Quaternion.identity);
        if (chunkObject.GetComponent<ChunkComponent>())
        {
            component = chunkObject.GetComponent<ChunkComponent>();
        }
        else
        {
            component = chunkObject.AddComponent<ChunkComponent>();
        }

        // calculate position of chunk: global offset + chunk's local space
        Vector2 chunkOffset = mapGenSettings.globalOffset + new Vector2((mapGenSettings.vertexWidth-1) * x, (mapGenSettings.vertexHeight-1) * y);



        mesh = new Mesh();
        chunkObject.GetComponent<MeshFilter>().mesh = mesh;
        MeshCollider chunkCollider = chunkObject.GetComponent<MeshCollider>();
        chunkCollider.sharedMesh = mesh;

        // generate all the octaves' noisemaps
        for (int i = 0; i < mapGenSettings.octaves.Length; i++)
        {
            mapGenSettings.octaves[i].noiseMap = Noise.GenerateNoiseMap(
                mapGenSettings.vertexWidth + 2, mapGenSettings.vertexHeight + 2, // add 2 to the width and height to create a layer of padding which is never rendered (fixes colour stitching)
                mapGenSettings.octaves[i].frequency, mapGenSettings.octaves[i].amplitude, mapGenSettings.octaves[i].offset, // pass the octave's details over
                chunkOffset, // add the chunk offset so that the chunk gameobject's position lines up with the chunk's noise
                mapGenSettings.detail // this is kind of like the opposite of LOD tesselation. increasing this number "zooms in" on the noise - its like  a higher resolution image. it has more pixels, but you have to zoom out to see them all
                );
        }

        // merge all the octaves together into one noisemap
        finalNoiseMap = MergeOctaves();
        // normalise the noisemap if required
        if (mapGenSettings.normalize)
        {
            finalNoiseMap = Noise.NormalizeNoiseMap(finalNoiseMap); // get the min and max of the homechunk
        }

        if(mapGenSettings.useFalloffMap)
        {
            finalNoiseMap = FalloffMap(finalNoiseMap);
        }


        // process the noisemap into mesh data (vertices, indices, and vertex colours)
        meshData = GenerateMeshData(finalNoiseMap);
        // display the mesh on the screen
        GenerateMesh(meshData);

        component.dataFile = this;
        component.data = meshData;
        component.noiseMap = finalNoiseMap;
        component.width = mapGenSettings.vertexWidth;
        component.height = mapGenSettings.vertexHeight;
        component.size = mapGenSettings.size;
        component.colourGradient = mapGenSettings.colourGradient;
        component.colourGradientSensitivity = mapGenSettings.colourGradientSensitivity;
    }

    /// <summary>
    /// <para> merges multiple noise maps into 1 </para>
    /// Warning: output noise map will not range between 0 and 1 and may need normalizing
    /// </summary>
    float[,] MergeOctaves()
    {
        float[,] temp = new float[mapGenSettings.vertexWidth+2, mapGenSettings.vertexHeight+2];

        for (int y = 0; y < mapGenSettings.vertexHeight+2; y++)
        {
            for (int x = 0; x < mapGenSettings.vertexWidth+2; x++)
            {
                float value = 0;
                foreach (MapGenerator.Octave o in mapGenSettings.octaves)
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
        meshData.vertices = new Vector3[mapGenSettings.vertexWidth * mapGenSettings.vertexHeight];
        meshData.uvs = new Vector2[mapGenSettings.vertexWidth * mapGenSettings.vertexHeight];



        /* DEV NOTE:
         *  because of the colour stiching issue, i have added a layer of padding around the noise that serves as an overlap
         *  this fixes the issue, but makes this block of code significantly harder because i can no longer just iterate through all the
         *  noise. i have to ignore the padding which requires extra checks and awkward iteration
         */

        // loop through noise
        for (int i = 0, y = 1; y < (mapGenSettings.vertexHeight+1); y++)
        {
            for (int x = 1; x <( mapGenSettings.vertexWidth+1); x++)
            {
                // set x and z based on scale
                // set y based on noise * yScale
                if (!mapGenSettings.useHeightCurve)
                {
                    meshData.vertices[i] = new Vector3(x * mapGenSettings.size, noiseMap[x, y] * mapGenSettings.heightMultiplier * mapGenSettings.size, y * mapGenSettings.size);
                }
                else
                {
                    meshData.vertices[i] = new Vector3(x * mapGenSettings.size, mapGenSettings.heightCurve.Evaluate(noiseMap[x, y]) * mapGenSettings.heightMultiplier * mapGenSettings.size, y * mapGenSettings.size);
                }

                meshData.uvs[i] = new Vector2((float)x / (float)mapGenSettings.vertexWidth, (float)y / (float)mapGenSettings.vertexHeight);
                i++;
            }
        }

        // ####### generate indices #######
        int facesWidth = mapGenSettings.vertexWidth - 1;                                              // Imortant note:
        int facesHeight = mapGenSettings.vertexHeight - 1;                                            //  -  vertexWidth refers to the number of vertices along the x axis
        meshData.indices = new int[(mapGenSettings.vertexWidth - 1) * (mapGenSettings.vertexHeight - 1) * 6]; //  -  facesWidth refers to the number of faces along the x axis
        int currentVertex = 0;                                                                //  
        int currentTriangle = 0;                                                              //  use vertexWidth/vertexHeight when working with vertices
        for (int z = 1; z < facesHeight+1; z++)                                               //  use facesWidth/facesHeight when working with faces
        {                                                                                     //  this helps a lot with confusion
            for (int x = 1; x < facesWidth+1; x++)
            {
                meshData.indices[currentTriangle + 0] = currentVertex + 0;                //  +    +
                meshData.indices[currentTriangle + 1] = currentVertex + facesWidth+ 1;    //  | \
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
        float[,] gradientRange = new float[mapGenSettings.vertexWidth, mapGenSettings.vertexHeight];
        
        for (int y = 1; y < (mapGenSettings.vertexHeight+1); y++)
        {
            for (int x = 1; x < (mapGenSettings.vertexWidth+1); x++)
            {
                List<float> vertices = new List<float>();


                // non height curve multiplied values - may be wrong
                if (!mapGenSettings.useHeightCurve)
                {
                    vertices.Add(noiseMap[x, y] * mapGenSettings.heightMultiplier * mapGenSettings.size);
                    vertices.Add(noiseMap[x + 1, y] * mapGenSettings.heightMultiplier * mapGenSettings.size);
                    vertices.Add(noiseMap[x - 1, y] * mapGenSettings.heightMultiplier * mapGenSettings.size);
                    vertices.Add(noiseMap[x, y + 1] * mapGenSettings.heightMultiplier * mapGenSettings.size);
                    vertices.Add(noiseMap[x + 1, y + 1] * mapGenSettings.heightMultiplier * mapGenSettings.size);
                    vertices.Add(noiseMap[x - 1, y + 1] * mapGenSettings.heightMultiplier * mapGenSettings.size);
                    vertices.Add(noiseMap[x, y - 1] * mapGenSettings.heightMultiplier * mapGenSettings.size);
                    vertices.Add(noiseMap[x + 1, y - 1] * mapGenSettings.heightMultiplier * mapGenSettings.size);
                    vertices.Add(noiseMap[x - 1, y - 1] * mapGenSettings.heightMultiplier * mapGenSettings.size);
                }
                else
                {
                    vertices.Add(mapGenSettings.heightCurve.Evaluate(noiseMap[x, y]) * mapGenSettings.heightMultiplier * mapGenSettings.size);
                    vertices.Add(mapGenSettings.heightCurve.Evaluate(noiseMap[x + 1, y]) * mapGenSettings.heightMultiplier * mapGenSettings.size);
                    vertices.Add(mapGenSettings.heightCurve.Evaluate(noiseMap[x - 1, y]) * mapGenSettings.heightMultiplier * mapGenSettings.size);
                    vertices.Add(mapGenSettings.heightCurve.Evaluate(noiseMap[x, y + 1]) * mapGenSettings.heightMultiplier * mapGenSettings.size);
                    vertices.Add(mapGenSettings.heightCurve.Evaluate(noiseMap[x + 1, y + 1]) * mapGenSettings.heightMultiplier * mapGenSettings.size);
                    vertices.Add(mapGenSettings.heightCurve.Evaluate(noiseMap[x - 1, y + 1]) * mapGenSettings.heightMultiplier * mapGenSettings.size);
                    vertices.Add(mapGenSettings.heightCurve.Evaluate(noiseMap[x, y - 1]) * mapGenSettings.heightMultiplier * mapGenSettings.size);
                    vertices.Add(mapGenSettings.heightCurve.Evaluate(noiseMap[x + 1, y - 1]) * mapGenSettings.heightMultiplier * mapGenSettings.size);
                    vertices.Add(mapGenSettings.heightCurve.Evaluate(noiseMap[x - 1, y - 1]) * mapGenSettings.heightMultiplier * mapGenSettings.size);
                }

                float range = vertices.Max() - vertices.Min();
                //if (range < minRange) minRange = range;
                //if (range > maxRange) maxRange = range;


                gradientRange[x-1, y-1] = range;

            }
        }
        // ##### normalise ranges #####
        for (int y = 0; y < gradientRange.GetLength(1); y++)
        {
            for (int x = 0; x < gradientRange.GetLength(0); x++)
            {
                //gradientRange[x, y] = Mathf.InverseLerp(minRange, maxRange, gradientRange[x, y]) * mapGen.colourGradientSensitivity;

                gradientRange[x, y] = Mathf.InverseLerp(0, mapGenSettings.size * 2, gradientRange[x, y]) * mapGenSettings.colourGradientSensitivity;
            }
        }

        // ##### set colours based on normalised ranges #####
        for (int i = 0, y = 0; y < mapGenSettings.vertexHeight; y++)
        {
            for (int x = 0; x < mapGenSettings.vertexWidth; x++)
            {

                meshData.vertexColours[i] = mapGenSettings.colourGradient.Evaluate(gradientRange[x, y]);
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

    float[,] FalloffMap(float[,] heightmap)
    {
        float[,] temp = new float[mapGenSettings.vertexWidth + 2, mapGenSettings.vertexHeight + 2];

        for (int indexY = 0; indexY < mapGenSettings.vertexHeight + 2; indexY++)
        {
            for (int indexX = 0; indexX < mapGenSettings.vertexWidth + 2; indexX++)
            {
                Vector2 chunkPos2d = new Vector2(chunkPos.x, chunkPos.z);
                Vector2 test =  new Vector2(indexX * mapGenSettings.size, indexY * mapGenSettings.size);

                float distance = Vector2.Distance(chunkPos2d + test, mapGenSettings.mapOrigin) / mapGenSettings.falloffDistance;


                temp[indexX, indexY] = mapGenSettings.falloffCurve.Evaluate(distance ) * heightmap[indexX, indexY];
            }
        }

        return temp;

    }

    public void UpdateColours(Color[] texture)
    {
        for (int i = 0; i < meshData.vertexColours.Length; i++)
        {

            meshData.vertexColours[i] = texture[i];
        }
        GenerateMesh(meshData, false);
    }
    

    /// <summary>
    /// using meshdata it compiles a mesh.  can be done at runtime
    /// </summary>
    /// <param name="meshData"></param>
    public void GenerateMesh(MeshData meshData, bool clearPreviousData = true)
    {
        if(clearPreviousData) mesh.Clear();
        mesh.vertices = meshData.vertices;
        mesh.triangles = meshData.indices;
        mesh.uv = meshData.uvs;
        mesh.colors = meshData.vertexColours;

        mesh.RecalculateNormals();
    }

    
}
