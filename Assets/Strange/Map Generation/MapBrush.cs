using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MapBrush: MonoBehaviour
{
    public Color color;
    public float radius = 25;
    public AnimationCurve curve = AnimationCurve.Linear(0,0,1,1);

    GameObject brush;

    public void SetBrush(Vector3 point)
    {
        if (brush == null)
        {
            brush = Instantiate(Resources.Load<GameObject>("Brush"));
        }

        brush.transform.localScale = Vector3.one * (radius / 5);
        Vector3 offset = new Vector3(0, 1, 0);
        brush.transform.position = point + offset;

        brush.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MainTex", CurveToTexture());
    }
    public void DeleteBrush()
    {
        if (brush)
        {
            DestroyImmediate(brush);
        }
    }

    public Texture CurveToTexture()
    {
        int textureSize = 55;
        Texture2D tex = new Texture2D(textureSize, textureSize);
        Vector2 centre = new Vector2(28, 28);

        for (int Y = 0; Y < tex.height; Y++)
        {
            for (int X = 0; X < tex.width; X++)
            {
                float distance = Vector2.Distance(centre, new Vector2(X, Y));
                if (distance > textureSize / 2)
                {
                    tex.SetPixel(X, Y, Color.clear);
                }
                else
                {
                    float percent = (distance / (textureSize / 2));
                    float rgb = 1 - curve.Evaluate(percent);
                    tex.SetPixel(X, Y, new Color(1, 1, 1, rgb));
                }
            }
        }
        tex.Apply();

        return tex;
    }


    public void Manipulate(Vector3 point)
    {
        GameObject mapFolder = GameObject.Find("Map");
        Collider[] colliders = Physics.OverlapSphere(point, radius);
        List<Transform> nearbyChunkTransforms = new List<Transform>();
        foreach (Collider item in colliders)
        {
            if (item.transform.IsChildOf(mapFolder.transform))
            {
                nearbyChunkTransforms.Add(item.transform);
            }
        }

        foreach (Transform chunk in nearbyChunkTransforms)
        {
            Mesh mesh = chunk.GetComponent<MeshFilter>().sharedMesh;
            Vector3[] verts = mesh.vertices;
            Color[] colours = mesh.colors;

            for (int i = 0; i < verts.Length; i++)
            {
                // if within range
                float distance = Vector3.Distance(verts[i] + chunk.position, point);
                if (distance < radius)
                {
                    float percentage = 1 - (distance / radius);
                    Blend(colours, i, percentage);
                }
            }

            mesh.SetColors(colours);

            chunk.GetComponent<MeshCollider>().sharedMesh = mesh;
        }

    }

    private void Blend(Color[] colours, int i, float percentage)
    {
        colours[i] = (color * percentage) + (colours[i] * (1 - percentage));
    }
}