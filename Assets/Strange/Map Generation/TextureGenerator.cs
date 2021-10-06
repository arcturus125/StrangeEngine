using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator
{
    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }
    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        // get width and height from the noise map
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        //generate an array of colours
        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //generate colour between black and white where the noisemap values are used to determine colour
                // 0 = black
                // 0.5 = grey
                // 1 = white
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }
        return TextureFromColourMap(colorMap, width, height);
    }
}
