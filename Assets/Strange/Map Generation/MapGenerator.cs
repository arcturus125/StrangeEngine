using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    // ###############################################################################
    // #                              INSPECTOR SETTINGS                             #
    // ###############################################################################
    [Tooltip("The 'folder' that all map chunks will be a child of - use for hierarchy management of the map. if left blank a folder will be createdd for you")]
    public Transform mapParent;
    [Tooltip("the prefab that is spawned in and then generated")]
    public GameObject MapPrefab;

    [Header("NoiseMap Size settings:")]
    [Tooltip("The width (x axis) of the noise, max 255")]
    public int vertexWidth;
    [Tooltip("The height/length (z axis) of the noise, max 255")]
    public int vertexHeight;

    [Header("Mesh Size Settings:")]
    [Tooltip("the Y scale of the map (to control the scale of mountains)")]
    public float heightMultiplier;
    [Tooltip("the XZ scale of the map")]
    public float size;
    [Tooltip("The scale of the noise. setting this to 2 will mean you will sample twice as many points from the noise, however this also incidentally scales the map.")]
    public float detail;
    [Tooltip("The radius of chunks around the player that will be generated")]
    public int renderDistance = 1;

    [Header("Additional Settings:")]
    [Tooltip("clamp all noise values between 0 and 1")]
    public bool normalize = false;
    [Tooltip("a curve that helps you control the steepness of the map at any noise value")]
    public AnimationCurve heightCurve;
    [Tooltip("apply the above curve to the map - values must be normalised first!")]
    public bool useHeightCurve = false;

    [Header("Colour settings:")]
    [Tooltip("the colour shading applied to the map. left side is applied to flat areas and the right side is applied to the steep areas")]
    public Gradient colourGradient;
    [Range(0.1f, 2.0f)]
    [Tooltip("controls the sensitivity of this colour gradient. the higher this number is, the more values get pushed to the upper end of the colour gradient above")]
    public float colourGradientSensitivity = 4f;

    [Header("Global Offsets:")]
    public Vector2 globalOffset;

    [Header("Falloff map")]
    public AnimationCurve falloffCurve;
    public bool useFalloffMap = true;
    [HideInInspector]
    public Vector2 mapOrigin;
    [HideInInspector]
    public float falloffDistance;


    [Header("Auto Generate")]
    public bool autoUpdate = false;

    private bool forceUpdate = true;


    [Header("Octave randomizer")]
    [Range(3,10)]
    public int noOfOctaves = 4;
    [Range(0.1f, 5.0f)]
    public float Smoothness = 1;



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


    // ###############################################################################
    // #                                chunk manager                                #
    // ###############################################################################

    public static float theoretical_min;
    public static float theoretical_max;

    public void GenerateMap()
    {
        if(mapParent == null)
        {
            mapParent = new GameObject("Map").transform;
        }

        mapOrigin = new Vector2((vertexWidth * size) / 2, (vertexHeight* size) / 2);

        falloffDistance = Vector2.Distance(mapOrigin, new Vector2(vertexWidth * renderDistance * size, vertexHeight * renderDistance * size));

        CalculateTheoreticals();

        if (renderDistance == 0)
            ChunkManager.Generate(0, 0, this, forceUpdate);
        else
        {
            for (int Y = -renderDistance; Y <= renderDistance; Y++)
            {
                for (int X = -renderDistance; X <= renderDistance; X++)
                {
                    ChunkManager.Generate(X, Y, this, forceUpdate);
                }
            }
        }

        forceUpdate = false;
    }
    private void CalculateTheoreticals()
    {
        float max = 0;
        foreach(Octave o in octaves)
        {
            max += o.amplitude;
        }
        theoretical_min = -max;
        theoretical_max = max;
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
        if (detail < 1) detail = 1; 
        if (useHeightCurve && !normalize) normalize = true;
        if (colourGradientSensitivity < 0) colourGradientSensitivity = 0;

        for(int i = 0; i < octaves.Length;i++)
        {
            if (octaves[i].amplitude < 0) octaves[i].amplitude = 0;
            if (octaves[i].frequency < 0) octaves[i].frequency = 0;
        }

        forceUpdate = true;
    }



    public void RandomiseOctaves()
    {
        List<Octave> tempList = new List<Octave>();

        int maxAmplitude = UnityEngine.Random.Range(5, 8);
        int maxFrequency = UnityEngine.Random.Range(150, 280);

        for (int i = 0; i < noOfOctaves; i++)
        {
            Octave o = new Octave();
            if (i != 0)
            {
                o.amplitude = maxAmplitude / (Mathf.Pow(i, 2) * (i * Smoothness));
                o.frequency = maxFrequency / Mathf.Pow(i, 2);
            }
            else
            {
                o.amplitude = maxAmplitude;
                o.frequency = maxFrequency;
            }

            int offset = UnityEngine.Random.Range(100, 1000);
            o.offset = new Vector2(offset, offset);

            tempList.Add(o);
        }
        octaves = tempList.ToArray();
    }

}
static class ChunkManager
{
    const string MapGenDataParentFolder = "Assets/Resources";
    const string MapGenDataFolderName = "MapGenData";

    const string MapGenDataPath = MapGenDataParentFolder + "/"+ MapGenDataFolderName;

    public static void Generate(int x ,int y, MapGenerator mapGen, bool forceUpdate)
    {
        if (!AssetDatabase.IsValidFolder(MapGenDataPath))
            AssetDatabase.CreateFolder(MapGenDataParentFolder, MapGenDataFolderName);


        MapGenChunk chunk;
        if (!DoesChunkExist(x, y))
        {
            //chunk = new MapGenChunk(mapGen);
            chunk = ScriptableObject.CreateInstance<MapGenChunk>();
            chunk.mapGenSettings = mapGen;
            chunk.x = x;
            chunk.y = y;

            AssetDatabase.CreateAsset(chunk, $"{MapGenDataPath}/chunk {x} {y}.asset");
            AssetDatabase.SaveAssets();

            chunk.GenerateMapChunk();
        }
        else
        {
            chunk = LoadChunk(x, y);
            if (forceUpdate)
            {
                chunk.GenerateMapChunk();
            }
            else
            {
                //chunk.mapGen = mapGen; // update the settings?

                chunk.GenerateMesh(chunk.meshData, false);
                Debug.Log($"loading chunk {x} {y} from file");
            }
        }
        

    }

    public static bool DoesChunkExist(int x, int y)
    {
        DirectoryInfo dir = new DirectoryInfo(MapGenDataPath);
        FileInfo[] info = dir.GetFiles("*.*");

        foreach (FileInfo f in info)
        {
            if (f.Name == $"chunk {x} {y}.asset") return true;
        }
        return false;
    }
    public static MapGenChunk LoadChunk(int x, int y)
    {
         return Resources.Load<MapGenChunk>($"{MapGenDataFolderName}/chunk {x} {y}");
    }


}



