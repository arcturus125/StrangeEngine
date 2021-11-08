// Copyright(c) 2020 Arcturus125 & StrangeDevTeam
// Free to use and modify as you please, Not to be published, distributed, licenced or sold without permission from StrangeDevTeam
// Requests for the above to be made here: https://www.reddit.com/r/StrangeDev/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue
{

    public struct DialogueMessage
    {
        public Dialogue dialogue;
        public bool firstRun;
    }


    public bool allowMultipleChoices = false;
    public bool isDialogueChoice = false;

    public Dialogue previousDialogue = null;
    public Dialogue nextDialogue = null;
    public Quest triggeredQuest = null;

    public bool firstRun = true; // after this dialogue has been run for the first time, this changes to false

    public string text;

    // Create a basic dialogue with no linked dialogues
    public Dialogue(string dialogueText)
    {
        text = dialogueText;
    }
    // Create a basic dialogue with no linked dialogues and triggers a quest
    public Dialogue(string dialogueText, Quest questToTrigger)
    {
        text = dialogueText;

        QuestCheck(questToTrigger);
    }

    // create a dialogue with a linked dialogue. once the chat key is pressed the linked dialogue will be played
    public Dialogue(string dialogueText, Dialogue linkedDialogue)
    {
        text = dialogueText;
        nextDialogue = linkedDialogue;
        if (linkedDialogue.previousDialogue == null)
            linkedDialogue.previousDialogue = this;
        else
        {
            StrangeLogger.LogError("User attempting to link  a dialogue that is already linked");
        }
    }
    // create a dialogue with a linked dialogue. and triggers a quest
    public Dialogue(string dialogueText, Dialogue linkedDialogue, Quest questToTrigger)
    {
        text = dialogueText;
        nextDialogue = linkedDialogue;
        if (linkedDialogue.previousDialogue == null)
            linkedDialogue.previousDialogue = this;
        else
        {
            StrangeLogger.LogError("User attempting to link  a dialogue that is already linked");
        }
        QuestCheck(questToTrigger);
    }

    protected void QuestCheck(Quest q)
    {
        if(q == null)
        {
            StrangeLogger.LogError("The Quest passed into Dialogue '" + text + "' is null... Please check the order of your code and make sure that you create and define the quest before you attach it to the dialogue");
        }
        triggeredQuest = q;
    }


    public void Play()
    {
        // setup the struct
        DialogueMessage dm = new DialogueMessage();
        dm.dialogue = this;
        dm.firstRun = firstRun;

        foreach(GameObject UI in StrangeChatSystem.singleton.dialogueUI)
        {
            UI.SendMessage("ShowDialogue", dm);
        }
    }
}
public class DialogueChoice : Dialogue
{
    public int lastChoice = -1; // remembers the last choice, -1 is rogue value: means no choice has been made yet

    // common index used: choice [1] will run branch [1]
    public string[]   choices;
    public Dialogue[] branches;

    public DialogueChoice(string dialogueText, string[] dialogueChoices, Dialogue[] dialogueBranches) : base(dialogueText)
    {
        isDialogueChoice = true;

        // warnings to the user if invalid data is inputted
        if ((dialogueChoices.Length > 6) || (dialogueBranches.Length > 6))
            StrangeLogger.LogError("User attempting to create a dialogue choice with more then 6 branches - 6 is the maximum");
        else if ((dialogueChoices.Length == 0) || (dialogueBranches.Length == 0))
            StrangeLogger.LogError("User attempting to create a dialogue choice with zero branches - There must be at least one!");
        if (dialogueChoices.Length != dialogueBranches.Length)
            StrangeLogger.LogError("User attempting to create a dialogue choice with a missmached number of dialogueChoices and dialogueBranches");

        choices  = dialogueChoices;
        branches = dialogueBranches;

        foreach(Dialogue d in dialogueBranches)
        {
            d.previousDialogue = this;
        }
    }

    public DialogueChoice(string dialogueText, string[] dialogueChoices, Dialogue[] dialogueBranches, Quest questToTrigger = null) : base(dialogueText)
    {
        isDialogueChoice = true;

        // warnings to the user if invalid data is inputted
        if ((dialogueChoices.Length > 6) || (dialogueBranches.Length > 6))
            StrangeLogger.LogError("User attempting to create a dialogue choice with more then 6 branches - 6 is the maximum");
        else if ((dialogueChoices.Length == 0) || (dialogueBranches.Length == 0))
            StrangeLogger.LogError("User attempting to create a dialogue choice with zero branches - There must be at least one!");
        if (dialogueChoices.Length != dialogueBranches.Length)
            StrangeLogger.LogError("User attempting to create a dialogue choice with a missmached number of dialogueChoices and dialogueBranches");

        choices = dialogueChoices;
        branches = dialogueBranches;
        QuestCheck(questToTrigger);


        foreach (Dialogue d in dialogueBranches)
        {
            d.previousDialogue = this;
        }
    }

}
