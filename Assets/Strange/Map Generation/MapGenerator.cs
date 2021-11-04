using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    // ###############################################################################
    // #                              INSPECTOR SETTINGS                             #
    // ###############################################################################
    [Header("The 'folder' that all map chunks will be a child of")]
    public Transform mapParent;
    public GameObject MapPrefab;

    [Header("NoiseMap Size settings:")]
    public int vertexWidth;
    public int vertexHeight;

    [Header("Mesh Size Settings:")]
    public float heightMultiplier;
    public float size;
    public float detail;
    public int renderDistance = 1;

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
        CalculateTheoreticals();
        DeleteForgottenChunks();


        for (int Y = -renderDistance; Y <= renderDistance; Y++)
        {
            for (int X = -renderDistance; X <= renderDistance; X++)
            {
                ChunkManager.Generate(X,Y, this);
            }
        }

        //ChunkManager.Generate(0, -1, this);
        //ChunkManager.Generate(0, 1, this);

        //ChunkManager.Generate(1, 0, this);
        //ChunkManager.Generate(1, -1, this);
        //ChunkManager.Generate(1, 1, this);

        //ChunkManager.Generate(-1, 0, this);
        //ChunkManager.Generate(-1, -1, this);
        //ChunkManager.Generate(-1, 1, this);



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


    //if there are map gameobjects attached to the mapFolder that the script doesnt recognise,
    // delete all children and generate them again -- this doesnt happen often, at most every time unity is closed and re-opened
    private void DeleteForgottenChunks()
    {
        if(ChunkManager.savedChunks.Count != mapParent.childCount)
        {
            int childCount = mapParent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                DestroyImmediate(mapParent.GetChild(0).gameObject);
            }
            ChunkManager.savedChunks.Clear();
        }

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
    }

}
static class ChunkManager
{
    public static List<MapGenChunk> savedChunks = new List<MapGenChunk>();

    public static bool DoesChunkExist(int x, int y)
    {
        foreach (MapGenChunk chunk in savedChunks)
        {
            if(chunk.x == x && chunk.y == y)
            {
                return true;
            }
        }
        return false;
    }
    public static MapGenChunk GetChunk(int x, int y)
    {
        foreach (MapGenChunk chunk in savedChunks)
        {
            if(chunk.x == x && chunk.y == y)
            {
                return chunk;
            }
        }
        return null;
    }

    public static void Generate(int x ,int y, MapGenerator mapGen)
    {
        if (!DoesChunkExist(x, y))
        {
            MapGenChunk chunk = new MapGenChunk(mapGen);
            chunk.x = x;
            chunk.y = y;
            chunk.GenerateMapChunk();
            savedChunks.Add(chunk);
        }
        else
        {
            GetChunk(x, y).GenerateMapChunk(); // reloads the chunk
        }   
    }

    public static void Show(int x, int y)
    {
        GetChunk(x, y).chunkObject.SetActive(true);
    }
    public static void Hide(int x, int y)
    {
        GetChunk(x, y).chunkObject.SetActive(false);
    }
}



