using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DialogueTree : ScriptableObject
{
    public NodeSaveState[] nodes;

    public void Play()
    {
        NodeSaveState entrypoint =  Array.Find(nodes, node => node.isEntryPoint == true);
        NodeSaveState firstNode = Array.Find(nodes, node => node.guid == entrypoint.ports[0].inputID);
        Dialogue d = firstNode.ToDialogue(this);
        d.Play();
    }
    public NodeSaveState findGuid(string guid)
    {
        return Array.Find(nodes, node => node.guid == guid);
    }
}

[Serializable]
public class NodeSaveState
{
    public Rect nodePos; // position of the node
    public bool isEntryPoint; // is this node the entrypoint?
    public string guid; // the ID of this node
    public string dialogueText; // the Text/Title of this node
    public DialoguePort[] ports;
    public Quest triggeredQuest;


    /// <summary>
    /// translates the serialied dialigue tree data into a object orianted data structure that is compatible with the API
    /// </summary>
    /// <param name="tree"> a reference to the parent dialogue tree is needed so we can search through the tree to find other nodes</param>
    /// <returns> a Dialogue object that can be executed</returns>
    public Dialogue ToDialogue(DialogueTree tree)
    {
        List<string> replies = new List<string>();
        List<Dialogue> linkedDialogues = new List<Dialogue>();
        for (int i = 0; i < ports.Length; i++)
        {
            replies.Add(ports[i].portName);
            linkedDialogues.Add(tree.findGuid(ports[i].inputID).ToDialogue(tree));
        }


        if (ports.Length == 0)
        {
            Dialogue d;
            if(triggeredQuest)
                d = new Dialogue(dialogueText , triggeredQuest);
            else
                d = new Dialogue(dialogueText);
            d.GUID = guid;
            return d;
        }
        else if (ports.Length == 1)
        {
            Dialogue d;
            if (triggeredQuest)
                d = new Dialogue(dialogueText,linkedDialogues[0], triggeredQuest);
            else
                d = new Dialogue(dialogueText, linkedDialogues[0]);
            d.GUID = guid;
            return d;
        }
        else
        {
            DialogueChoice d;
            if (triggeredQuest)
                d = new DialogueChoice(dialogueText, replies.ToArray(), linkedDialogues.ToArray(), triggeredQuest);
            else
                d = new DialogueChoice(dialogueText, replies.ToArray() , linkedDialogues.ToArray());
            d.GUID = guid;
            return d;
        }
    }
    public void Play(Dialogue d)
    {
        d.Play();
    }
}

[Serializable]
public class DialoguePort
{
    public string inputID;
    public string portName;
}
