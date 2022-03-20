
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VertexManipulator : MonoBehaviour
{

    public enum BrushType
    {
        Flatten,
        Add,
        Subtract
    };
    public BrushType brushType = BrushType.Flatten;


    public float radius = 25;
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    [Range(0.01f, 1f)]
    public float brushStrength = 0.2f;

    GameObject brush;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

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
        // get x and z position of this object
        // translate that position into NoiseSpace
        // access all points witin radius
        // find difference between points' Y value and this object's y value
        // take a percentage of the difference, the percentage is based on a spline
        // add this difference to the points

        //Vector2 XZ = new Vector2(this.transform.position.x, this.transform.position.z);
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

            for (int i = 0; i < verts.Length; i++)
            {
                // if within range
                float distance = Vector3.Distance(verts[i] + chunk.position, point);
                //Debug.Log(distance);
                if (distance < radius)
                {
                    float percentage = 1 - (distance / radius);

                    if (brushType == BrushType.Flatten)
                        Flatten(point, verts, i, percentage);
                    else if (brushType == BrushType.Add)
                        Add(point, verts, i, percentage);
                    else if (brushType == BrushType.Subtract)
                        Subtract(point, verts, i, percentage);
                }
            }

            mesh.SetVertices(verts);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            chunk.GetComponent<MeshCollider>().sharedMesh = mesh;
            chunk.GetComponent<ChunkComponent>().RecalculateColours();
        }

    }

    private void Flatten(Vector3 point, Vector3[] verts, int i, float percentage)
    {
        float difference = point.y - verts[i].y;

        float adjustment = difference * curve.Evaluate(percentage);

        verts[i] = verts[i] + new Vector3(0, adjustment, 0);
    }


    private void Add(Vector3 point, Vector3[] verts, int i, float percentage)
    {
        verts[i] = verts[i] + new Vector3(0, brushStrength, 0);
    }

    private void Subtract(Vector3 point, Vector3[] verts, int i, float percentage)
    {
        verts[i] = verts[i] - new Vector3(0, brushStrength, 0);
    }
}