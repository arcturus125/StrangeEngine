using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactible : MonoBehaviour
{

    public virtual void Start()
    {
    }

    public virtual void Update()
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

    public virtual void OnNearby()
    {
        //add this gameobject to a list of interactible objects
        InteractionEngine.AddInteractible(this.gameObject);
    }

    public virtual void NoLongerNearby()
    {
        // remove this objecct from the list of interactibles
        InteractionEngine.RemoveInteractible(this.gameObject);
    }

    public virtual void WhileNearby()
    {

    }
    public virtual void Use()
    {

    }
}
