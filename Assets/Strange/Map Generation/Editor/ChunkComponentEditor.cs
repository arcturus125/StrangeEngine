using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChunkComponent))]
public class ChunkComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ChunkComponent chunk = (ChunkComponent)target;

        DrawDefaultInspector();

        // create a button and when it is pressed, run this if statement
        if (GUILayout.Button("Get Png"))
        {
            chunk.GetImage();
        }
        if (GUILayout.Button("Set Png"))
        {
            chunk.SetImage();
        }
        if (GUILayout.Button("Reset"))
        {
            chunk.ResetTerrain();
        }
        if (GUILayout.Button("updateColours"))
        {
            chunk.RecalculateColours();
        }

    }
}
