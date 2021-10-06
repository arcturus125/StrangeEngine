using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{

    [Header("This script takes the noiseMap and displays it on the texture")]
    public Renderer textureRenderer;

    public void DrawTexture(Texture2D texture)
    {
        

        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }
}
