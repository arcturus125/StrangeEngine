using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
using static MapGenChunk;


[ExecuteInEditMode]
public class ChunkComponent : MonoBehaviour
{
    public MapGenChunk dataFile;

    public MeshData data;
    //    {
    //        public Vector3[] vertices;
    //        public int[] indices;
    //        public Vector2[] uvs;
    //        public Color[] vertexColours;
    //    }

    public float[,] noiseMap;
    public int width;
    public int height;
    public float size;
    public Gradient colourGradient;
    public float colourGradientSensitivity;
    private Mesh m;

    public void ResetTerrain()
    {
        Mesh mesh = this.gameObject.GetComponent<MeshFilter>().sharedMesh;
        mesh.Clear();
        mesh.vertices = data.vertices;
        mesh.triangles = data.indices;
        mesh.uv = data.uvs;
        mesh.colors = data.vertexColours;

        mesh.RecalculateNormals();
    }


    public void GetImage()
    {
        Texture2D tex = new Texture2D(width, height);


        int i = 0;
        for (int Y = 0; Y < height; Y++)
        {
            for (int X = 0; X < width; X++)
            {
                tex.SetPixel(X, Y, data.vertexColours[i]);
                i++;
            }
        }
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();

        string path = EditorUtility.SaveFilePanel("Save Chunk Vertex Colours as PNG", "", "chunkVertexColours", "png");

        File.WriteAllBytes(path, bytes);

    }

    public void SetImage()
    {
        string path = EditorUtility.OpenFilePanel("Set Chunk Vertex Colours with PNG", "", "png");
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);

        dataFile.UpdateColours(tex.GetPixels());

    }

    List<float> values;
    Vector3[] vertices;
    public void RecalculateColours()
    {

        m = GetComponent<MeshFilter>().sharedMesh;
        Color[] newVertexColours = new Color[data.vertices.Length];

        float[,] gradientRange = new float[width, height];
        vertices = m.vertices;
        int i = 0;
        for (int Y = 0; Y < height; Y++)
        {
            for (int X = 0; X < width; X++)
            {

                values = new List<float>();



                getValue(X - 1, Y + 1);
                getValue(X    , Y + 1);
                if(X != width-1)
                    getValue(X + 1, Y + 1);

                getValue(X - 1, Y);
                getValue(X    , Y);
                if (X != width - 1)
                    getValue(X + 1, Y);

                getValue(X - 1, Y - 1);
                getValue(X    , Y - 1);
                if (X != width - 1)
                    getValue(X + 1, Y - 1);

                float range = values.Max() - values.Min();
                gradientRange[X, Y] = Mathf.InverseLerp(0, size * 2, range) * colourGradientSensitivity;
                newVertexColours[i] = colourGradient.Evaluate(gradientRange[X, Y]);
                i++;
            }
        }


        m.colors = newVertexColours;




    }

    private void getValue(int X, int Y)
    {
        if (X < 0) return;
        if (X == width) return;
        if (Y < 0) return;
        if (Y == height) return;


        int i = (width * Y) + X;

        if (i >= 0 && i < data.vertices.Length)
        {
            //values.Add(data.vertices[i].y);
            values.Add(vertices[i].y);
        }
    }




    public static bool EditorApplicationQuit = false; //
    static bool WantsToQuit()                         //
    {                                                 //
        EditorApplicationQuit = true;                 // fix for OnDestroy being called at start and end of game
        return true;                                  //
    }                                                 //
    void OnEnable()                                   //
    {                                                 //
        EditorApplication.wantsToQuit += WantsToQuit; //
    }                                                 //


    private void OnDestroy()
    {
        if (Time.frameCount == 0 || EditorApplicationQuit) // without this if statement, onDestroy is also called at the start and end of tha game - this fixes that
            return;

        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(dataFile));
    }
}
