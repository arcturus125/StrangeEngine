using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Renderer renderer;

    public int width;
    public int height;
    public float scaleY;
    public float scale;

    public bool autoUpdate = false;

    [System.Serializable]
    public struct Octave
    {
        public float[,] noiseMap;
        [Range(.0f,.1f)]
        public float amplitude;
        public float frequency;
    }
    public Octave[] octaves;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateMap()
    {
        // generate all the octaves
        for(int i = 0; i < octaves.Length; i ++)
        {
            octaves[i].noiseMap = Noise.GenerateNoiseMap(width, height, scale, octaves[i].amplitude, octaves[i].frequency);
        }

        // merge all the octaves
        float[,] mergedOctaves = MergeOctaves();
        // normalise the octaves
        float[,] normalisedOctaves = Noise.NormalizeNoiseMap(mergedOctaves);
        // display the map
        DrawPoints(normalisedOctaves);
        //renderer.sharedMaterial.mainTexture = Noise.GenerateHeightMap(normalisedOctaves);
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

    Vector3[] verts;
    void DrawPoints(float[,] noiseMap)
    {
        verts = new Vector3[noiseMap.GetLength(0) * noiseMap.GetLength(1)];

        for (int i = 0, y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                verts[i] = new Vector3(x*scale, noiseMap[x,y]*scaleY, y*scale);
                i++;
            }
        }

        
    }
    private void OnDrawGizmos()
    {
        if (verts == null)
            return;

        for (int i = 0; i < verts.Length; i++)
        {
            Gizmos.DrawSphere(verts[i], 0.1f);
        }
    }

}
