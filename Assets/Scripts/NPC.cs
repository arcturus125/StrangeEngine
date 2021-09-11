using UnityEngine;

public class NPC : MonoBehaviour
{

    Dialogue hi;
    Quest q;

    void Start()
    {


        Dialogue d3 = new Dialogue("Okay bye");
        Dialogue d2 = new Dialogue("My name is jeff");


        string[] replies = { "What's your name?", "Go away"};
        Dialogue[] linkedDialogues = {d2, d3 };



        TalkQuest tq = new TalkQuest("ask me my name", d2);
        q = new Quest("Baby's first quest", "this is the firat quest you ever recieve", tq);

        DialogueChoice d = new DialogueChoice("Hello there",replies, linkedDialogues,q);



        hi = new Dialogue("Hi", d);

        //  |====|      |====|       |====|
        //  | hi | ---> |  d | --->  | d2 |
        //  |====|      |====|       |====|
        //                 |         |====|
        //                 |-------> | d3 |
        //                           |====|
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
                q.TurnIn();
            hi.Play();
        }
    }
}
