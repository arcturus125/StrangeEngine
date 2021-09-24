using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public Item stick;
    public StrangeInventory playerInventory;

    Dialogue hi;
    Quest q;
    Dialogue quest;

    void Start()
    {
        quest = new Dialogue("Thanks for Completing my Quest");

        Dialogue d3 = new Dialogue("Okay bye");
        Dialogue d2 = new Dialogue("My name is jeff");


        string[] replies = { "What's your name?", "Go away"};
        Dialogue[] linkedDialogues = {d2, d3 };

        q = new Quest("baby's first quest", "info", new TalkQuest("tell me to go away", d3));
        //q.rewards.Add(test);

        DialogueChoice d = new DialogueChoice("Hello there",replies, linkedDialogues,q);
        d.allowMultipleChoices = true;



        hi = new Dialogue("Hi", d);

        //  |====|      |====|       |====|
        //  | hi | ---> |  d | --->  | d2 |
        //  |====|      |====|       |====|
        //                 |         |====|
        //                 |-------> | d3 |
        //                           |====|





        TalkQuest tq = new TalkQuest("tell me to go away", d3);
        FetchQuest fq = new FetchQuest("Collect sticks", stick, 3);
        List<QuestObjective> l = new List<QuestObjective> {tq, fq };
        Quest q1 = new Quest("1baby's first quest", "info test", l);
        Quest q2 = new Quest("2baby's first quest", "info", new TalkQuest("tell me to go away", d3));


        q1.TriggerQuest();


    }

    void Update()
    {
        if (InteractionEngine.ClosestInteractible) // error avoidance
        {
            // if this object is is the closest interactible
            if (this.gameObject == InteractionEngine.ClosestInteractible)
            {
                //display the  UI interaction button
                InteractionEngine.singleton.interactionButtonLocation = this.transform;
            }
        }

        if(Input.GetKeyDown(KeyCode.H))
        {
            playerInventory.AddItem(stick);
        }

    
    }

    public void OnNearby()
    {
        //add this gameobject to a list of interactible objects
        InteractionEngine.AddInteractible(this.gameObject);
    }

    public void NoLongerNearby()
    {
        // remove this objecct from the list of interactibles
        InteractionEngine.RemoveInteractible(this.gameObject);
    }

    public void WhileNearby()
    {

    }
    public void Use()
    {
        if (!StrangeChatSystem.isInDialogue)
        {
            if (q.complete)
            {
                q.TurnIn();
                quest.Play();
            }
            else
                hi.Play();
        }
    }
}
