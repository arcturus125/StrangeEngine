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
        //NodeSaveState firstNode = Array.Find(nodes, node => node.guid == entrypoint.ports[0].inputID);
        //Dialogue d = firstNode.ToDialogue(this);
        //d.Play();

        Dialogue[] savedDialogues = Deserialize();
        Dialogue firstDialogue = Array.Find(savedDialogues, dialogue => dialogue.GUID == entrypoint.ports[0].inputID);
        firstDialogue.Play();
    }
    public NodeSaveState findGuid(string guid)
    {
        return Array.Find(nodes, node => node.guid == guid);
    }


    //#####################################


    public Dialogue[] Deserialize()
    {
        /* Dialogue[] saved dialogues
         * for each node in dialogue tree
         *    create a dialogue / DialogueChoice depending on number of ports in the node
         *    make sure the GUID if the dialogue is set to the GUID of the node
         *    add dialogue to array.
         * 
         * 
         * foreach dialogue in the array.
         *     if dialogue is dialogueChoice
         *         foreach port in the dialogue
         *             search through Dialogue array for a dialogue with a matching GUID as the port ID.
         *             when found, add this dialogue to the dialogueChoice's next values
         *     else dialogue is not dialogueChoice
         *         search through Dialogue array for a dialogue with a matching GUID as the port ID.
         *         when found, add this dialogue to the dialogues next dialogue
         */

        Dialogue[] savedDialogues = new Dialogue[nodes.Length-1];
        int i = 0;
        foreach(NodeSaveState node in nodes)
        {
            if (node.isEntryPoint) continue;

            int numberOfPorts = node.ports.Length;
            if(numberOfPorts < 2)
            {
                if (node.triggeredQuest)
                    savedDialogues[i] = new Dialogue(node.dialogueText, node.triggeredQuest);
                else
                    savedDialogues[i] = new Dialogue(node.dialogueText);

                savedDialogues[i].GUID = node.guid;
                savedDialogues[i].allowMultipleChoices = node.allowloops;
            }
            else
            {
                if (node.triggeredQuest)
                    savedDialogues[i] = new DialogueChoice(node.dialogueText, new string[node.ports.Length], new Dialogue[node.ports.Length], node.triggeredQuest);
                else
                    savedDialogues[i] = new DialogueChoice(node.dialogueText, new string[node.ports.Length], new Dialogue[node.ports.Length]);
                savedDialogues[i].GUID = node.guid;
                savedDialogues[i].allowMultipleChoices = node.allowloops;
            }
            i++;
        }

        //foreach(Dialogue dialogue in savedDialogues)
        for (int j = 0; j < savedDialogues.Length; j++)
        {
            NodeSaveState savestate = Array.Find(nodes, node => node.guid == savedDialogues[j].GUID);
            if(savedDialogues[j].isDialogueChoice)
            {
                //DialogueChoice dialogueChoice = savedDialogues[j] as DialogueChoice;
                int portIndex = 0;
                foreach(DialoguePort port in savestate.ports)
                {
                    Dialogue next = Array.Find(savedDialogues, dialogue => dialogue.GUID == port.inputID);
                    (savedDialogues[j] as DialogueChoice).choices[portIndex] = port.portName;
                    (savedDialogues[j] as DialogueChoice).branches[portIndex] = next;
                    next.previousDialogue = savedDialogues[j];

                    portIndex++;
                }
            }
            else
            {
                if (savestate.ports.Length > 0)
                {
                    Dialogue next = Array.Find(savedDialogues, dialogue => dialogue.GUID == savestate.ports[0].inputID);
                    savedDialogues[j].nextDialogue = next;
                    next.previousDialogue = savedDialogues[j];

                }
            }
        }
        return savedDialogues;
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

    public bool allowloops;


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
