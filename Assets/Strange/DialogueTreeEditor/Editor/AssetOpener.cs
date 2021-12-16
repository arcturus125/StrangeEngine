using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class AssetOpener
{
    [OnOpenAssetAttribute(1)]
    public static bool TestOpen(int instanceID, int line)
    {
        string name = EditorUtility.InstanceIDToObject(instanceID).GetType().ToString();
        if(name == "DialogueTree")
        {
            string filename = EditorUtility.InstanceIDToObject(instanceID).name;
            DialogueGraph dialogueTree =  DialogueGraph.OpenGraphWindowAndReturnInstance();
            dialogueTree.LoadData(filename);

            return true;
        }
        else return false;
    }
}
