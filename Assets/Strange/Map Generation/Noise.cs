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
        int seed,
        float scale,
        int octaves,
        float persistance,
        float lacunarity,
        Vector2 offset
        )
    {
        // create the grid
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random pseudoRandom = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i ++)
        {
            float offsetX = pseudoRandom.Next(-100000, 100000) + offset.x;
            float offsetY = pseudoRandom.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0) scale = 0.0001f; // this line prevents any divide by 0 errors

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;
        // loop through the grid
        for (int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x<mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                // an octave is a layer of a noisemap
                // by layering multiple noisemaps you can generate more realistic and detailed landmasses
                for (int i = 0; i < octaves; i++)
                {
                    // perlin noise outputs are identical when inputs are whole numbers
                    // this devision fixes that problem
                    float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y-halfHeight) / scale * frequency + octaveOffsets[i].y;

                    // using these new sample variables, generate the noise
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    // add the noise to the grid
                    //noiseMap[x, y] = perlinValue;
                    noiseHeight += perlinValue * amplitude;

                    // amplitude (intensity) of each octave decreses
                    amplitude *= persistance;
                    // frequancy increases each octave
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;


                // the noise (for this pixel) is the sum of all the octaves' noise height
                noiseMap[x, y] = noiseHeight;
            }
        }

        // normalise the noisemap so all values are between 0 and 1
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;

    }
}
