// Copyright(c) 2020 arcturus125 & StrangeDevTeam
// Free to use and modify as you please, Not to be published, distributed, licenced or sold without permission from StrangeDevTeam
// Requests for the above to be made here: https://www.reddit.com/r/StrangeDev/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

/// <summary>
///     Developer Note:
///         "the only reason i included this in the repository is so that the project isnt littered with errors.
///             feel free to delete this class and write your own.
///             
///             if you are using this class, i'd advise managing all your UI panels in this class assuming you make more.
/// </summary>

public class UIController : MonoBehaviour
{
    public static GameObject InteractionPanel;
    public static GameObject DialoguePanel;
    public static GameObject DialogueChoices;
    public static GameObject QuestHelperPanel;


    public static bool isCursorVisible = true; //when true, the user can use mouse to navigate menus without rotating the camera or player in game

    
    void Start()
    {
        ToggleMenus(); //default to cursor invisible on start

        InteractionPanel = GameObject.Find("InteractionPanel");
        InteractionPanel.SetActive(false);

        DialogueChoices = GameObject.Find("DialogueChoices");
        DialogueChoices.SetActive(false);

        DialoguePanel = GameObject.Find("DialoguePanel");
        DialoguePanel.SetActive(false);

        QuestHelperPanel = GameObject.Find("QuestHelperPanel");
        QuestHelperPanel.SetActive(false);

        ShowQuestHelper();
    }
    

    void Update()
    {
        //when escape is pressed, toggle whether mouse moved charcter or cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenus();
        }
    }

    /// <summary>
    /// toggle between Locking cursor for player rotation, and unlocking cursor for navigating menus
    /// </summary>
    public static void ToggleMenus()
    {
        isCursorVisible = !isCursorVisible;          
        if (isCursorVisible)                          
            UnityEngine.Cursor.lockState = CursorLockMode.None;  
        else                                         
            UnityEngine.Cursor.lockState = CursorLockMode.Locked; 
        UnityEngine.Cursor.visible = isCursorVisible;           

    }

    /// <summary>
    /// Show "press f to interact"
    /// </summary>
    public static void ShowInteractionTooltip()
    {
        InteractionPanel.SetActive(true);
    }
    /// <summary>
    /// hide "press f to interact"
    /// </summary>
    public static void HideInteractionTooltip()
    {
        InteractionPanel.SetActive(false);
    }


    public static void ShowDialogueBox(string text)
    {
        Text dialogueText = DialoguePanel.GetComponentInChildren<Text>();
        dialogueText.text = text;
        DialoguePanel.SetActive(true);
    }
    public static void HideDialogueBox()
    {
        DialoguePanel.SetActive(false);
    }

    public static void ShowDialogueChoices(string text)
    {
        Text dialogueChoiceText = DialogueChoices.GetComponentInChildren<Text>();
        dialogueChoiceText.text = text;
        DialogueChoices.SetActive(true);
    }
    public static void HideDialogueChoices()
    {
        DialogueChoices.SetActive(false);
    }

    public static void ShowQuestHelper()
    {
        QuestHelperPanel.SetActive(true);
    }
}
