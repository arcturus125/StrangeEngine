using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Renderer renderer;

    public int width;
    public int height;
    public float scale;
    public bool autoUpdate = false;


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
        float[,] noiseMap = Noise.GenerateNoiseMap(width, height, scale);
        renderer.sharedMaterial.mainTexture = Noise.GenerateHeightMap(noiseMap);
    }

}
