using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    const string _MenuTitle = "Dialogue Editor";

    private DialogueGraphView graphView;

    [MenuItem("StangeEngine/ Dialogue Visual Editor")] // adds this function to the unity menu


    // creates a blank unity window with a title
    public static void OpenGraphWindow()
    {
        DialogueGraph window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent(_MenuTitle);
    }

    /// <summary>
    /// creates a blank unity window with a title, then returns the instance of this class
    /// </summary>
    /// <returns></returns>
    public static DialogueGraph OpenGraphWindowAndReturnInstance()
    {
        DialogueGraph window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent(_MenuTitle);
        return window;
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }

    private void ConstructGraphView()
    {

        graphView = new DialogueGraphView
        {
            name = "Dialogue Graph"
        };
        graphView.StretchToParentSize();

        // last
        rootVisualElement.Add(graphView);
    }

    private void GenerateToolbar()
    {
        // generate the toolbar
        Toolbar toolbar = new Toolbar();

        toolbar.Add(new Button(() => SaveBtnCLicked()) { text = "Save" });
        //toolbar.Add(new Button(() => LoadData()) { text = "Load" });
        toolbar.Add(new Button(() => Clear()) { text = "Clear" });




        Label spacer = new Label("|    ");
        toolbar.Add(spacer);



        Button toolbarButton = new Button( () => // create a button, which runs this function
        {                                        //                 |
            graphView.CreateNode("new node");    // <---------------/
        });
        toolbarButton.text = "Insert Dialogue";

        // add the button to the toolbar 
        toolbar.Add(toolbarButton);

        // add the toolbar to the window
        rootVisualElement.Add(toolbar);

    }


    private void Clear()
    {
        graphView.ClearGraph();
    }


    public void LoadData(string filename)
    {
        graphView.Load(filename);
    }
    private void SaveBtnCLicked()
    {
        string path  = EditorUtility.SaveFilePanel("Save Dialogue", $"{Application.dataPath}/Resources/DialogueTrees", "DialogueTree", "asset");

        string filename =  System.IO.Path.GetFileName(path);
        if(filename == "")
            Debug.Log("File was not saved!");
        else
            graphView.Save(path);

    }
}
