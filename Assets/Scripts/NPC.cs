using UnityEngine;

public class NPC : MonoBehaviour
{
    public Transform interactionButtonLocation;



    Dialogue d;

    // Start is called before the first frame update
    void Start()
    {
        Dialogue d2 = new Dialogue("dialogue two");
        d = new Dialogue("dialogue one", d2);

    }

    void Update()
    {
        if (InteractionEngine.ClosestInteractible) // error avoidance
        {
            // if this object is is the closest interactible
            if (this.gameObject == InteractionEngine.ClosestInteractible)
            {
                //display the  UI interaction button
                InteractionEngine.singleton.interactionButtonLocation = this.interactionButtonLocation;
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
        if (!StrangeChatSystem.isInDialogue) d.Play();
    }
}
