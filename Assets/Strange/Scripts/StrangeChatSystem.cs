// Copyright(c) 2020 Arcturus125 & StrangeDevTeam
// Free to use and modify as you please, Not to be published, distributed, licenced or sold without permission from StrangeDevTeam
// Requests for the above to be made here: https://www.reddit.com/r/StrangeDev/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrangeChatSystem : MonoBehaviour
{
    public static StrangeChatSystem Singleton;


    [Header("key mapping")]
    public KeyCode chatKey = KeyCode.F;       //
    public KeyCode choice1 = KeyCode.Alpha1;  //
    public KeyCode choice2 = KeyCode.Alpha2;  //
    public KeyCode choice3 = KeyCode.Alpha3;  // keybinds
    public KeyCode choice4 = KeyCode.Alpha4;  //
    public KeyCode choice5 = KeyCode.Alpha5;  //
    public KeyCode choice6 = KeyCode.Alpha6;  //

    public static bool isInDialogue = false; // true only when the player has a dialogue on their screen
    public static Dialogue currentDialogue; // holds the instance of dialogue the player is in

    public GameObject chatWindow; // the panel that shows the basic chat box
    public GameObject dialogueWheel; // the panel that shows the advanced chat box (with the dialogue wheel)
    public GameObject chatTooltip; // the panel that shows the tooltip to remind the user what key to press when chatting


    public Text[] choiceTexts; // the text of all the players possible replies
    public Button[] choiceButtons; // the buttons of all the choices
    public Text[] choiceButtonKeyConfigs; // the text on top of each choicebutton
    public Text chatButtonTooltipText; // the text on top of the chat tooltip


    // Start is called before the first frame update
    void Start()
    {
        Singleton = this;
        chatWindow.SetActive(false);
        dialogueWheel.SetActive(false);

        chatTooltip.SetActive(false);

        UpdateUIKeyConfigs();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentDialogue == null) isInDialogue = false; // by changing this at the start of the frame rather than the end of the frame, this shoudl stop dialogue loops


        if (isInDialogue)
        {
            // basic dialogue chains
            if (!currentDialogue.isDialogueChoice)
            {
                if (Input.GetKeyDown(chatKey))
                {
                    if (currentDialogue.nextDialogue == null)
                    {
                        HideDialogue();
                    }
                    else
                    {
                        currentDialogue.nextDialogue.Play();
                    }
                }
            }
            // dialogue branches
            else
            {
                if (Input.GetKeyDown(choice1)) ChooseDialogue(1);
                if (Input.GetKeyDown(choice2)) ChooseDialogue(2);
                if (Input.GetKeyDown(choice3)) ChooseDialogue(3);
                if (Input.GetKeyDown(choice4)) ChooseDialogue(4);
                if (Input.GetKeyDown(choice5)) ChooseDialogue(5);
                if (Input.GetKeyDown(choice6)) ChooseDialogue(6);
            }
        }
    }

    public void ShowDialogue(Dialogue dialogue)
    {
        isInDialogue = true;
        chatWindow.SetActive(true);
        chatWindow.GetComponentInChildren<Text>().text = dialogue.text;
        currentDialogue = dialogue;
        if (dialogue.isDialogueChoice)
        {
            ShowDialogueChoices((DialogueChoice)dialogue);
        }
        else
        {
            dialogueWheel.SetActive(false);
            chatTooltip.SetActive(true);
        }

        // complete any quests for running this dialogue
        CheckForQuestedDialogues(dialogue);

        // trigger any quests attached to this dialogue
        if(dialogue.triggeredQuest != null)
            dialogue.triggeredQuest.TriggerQuest();
    }

    // when a dialogue is run, check if the user has a quest where an objective is to run this dialogue, if so, complete that objective in the quest
    private static void CheckForQuestedDialogues(Dialogue dialogue)
    {
        // loop through all quests given to the player
        foreach (Quest q in StrangeQuestSystem.activeQuests)
        {
            // of those quests, loop through all the questObjectives
            foreach (QuestObjective qo in q.objectives)
            {
                // if any of these objectives are TalkQuests, run the follwoing code
                if (qo.objectiveType == QuestObjective.ObjectiveType.TalkQuest)
                {
                    if(((TalkQuest)qo).questedDialogue == dialogue)
                    {
                        ((TalkQuest)qo).QuestedDialogueRun();
                    }
                }
            }
        }
    }
    void HideDialogue()
    {
        chatWindow.SetActive(false);
        currentDialogue = null;
        dialogueWheel.SetActive(false);
        chatTooltip.SetActive(false);
    }


    void ShowDialogueChoices(DialogueChoice dialogue)
    {
        dialogueWheel.SetActive(true);
        chatTooltip.SetActive(false);
        // show choices
        for (int i = 0; i < dialogue.choices.Length;i++)
        {
            // show button
            choiceTexts[i].gameObject.SetActive(true);
            choiceButtons[i].interactable = true;

            // set text
            choiceTexts[i].text = dialogue.choices[i];
        }
        // hide choices not being used
        for(int i = dialogue.choices.Length; i < 6;i++)
        {
            choiceTexts[i].gameObject.SetActive(false);
            choiceButtons[i].interactable = false;
        }
    }

    public void ChooseDialogue(int dialogueNumber)
    {
        if(currentDialogue.isDialogueChoice)
        {
            DialogueChoice dialogueCast = (DialogueChoice)currentDialogue;
            if (dialogueCast.branches[dialogueNumber-1] != null)
            {
                dialogueCast.branches[dialogueNumber - 1].Play();
            }
        }
    }

    // just incase the key configurations have been changed. running this function will make all the tooltips in the chat match the key configs the user has set
    void UpdateUIKeyConfigs()
    {
        chatButtonTooltipText.text = chatKey.ToString();
        choiceButtonKeyConfigs[0].text = choice1.ToString().Replace("Alpha", "");
        choiceButtonKeyConfigs[1].text = choice2.ToString().Replace("Alpha", "");
        choiceButtonKeyConfigs[2].text = choice3.ToString().Replace("Alpha", "");
        choiceButtonKeyConfigs[3].text = choice4.ToString().Replace("Alpha", "");
        choiceButtonKeyConfigs[4].text = choice5.ToString().Replace("Alpha", "");
        choiceButtonKeyConfigs[5].text = choice6.ToString().Replace("Alpha", "");


    }

}
