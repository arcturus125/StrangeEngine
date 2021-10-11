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
        float scale,
        float amplitude,
        Vector2 offset
        )
    {
        // create the grid
        float[,] noiseMap = new float[mapWidth, mapHeight];

        if (scale <= 0) scale = 0.0001f; // this line prevents any divide by 0 errors

        // loop through the grid
        for (int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {

                // perlin noise outputs are identical when inputs are whole numbers
                // this devision fixes that problem
                float sampleX = (x / scale) + offset.x;
                float sampleY = (y / scale) + offset.y;

                // using these new sample variables, generate the noise
                float perlinValue = (Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1.0f) * amplitude;
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

    public static float[,] NormalizeNoiseMap(float[,] noiseMap)
    {
        int mapWidth = noiseMap.GetLength(0);
        int mapHeight = noiseMap.GetLength(1);

        float maxValueFound = float.MinValue;
        float minValueFound = float.MaxValue;

        // first loop through the whole map and find the lowest and highest numbers
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (noiseMap[x, y] < minValueFound) minValueFound = noiseMap[x, y];
                if (noiseMap[x, y] > maxValueFound) maxValueFound = noiseMap[x, y];
            }
        }

        // then loop through the whole map and  find the inverse lerp (the percentage it was between the two)
        float[,] newNoiseMap = new float[mapWidth, mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                newNoiseMap[x, y] = Mathf.InverseLerp(minValueFound, maxValueFound, noiseMap[x, y]);
            }
        }
        return newNoiseMap;
    }
}
