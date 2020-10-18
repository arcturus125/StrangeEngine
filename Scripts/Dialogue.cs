// Copyright(c) 2020 arcturus125 & StrangeDevTeam
// Free to use and modify as you please, Not to be published, distributed, licenced or sold without permission from StrangeDevTeam
// Requests for the above to be made here: https://www.reddit.com/r/StrangeDev/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//IMPORT:: if you want to utilise this script attatch this script to a gameobject that is always active in the scene- either the player or camera
public class Dialogue : MonoBehaviour
{
    /// Static
    public static bool isInDialogue = false; // is the user currently in a dialogue with an entitiy?
    public static KeyCode nextKey = KeyCode.F; // the key the user presses to move on to the next dialogue
    public static Dialogue currentDialogue; // a link to the current or last dialogue the user has active
    private static int nextID = 0; // used to increment the ID of the dialogues 

    /// public 
    [HideInInspector]
    public string text; // the contents of the dialogue
    [HideInInspector]
    public Dialogue nextDialogue = null; // the link to the next dialogue if there is one
    [HideInInspector]
    public bool pauseForFrame = true; // stops the program from rushing ahead, when true the program will wait 1 frame before monitoring inputs again
    [HideInInspector]
    public int dialogueID = -1; // used in if statements to check if the user is currently in a specific dialogue
    public Quest triggeredQuest = null; // the quest to be triggered on this dialogue, if there is one

    void LateUpdate()
    {
        //if Dialogue is DialogueChoice // Developer Note: " yes i realise now this is badly written, fow now just go with it and i will rework it at some point later"
        try
        {
            DialogueChoice currentDialogueChoice = (DialogueChoice)currentDialogue;
            if (!currentDialogue.pauseForFrame)
            {
                if (isInDialogue)
                {
                    try
                    {
                        // when user presses 1, run dialogue branch 1 if possible
                        if (Input.GetKeyDown(DialogueChoice.option1))
                        {
                            try
                            {
                                DialogueChoice castedChoice = (DialogueChoice)currentDialogueChoice.nextBranches[0];
                                castedChoice.ShowDialogue();
                            }
                            catch (Exception)
                            {
                                currentDialogueChoice.nextBranches[0].ShowDialogue();
                            }
                        }
                        // when user presses 2, run dialogue branch 2 if possible
                        else if (Input.GetKeyDown(DialogueChoice.option2))
                        {
                            try
                            {
                                DialogueChoice castedChoice = (DialogueChoice)currentDialogueChoice.nextBranches[1];
                                castedChoice.ShowDialogue();
                            }
                            catch (Exception)
                            {
                                currentDialogueChoice.nextBranches[1].ShowDialogue();
                            }
                        }
                        // when user presses 3, run dialogue branch 3 if possible
                        else if (Input.GetKeyDown(DialogueChoice.option3))
                        {
                            try
                            {
                                DialogueChoice castedChoice = (DialogueChoice)currentDialogueChoice.nextBranches[2];
                                castedChoice.ShowDialogue();
                            }
                            catch (Exception)
                            {
                                currentDialogueChoice.nextBranches[2].ShowDialogue();
                            }
                        }
                        // when user presses 4, run dialogue branch 4 if possible
                        else if (Input.GetKeyDown(DialogueChoice.option4))
                        {
                            try
                            {
                                DialogueChoice castedChoice = (DialogueChoice)currentDialogueChoice.nextBranches[3];
                                castedChoice.ShowDialogue();
                            }
                            catch (Exception)
                            {
                                currentDialogueChoice.nextBranches[3].ShowDialogue();
                            };
                        }
                        // when user presses 5, run dialogue branch 5 if possible
                        else if (Input.GetKeyDown(DialogueChoice.option5))
                        {
                            try
                            {
                                DialogueChoice castedChoice = (DialogueChoice)currentDialogueChoice.nextBranches[4];
                                castedChoice.ShowDialogue();
                            }
                            catch (Exception)
                            {
                                currentDialogueChoice.nextBranches[4].ShowDialogue();
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
            {
                currentDialogue.pauseForFrame = false;
            }
        }
        // if Dialogue is not DialogueChoice
        catch (Exception)
        {
            try
            {
                if (!currentDialogue.pauseForFrame)
                {
                    if (isInDialogue)
                    {
                        // when the 'next key' (default "F") is pressed, show the next dialogue. or hide the window if there is no next dialogue
                        if (Input.GetKeyDown(nextKey))
                        {
                            try
                            {
                                currentDialogue.nextDialogue.ShowDialogue();
                            }
                            catch (Exception)
                            {
                                currentDialogue.HideDialogue();
                            }
                        }
                    }
                }
                else
                {
                    currentDialogue.pauseForFrame = false;
                }
            }
            catch (Exception)
            { }
        }


    }


    /// <summary>
    /// create a dialogue. After running the window will close.
    /// </summary>
    /// <param name="dialogueText">the dialogue text</param>
    public Dialogue(string dialogueText)
    {
        text = dialogueText;
        dialogueID = nextID;
        nextID++;
    }
    /// <summary>
    /// create a dialogue which will trigger a quest when run. After running the window will close.
    /// </summary>
    /// <param name="dialogueText"> the dialogue text</param>
    /// <param name="QuestLinkedToDialogue"> the quest to give the player once this dialogue is run</param>
    public Dialogue(string dialogueText, Quest QuestLinkedToDialogue)
    {
        text = dialogueText;
        triggeredQuest = QuestLinkedToDialogue;
        dialogueID = nextID;
        nextID++;
    }
    /// <summary>
    /// create a dialogue. after running the linked dialogue will show.
    /// </summary>
    /// <param name="dialogueText">the dialogue text</param>
    /// <param name="linkedDialogue"> the dialogue to run after this one ends</param>
    public Dialogue(string dialogueText, Dialogue linkedDialogue)
    {
        text = dialogueText;
        nextDialogue = linkedDialogue;
        dialogueID = nextID;
        nextID++;
    }
    /// <summary>
    /// create a dialogue which will trigger a quest when run. after running the linked dialogue will show.
    /// </summary>
    /// <param name="dialogueText">the dialogue text</param>
    /// <param name="linkedDialogue"> the dialogue to run after this one ends</param>
    /// <param name="QuestLinkedToDialogue"> the quest to give the player once this dialogue is run</param>
    public Dialogue(string dialogueText, Dialogue linkedDialogue, Quest QuestLinkedToDialogue)
    {
        text = dialogueText;
        nextDialogue = linkedDialogue;
        triggeredQuest = QuestLinkedToDialogue;
        dialogueID = nextID;
        nextID++;
    }

    //Show dialogue on screen, trigger quest if there is one
    public void ShowDialogue()
    {
        currentDialogue = this;
        UIController.ShowDialogueBox(text);
        UIController.HideDialogueChoices();
        isInDialogue = true;
        pauseForFrame = true;
        triggerQuest();
        CheckForQuestDialogue();
    }
    //hide dialogue from screen
    public void HideDialogue()
    {
        UIController.HideDialogueBox();
        isInDialogue = false;
    }

    protected void triggerQuest()
    {
        if (triggeredQuest != null)
        {
            Quest.ActiveQuest = triggeredQuest;// TODO: add this into quest log (once implemented) instead of setting it to active
            Quest.ActiveQuest.started = true;
            Debug.Log("Quest triggered: " + triggeredQuest.title + "(set to active)");
        }
    }

    //check if the player is currently in a dialogue which is part of a quest
    public void CheckForQuestDialogue()
    {
        //index through all quest objectives
        for(int i = 0; i< Quest.ActiveQuest.objectives.Count ; i++ )
        {
            // if they can be converted to a talkQuest
            TalkQuest activeTalkQuest = Quest.convertToTalkQuest(Quest.ActiveQuest.objectives[i]);
            if( activeTalkQuest !=null)
            {
                // and the current dialogue ID matches the one they need to ru nto complete the quest
                if(activeTalkQuest.questedDialogue.dialogueID == currentDialogue.dialogueID)
                {
                    // complete the quest
                    Debug.Log("Quest match!");
                    activeTalkQuest.QuestedDialogueRun();
                }
            }
        }
    }
}
public class DialogueChoice : Dialogue
{

    public static KeyCode option1 = KeyCode.Alpha1;
    public static KeyCode option2 = KeyCode.Alpha2;
    public static KeyCode option3 = KeyCode.Alpha3;
    public static KeyCode option4 = KeyCode.Alpha4;
    public static KeyCode option5 = KeyCode.Alpha5;


    [HideInInspector]
    public string[] choices;
    [HideInInspector]
    public Dialogue[] nextBranches;

    /// <summary>
    /// create a dialogue where the user may respond a number of ways
    /// </summary>
    /// <param name="dialogueText">the dialogue text</param>
    /// <param name="dialogueChoices">an array of dialogue options the user may choose, written as a string</param>
    /// <param name="nextDialogueBranches">an array of dialogues to be run after the respective dialogueChoice is selected by the player</param>
    public DialogueChoice( string dialogueText, string[] dialogueChoices , Dialogue[] nextDialogueBranches) : base(dialogueText)
    {
        choices = dialogueChoices;
        nextBranches = nextDialogueBranches;
    }


    /// <summary>
    /// run the dialogue (only works for DialogueChoice, not Dialogue)
    /// </summary>
    new public void ShowDialogue()
    {
        currentDialogue = this;
        UIController.ShowDialogueBox(text);
        isInDialogue = true;
        pauseForFrame = true;
        string choiceText = "";

        for(int i = 0; i< choices.Length; i++)
        {
            choiceText = choiceText + (i+1) + " -  "+ choices[i] + "\n";
        }

        triggerQuest();


        UIController.ShowDialogueChoices(choiceText);
    }
}
