using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DialogueTree : ScriptableObject
{
    public NodeSaveState[] nodes;
    public EdgeSaveState[] connections;
}


[Serializable]
public class NodeSaveState
{
    public Rect nodePos; // position of the node
    public bool isEntryPoint; // is this node the entrypoint?
    public string guid; // the ID of this node
    public string dialogueText; // the Text/Title of this node
    public string[] choices; // choices contained within this node
}

[Serializable]
public class EdgeSaveState
{
    public string inputID;
    public string portName;
    public string outputID;
}
