using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionEngine : MonoBehaviour
{
    public static InteractionEngine singleton;

    [Header("Drag the StrangePlayer here to be able to detect distances from the player")]
    public Transform playerTransform;
    [Header("")]

    // =========================================
    //      Detecting colliders
    // =========================================
    public float interactionDistance = 7.5f; // the distance in which the user can interact with a UsableObject
    public KeyCode useKey = KeyCode.F; // this is the key the user will press to use/ interact with an object. Default f
    public static List<Collider> previousColliders = new List<Collider>();


    private void Awake()
    {
        singleton = this;
    }
    

    void LateUpdate()
    {

        Collider[] nearbyColliders = Physics.OverlapSphere(playerTransform.position, interactionDistance);  // get every collider within interactionDistance
        List<Collider> copyOfNearbyColliders = ArrayToList(ref nearbyColliders); // make a copy of the nearby colliders that can be manipulated

        calculatePreferredInteractible();

        SetInteractionButtonPosition();



        ///OnNearby()
        //detect when player walks towards a new collider and attempt to run OnNearby() on it
        for (int h = 0; h < previousColliders.Count; h++)
        {
            for (int g = 0; g < copyOfNearbyColliders.Count; g++)
            {
                if (copyOfNearbyColliders[g] == previousColliders[h])
                {
                    copyOfNearbyColliders.Remove(copyOfNearbyColliders[g]);
                }
            }
        }
        for (int f = 0; f < copyOfNearbyColliders.Count; f++)
        {
            copyOfNearbyColliders[f].gameObject.SendMessage("OnNearby", SendMessageOptions.DontRequireReceiver);
        }

        ///NoLongerNearby()
        //detect when the player as walked away from a collider and run NoLongerNearby() on it if possible
        for (int g = 0; g < previousColliders.Count; g++)
        {
            for (int h = 0; h < nearbyColliders.Length; h++)
            {
                if (g >= 0 && g < previousColliders.Count)
                {
                    if (previousColliders[g] == nearbyColliders[h])
                    {
                        previousColliders.Remove(previousColliders[g]);
                    }
                }
            }
        }
        for (int f = 0; f < previousColliders.Count; f++)
        {
            if (previousColliders[f])
                previousColliders[f].gameObject.SendMessage("NoLongerNearby", SendMessageOptions.DontRequireReceiver);
        }

        ///WhileNearby()
        //while a user is nearby a collider, run WhileNearby on the collider if possible
        for (int i = 0; i < nearbyColliders.Length; i++)
        {
            GameObject NearbyObject = nearbyColliders[i].gameObject;
            NearbyObject.SendMessage("WhileNearby", SendMessageOptions.DontRequireReceiver);
        }

        ///Use()
        // while a user is wihtin range of a collider and they haev pressed F. run Use() on it if possible
        if (!StrangeChatSystem.isInDialogue)
        {
            if (Input.GetKeyDown(useKey))
            {
                if (ClosestInteractible)
                    ClosestInteractible.SendMessage("Use", SendMessageOptions.RequireReceiver);
            }
        }

        //at the end of the frame, set the current frame's collider to the previous frame's colliders ready for the next frame
        previousColliders = ArrayToList(ref nearbyColliders);


    }

    // converts a Collider array to a list of Colliders
    List<Collider> ArrayToList(ref Collider[] array) 
    {
        List<Collider> tempList = new List<Collider>();
        foreach (Collider coll in array)
        {
            tempList.Add(coll);
        }
        return tempList;
    }





    // =========================================
    //          managing interactibles
    // =========================================

    public static GameObject ClosestInteractible; // a reference to the interactibel closest to the player
    public static List<GameObject> interactibles = new List<GameObject>(); // a list of all interactibles within range of the player
    public static GameObject interactionSymbol; // a reference to the interaction symbol on the screen (null when no interactibles in range)
    [HideInInspector]
    public Transform interactionButtonLocation = null; //the WORLD location (target) of the interactible button ::set in interactible boilder-plate code

    // add an object to a list of interactibles, the closest interactable within range of the player will have an interaction icon on it
    public static void AddInteractible(GameObject obj)
    {
        interactibles.Add(obj);
    }
    // remove an object from the list of interactibles, meaning the interaction icon will not appear and the player can no longer interact wit this object
    public static void RemoveInteractible(GameObject obj)
    {
        if (ClosestInteractible == obj)
        {
            ClosestInteractible = null;
            Destroy(interactionSymbol);
        }

        interactibles.Remove(obj);
    }
    public void calculatePreferredInteractible()
    {
        float closestDistance = interactionDistance + 0.1f;
        foreach (GameObject go in interactibles)
        {
            float dist = Vector3.Distance(playerTransform.position, go.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                ClosestInteractible = go;
                //Debug.Log("(" + interactibles.Count + ") calculating closest interactible: " + ClosestInteractible.name);
            }
        }
    }

    public void SetInteractionButtonPosition()
    {
        if (ClosestInteractible != null) // if there is an interactible in range
        {
            // get position of the interactible on the screen
            Vector3 UIButtonPosition = Camera.main.WorldToScreenPoint(ClosestInteractible.transform.position);
            // destroy any old interaction buttons if needed
            if (interactionSymbol) Destroy(interactionSymbol);
            // create a new interaction button
            interactionSymbol = Instantiate(Resources.Load<GameObject>("Interaction Button"), this.transform);
            interactionSymbol.transform.position = UIButtonPosition;
        }
    }
}
