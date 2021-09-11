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

    [Header("Drag the player into here so that quests can access the players inventory to give rewards")]
    public StrangeInventory playerInventory;

    void Start() 
    {
        singleton = this;
    }

    void Update() {}
}
