using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrangeChatSystem : MonoBehaviour
{
    public static StrangeChatSystem singleton;
    public static bool isInDialogue = false; // true only when the player has a dialogue on their screen
    public static Dialogue currentDialogue;
    public GameObject[] dialogueUI;

    void Start()
    {
        singleton = this;
    }

    public void HideDialogueWindow()
    {
        foreach (GameObject UI in singleton.dialogueUI)
        {
            UI.SendMessage("HideDialogue", SendMessageOptions.DontRequireReceiver);
        }
    }
}
