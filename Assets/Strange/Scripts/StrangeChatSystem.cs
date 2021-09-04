using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrangeChatSystem : MonoBehaviour
{
    public static StrangeChatSystem Singleton;

    public KeyCode chatKey = KeyCode.F;

    public static bool isInDialogue = false; // true only when the player has a dialogue on their screen
    public static Dialogue currentDialogue; // holds the instance of dialogue the player is in

    public GameObject chatWindow;


    // Start is called before the first frame update
    void Start()
    {
        Singleton = this;
        chatWindow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentDialogue == null) isInDialogue = false; // by changing this at the start of the frame rather than the end of the frame, this shoudl stop dialogue loops
        if (isInDialogue)
        {
            if(Input.GetKeyDown(chatKey))
            {
                if(currentDialogue.nextDialogue == null)
                {
                    HideDialogue();
                }
                else
                {
                    currentDialogue.nextDialogue.Play();
                }
            }
        }
    }

    public void ShowDialogue(Dialogue dialogue)
    {
        isInDialogue = true;
        chatWindow.SetActive(true);
        chatWindow.GetComponentInChildren<Text>().text = dialogue.text;
        currentDialogue = dialogue;
    }
    void HideDialogue()
    {
        chatWindow.SetActive(false);
        currentDialogue = null;
    }


}
