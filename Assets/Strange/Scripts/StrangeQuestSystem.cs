// Copyright(c) 2020 Arcturus125 & StrangeDevTeam
// Free to use and modify as you please, Not to be published, distributed, licenced or sold without permission from StrangeDevTeam
// Requests for the above to be made here: https://www.reddit.com/r/StrangeDev/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrangeQuestSystem : MonoBehaviour
{
    public static StrangeQuestSystem singleton;
    public static Quest trackedQuest = null; // the quest the user is currently tracking on the quest helper
    public static List<Quest> activeQuests = new List<Quest>(); // all the quests that the player has been given
    public static List<Quest> completedQuests = new List<Quest>(); // all the quests that the player has completed
    public static List<Quest> failedQuests = new List<Quest>(); // all the quests that the player has failed



    [Header("access to the player's inventory is required to give quest rewards")]
    public StrangeInventory playerInventory;
    [Header("access to the player's position to track discovery quests")]
    public Transform playerTransform;


    [Header("Drag all UI scripts Here")]
    public GameObject[] QuestUI;

    void Start() 
    {
        singleton = this;

        if (playerInventory == null)
            StrangeLogger.LogError("StrangeQuestSystem has no access to the players inventory, this will cause errors when you try to turn in quests... please drag StrangePlayer into the 'PlayerInventory' box in the inspector");
        if (playerTransform == null)
            StrangeLogger.LogError("StrangeQuestSystem has no access to the players position, this will cause errors when you use DiscoveryQuests... please drag StrangePlayer into the 'PlayerTransform' box in the inspector");

        // every 5 seconds, call CheckForDiscoveryQuestCompletion()
        InvokeRepeating("CheckForDiscoveryQuestCompletion", 5, 5);
    }

    public static void SetTrackedQuest(Quest q)
    {
        trackedQuest = q;
    }

    void Update() 
    {
    }

    public void CheckForFetchQuestCompletion()
    {
        foreach (Quest activeQuest in StrangeQuestSystem.activeQuests)
        {
            foreach (QuestObjective qo in activeQuest.objectives)
            {
                if (qo.objectiveType == QuestObjective.ObjectiveType.FetchQuest)
                {
                    FetchQuest castedQuestObjective = (FetchQuest)qo;
                    Item itemToCheckFor = castedQuestObjective.questedItem;

                    int numberOfItemsCollected = playerInventory.GetNumberOfItems(itemToCheckFor);
                    castedQuestObjective.ItemCollected(numberOfItemsCollected);
                }
            }
        }
    }

    public void CheckForDiscoveryQuestCompletion()
    {
        foreach (Quest activeQuest in StrangeQuestSystem.activeQuests)
        {
            foreach (QuestObjective qo in activeQuest.objectives)
            {
                if (qo.objectiveType == QuestObjective.ObjectiveType.DiscoveryQuest)
                {
                    DiscoveryQuest castedQuestObjective = (DiscoveryQuest)qo;

                    castedQuestObjective.CheckLocation(playerTransform.position);
                }
            }
        }
    }
}
