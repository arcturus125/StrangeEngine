using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VertexManipulator))]
public class VertexManipulatorEditor : Editor
{
    bool manipulate = false;
    public override void OnInspectorGUI()
    {
        VertexManipulator manipulator = (VertexManipulator)target;

        DrawDefaultInspector();

        GUILayout.Space(150);
        if (!manipulate)
        {
            // create a button and when it is pressed, run this if statement
            if (GUILayout.Button("Begin editing"))
            {
                manipulate = true;
            }
        }
        else
        {
            // create a button and when it is pressed, run this if statement
            if (GUILayout.Button("Stop editing"))
            {
                manipulate = false;
                manipulator.DeleteBrush();
            }
        }
        GUI.DrawTexture(new Rect(200, 100, 120, 120), manipulator.CurveToTexture());//, ScaleMode.ScaleToFit, true, 10.0F);

    }

    private void OnSceneGUI()
    {
        VertexManipulator manipulator = (VertexManipulator)target;
        if (manipulate)
        {
            Event guiEvent = Event.current;

            if (guiEvent.type == EventType.ScrollWheel)
            {
                Event.current.Use();
                if (guiEvent.delta.y < 0)
                    manipulator.radius += manipulator.radius * 0.1f;
                else
                    manipulator.radius -= manipulator.radius * 0.1f;
            }


            Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(mouseRay, out hitInfo))
            {
                manipulator.SetBrush(hitInfo.point);

                // left click
                //if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)


                // left drag
                if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0)
                {
                    LeftClick(hitInfo);
                }
            }

            // when the user clicks, keep this object selected
            if (guiEvent.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }
        }
        else
            manipulator.DeleteBrush();
    }

    void LeftClick(RaycastHit hitinfo)
    {
        VertexManipulator manipulator = (VertexManipulator)target;
        manipulator.Manipulate(hitinfo.point);
    }
}
