using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        // draw the default inspector and if anything was changed, run this if statement
        if(DrawDefaultInspector())
        {
            if (mapGen.autoUpdate) mapGen.GenerateMap();
        }

        // create a button and when it is pressed, run this if statement
        if(GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }

        // create a button and when it is pressed, run this if statement
        if (GUILayout.Button("Randomise Octaves"))
        {
            mapGen.RandomiseOctaves();
        }

    }
}
