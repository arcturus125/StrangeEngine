using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{

    [Header("This script takes the noiseMap and displays it on the texture")]
    public Renderer textureRenderer;

    public void DrawNoiseMap(float[,] noiseMap)
    {
        // get width and height from the noise map
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        // generate an empty texture
        Texture2D texture = new Texture2D(width, height);

        //generate an array of colours
        Color[] colorMap = new Color[width * height];
        for(int y=0;y<height;y++)
        {
            for(int x=0;x<width;x++)
            {
                //generate colour between black and white where the noisemap values are used to determine colour
                // 0 = black
                // 0.5 = grey
                // 1 = white
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
        // set the texture to the colour array we just made
        texture.SetPixels(colorMap);
        // basically saves our changes ot the texture
        texture.Apply();

        // tell the renderer to use the texture we have made
        textureRenderer.sharedMaterial.mainTexture = texture;
        // set the size of the plane to the size of the texture
        textureRenderer.transform.localScale = new Vector3(width, 1, height);
    }
}
