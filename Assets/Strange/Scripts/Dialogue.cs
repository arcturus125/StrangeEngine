using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue
{
    public bool isDialogueChoice = false;

    public Dialogue previousDialogue = null;
    public Dialogue nextDialogue = null;

    public string text;

    // Create a basic dialogue with no linked dialogues
    public Dialogue(string dialogueText)
    {
        text = dialogueText;
    }

    // create a dialogue with a linked dialogue. once the chat key is pressed the linked dialogue will be played
    public Dialogue(string dialogueText, Dialogue linkedDialogue)
    {
        text = dialogueText;
        nextDialogue = linkedDialogue;
        if(linkedDialogue.previousDialogue == null)
            linkedDialogue.previousDialogue = this;
        else
        {
            Debug.LogWarning("STRANGE ERROR: User attempting to link  a dialogue that is already linked");
        }
    }

    public void Play()
    {
        StrangeChatSystem.Singleton.ShowDialogue(this);
    }
}
public class DialogueChoice : Dialogue
{

    // common index used: choice [1] will run branch [1]
    public string[]   choices;
    public Dialogue[] branches;

    public DialogueChoice(string dialogueText, string[] dialogueChoices, Dialogue[] dialogueBranches) : base(dialogueText)
    {
        choices  = dialogueChoices;
        branches = dialogueBranches;
    }

}
