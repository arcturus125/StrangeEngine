using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class TextFieldPopup : EditorWindow
{
    public string labelText;
    public string textfieldText;
    public string OKButtonText;
    public Func<string, int> functionPointer;


    string filename;

    private void OnGUI()
    {
        EditorGUILayout.LabelField("labelText\n", EditorStyles.wordWrappedLabel);


        string temp = EditorGUILayout.TextField("textfieldText", "");

        // textfield is checked for updated every frame. if there are no updates, it will be blank, this fixes that
        if (temp != "")
            filename = temp;

        if (GUILayout.Button(OKButtonText))
        {
            Debug.Log(filename);
            functionPointer(filename);
            Close();
        }
        if (GUILayout.Button("Cancel"))
        {
            Close();
        }
    }
}
