using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    /// <summary>
    /// returns a grid of values between 0 and 1
    /// </summary>
    /// <returns></returns>
    public static float[,] GenerateNoiseMap(
        int mapWidth,
        int mapHeight,
        float scale
        )
    {
        // create the grid
        float[,] noiseMap = new float[mapWidth, mapHeight];

        if (scale <= 0) scale = 0.0001f; // this line prevents any divide by 0 errors

        // loop through the grid
        for (int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x<mapWidth; x++)
            {

                // perlin noise outputs are identical when inputs are whole numbers
                // this devision fixes that problem
                float sampleX = x / scale;
                float sampleY = y / scale;

                // using these new sample variables, generate the noise
                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                // add the noise to the grid
                noiseMap[x, y] = perlinValue;
            }
        }

        return noiseMap;

    }

    public static Texture GenerateHeightMap(float[,] noiseMap)
    {
        // create a new texture
        int height = noiseMap.GetLength(0);
        int width = noiseMap.GetLength(1);
        Texture2D texture = new Texture2D(width, height);

        //generate an array of colours
        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = new Color(noiseMap[y, x], noiseMap[y, x], noiseMap[y, x]);
            }
        }
        texture.SetPixels(colorMap);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }
}
