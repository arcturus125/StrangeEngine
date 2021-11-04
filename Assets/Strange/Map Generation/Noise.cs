using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{

    public static float farFromZero = 1000; // perlin noise mirrors in negatives. this should keep all noise generated well away form 0
    /// <summary>
    /// returns a grid of float values between 0 and 1
    /// </summary>
    public static float[,] GenerateNoiseMap(
        int mapWidth,
        int mapHeight,
        float scale,
        float amplitude,
        Vector2 octaveOffset,
        Vector2 globalOffset,
        float zoom
        )
    {
        // create the grid of floats
        float[,] noiseMap = new float[mapWidth, mapHeight];

        if (scale <= 0) scale = 0.0001f; // this line prevents any divide by 0 errors

        // loop through the grid
        for (int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                // perlin noise outputs are identical when inputs are whole numbers
                // this devision fixes that problem
                float sampleX = (globalOffset.x/zoom / scale) + (x/zoom / scale) + octaveOffset.x + farFromZero;
                float sampleY = (globalOffset.y/zoom / scale) + (y/zoom / scale) + octaveOffset.y + farFromZero;

                // using these new sample variables, generate the noise
                float perlinValue = (Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1.0f) * amplitude;
                // add the noise to the grid
                noiseMap[x, y] = perlinValue;
            }
        }
        return noiseMap;
    }

    /// <summary>
    /// turns the grid of floats into a texture
    /// each value in the grid will represent a pixel in the texture
    /// 0 = black pixel
    /// 1 = white pixel
    /// 0.834 = somewhere inbetween
    /// </summary>
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
                // for each cell in the grid, set a pixels colour in the texture
                colorMap[y * width + x] = new Color(noiseMap[y, x], noiseMap[y, x], noiseMap[y, x]);
            }
        }
        // set the texture we just made
        texture.SetPixels(colorMap);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        return texture;
    }

    /// <summary>
    /// takes a noise map that doesn't range between 0 and 1 and makes it range betwen 0 and 1
    /// </summary>
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
                //newNoiseMap[x, y] = Mathf.InverseLerp(minValueFound, maxValueFound, noiseMap[x, y]);
                newNoiseMap[x, y] = Mathf.InverseLerp(MapGenerator.theoretical_min, MapGenerator.theoretical_max, noiseMap[x, y]);
            }
        }
        return newNoiseMap;
    }



}
