using System.Collections.Generic;
using UnityEngine;

public class ScriptedNPC : MonoBehaviour
{

    public Quest q;
    Dialogue d1;

    void Start()
    {
        Dialogue d4 = new Dialogue("Would you be able to kill those foxes over there?");
        Dialogue d3 = new Dialogue("thank you kind sir", d4);

        Dialogue d2 = new Dialogue("Fine, be on your way");

        List<Dialogue> dList = new List<Dialogue> { d2, d3 };
        List<string> replies = new List<string> { "No i am busy", "Sure , whats up?" };

        d1 = new DialogueChoice("Hello there, will you help mw with my sidequest?", replies.ToArray(), dList.ToArray(),q);


    }


    public void Use()
    {
        d1.Play();



    }



    // boiler plate code
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
}
