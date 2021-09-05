﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrangeChatSystem : MonoBehaviour
{
    public static StrangeChatSystem Singleton;


    [Header("key mapping")]
    public KeyCode chatKey = KeyCode.F;
    public KeyCode choice1 = KeyCode.Alpha1;
    public KeyCode choice2 = KeyCode.Alpha2;
    public KeyCode choice3 = KeyCode.Alpha3;
    public KeyCode choice4 = KeyCode.Alpha4;
    public KeyCode choice5 = KeyCode.Alpha5;
    public KeyCode choice6 = KeyCode.Alpha6;

    public static bool isInDialogue = false; // true only when the player has a dialogue on their screen
    public static Dialogue currentDialogue; // holds the instance of dialogue the player is in

    public GameObject chatWindow;
    public GameObject dialogueWheel;
    public Text[] choiceTexts;
    public Button[] choiceButtons;

    public Text chatButtonTooltipText;
    public GameObject chatTooltip;


    // Start is called before the first frame update
    void Start()
    {
        Singleton = this;
        chatWindow.SetActive(false);
        dialogueWheel.SetActive(false);

        Debug.Log(chatKey.ToString());
        chatButtonTooltipText.text = chatKey.ToString();
        chatTooltip.SetActive(false);
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
            ShowDialogueChoices((DialogueChoice)dialogue);
        else
        {
            dialogueWheel.SetActive(false);
            chatTooltip.SetActive(true);
        }
    }
    void HideDialogue()
    {
        chatWindow.SetActive(false);
        currentDialogue = null;
    }


    void ShowDialogueChoices(DialogueChoice dialogue)
    {
        dialogueWheel.SetActive(true);
        // show choices
        for(int i = 0; i < dialogue.choices.Length;i++)
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

}