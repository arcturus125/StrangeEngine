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
        isDialogueChoice = true;

        // warnings to the user if invalid data is inputted
        if ((dialogueChoices.Length > 6) || (dialogueBranches.Length > 6))
            Debug.LogWarning("STRANGE ERROR: User attempting to create a dialogue choice with more then 6 branches - 6 is the maximum");
        else if ((dialogueChoices.Length == 0) || (dialogueBranches.Length == 0))
            Debug.LogWarning("STRANGE ERROR: User attempting to create a dialogue choice with zero branches - There must be at least one!");
        if (dialogueChoices.Length != dialogueBranches.Length)
            Debug.LogWarning("STRANGE ERROR: User attempting to create a dialogue choice with a missmached number of dialogueChoices and dialogueBranches");

        choices  = dialogueChoices;
        branches = dialogueBranches;
    }

}
