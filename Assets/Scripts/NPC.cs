using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public Item test;

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
        q.rewards.Add(test);

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
        List<QuestObjective> l = new List<QuestObjective> {tq, tq, tq, tq, tq, tq, tq, tq };
        Quest q1 = new Quest("1baby's first quest", "info", l);
        Quest q2 = new Quest("2baby's first quest", "info", new TalkQuest("tell me to go away", d3));
        Quest q3 = new Quest("3baby's first quest", "info", new TalkQuest("tell me to go away", d3));
        Quest q4 = new Quest("4baby's first quest", "info", new TalkQuest("tell me to go away", d3));
        Quest q5 = new Quest("5baby's first quest", "info", new TalkQuest("tell me to go away", d3));

        Quest aq1 = new Quest("1baby's first quest", "info", l);
        Quest aq2 = new Quest("2baby's first quest", "info", new TalkQuest("tell me to go away", d3));
        Quest aq3 = new Quest("3baby's first quest", "info", new TalkQuest("tell me to go away", d3));
        Quest aq4 = new Quest("4baby's first quest", "info", new TalkQuest("tell me to go away", d3));
        Quest aq5 = new Quest("5baby's first quest", "info", new TalkQuest("tell me to go away", d3));

        Quest bq1 = new Quest("1baby's first quest", "info", l);
        Quest bq2 = new Quest("2baby's first quest", "info", new TalkQuest("tell me to go away", d3));
        Quest bq3 = new Quest("3baby's first quest", "info", new TalkQuest("tell me to go away", d3));
        Quest bq4 = new Quest("4baby's first quest", "info", new TalkQuest("tell me to go away", d3));
        Quest bq5 = new Quest("5baby's first quest", "info", new TalkQuest("tell me to go away", d3));

        Quest cq1 = new Quest("1baby's first quest", "info", l);
        Quest cq2 = new Quest("2baby's first quest", "info", new TalkQuest("tell me to go away", d3));
        Quest cq3 = new Quest("3baby's first quest", "info", new TalkQuest("tell me to go away", d3));
        Quest cq4 = new Quest("4baby's first quest", "info", new TalkQuest("tell me to go away", d3));
        Quest cq5 = new Quest("5baby's first quest", "info", new TalkQuest("tell me to go away", d3));

        Quest dq1 = new Quest("1baby's first quest", "info", l);
        Quest dq2 = new Quest("2baby's first quest", "info", new TalkQuest("tell me to go away", d3));
        Quest dq3 = new Quest("3baby's first quest", "info", new TalkQuest("tell me to go away", d3));
        Quest dq4 = new Quest("4baby's first quest", "info", new TalkQuest("tell me to go away", d3));
        Quest dq5 = new Quest("5baby's first quest", "info", new TalkQuest("tell me to go away", d3));


        q1.TriggerQuest();
        q2.TriggerQuest();
        q3.TriggerQuest();
        q4.TriggerQuest();
        q5.TriggerQuest();

        aq1.TriggerQuest();
        aq2.TriggerQuest();
        aq3.TriggerQuest();
        aq4.TriggerQuest();
        aq5.TriggerQuest();

        bq1.TriggerQuest();
        bq2.TriggerQuest();
        bq3.TriggerQuest();
        bq4.TriggerQuest();
        bq5.TriggerQuest();

        cq1.TriggerQuest();
        cq2.TriggerQuest();
        cq3.TriggerQuest();
        cq4.TriggerQuest();
        cq5.TriggerQuest();

        dq1.TriggerQuest();
        dq2.TriggerQuest();
        dq3.TriggerQuest();
        dq4.TriggerQuest();
        dq5.TriggerQuest();

        q2.FailQuest();
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
