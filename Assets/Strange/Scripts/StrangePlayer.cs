// Copyright(c) 2020 Arcturus125 & StrangeDevTeam
// Free to use and modify as you please, Not to be published, distributed, licenced or sold without permission from StrangeDevTeam
// Requests for the above to be made here: https://www.reddit.com/r/StrangeDev/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrangePlayer : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;
    public float walkSpeed = 1.0f;

    // movement keys, WASD by default
    public static KeyCode walkForward   = KeyCode.W;
    public static KeyCode walkBack      = KeyCode.S;
    public static KeyCode strafeLeft    = KeyCode.A;
    public static KeyCode strafeRight   = KeyCode.D;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        CheckForFetchQuestCompletion(); // TODO: run this once when the quest is given and once every time the player picks up an item
    }

    private void PlayerMovement()
    {
        // start with no force
        Vector3 movementForce = Vector3.zero;

        // if buttons are pressed, accumulate force in that direction - opposite directions cancel out
        if (Input.GetKey(walkForward))
            movementForce += Vector3.forward;
        if (Input.GetKey(walkBack))
            movementForce += Vector3.back;
        if (Input.GetKey(strafeLeft))
            movementForce += Vector3.left;
        if (Input.GetKey(strafeRight))
            movementForce += Vector3.right;

        // normalise movementForce so that moving diagonal does not move the player faster
        Vector3 finalForce = transform.TransformDirection(movementForce.normalized * walkSpeed);
        // move the player
        rb.MovePosition(transform.position + finalForce);
    }

    // because there can be multiple inventories in the scene at once, this code must run on the
    // player to make sure that this runs exclussively for the players inventory and none of the others
    private void CheckForFetchQuestCompletion()
    {
        StrangeInventory playerInventory =  this.gameObject.GetComponent<StrangeInventory>();


        foreach(Quest activeQuest in StrangeQuestSystem.activeQuests)
        {
            foreach(QuestObjective qo in activeQuest.objectives)
            {
                if(qo.objectiveType == QuestObjective.ObjectiveType.FetchQuest)
                {
                    FetchQuest castedQuestObjective = (FetchQuest)qo;
                    Item itemToCheckFor = castedQuestObjective.questedItem;

                    int numberOfItemsCollected = playerInventory.GetNumberOfItems(itemToCheckFor);
                    castedQuestObjective.ItemCollected(numberOfItemsCollected);
                }
            }
        }
    }
}
