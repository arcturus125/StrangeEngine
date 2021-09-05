using UnityEngine;

public class NPC : MonoBehaviour
{

    DialogueChoice d;
    Dialogue hi;
    // Start is called before the first frame update
    void Start()
    {
        Dialogue d3 = new Dialogue("Okay bye");
        Dialogue d2 = new Dialogue("My name is jeff");

        string[] replies = { "What's your name?", "Go away"};
        Dialogue[] linkedDialogues = {d2, d3 };
        d = new DialogueChoice("Hello there",replies, linkedDialogues);


        hi = new Dialogue("Hi", d);

        //   |====|       |====|      |====|
        //   |  d | --->  | d2 | ---> | d3 |
        //   |====|       |====|      |====|
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
            hi.Play();
        }
    }
}
