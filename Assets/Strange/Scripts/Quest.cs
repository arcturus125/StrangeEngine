﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public bool complete = false; // true when all quest objectives have been completed
    public bool started = false; // true when the quest has been given to the player
    public bool isTurnedIn = false; // true when the quest is turned in

    public string title = "Quest Title";
    public string info = "Quest Info";
    public List<QuestObjective> objectives = new List<QuestObjective>();
    public List<Item> rewards = new List<Item>();

    public Quest(string pTitle, string pInfo, List<QuestObjective> questObjectives)
    {
        complete = false;
        title = pTitle;
        info = pInfo;
        objectives = questObjectives;

        foreach(QuestObjective obj in objectives)
        {
            obj.attachParent(this);
        }

        StrangeLogger.Log("Quest created with mutliple objectives");
    }

    public Quest(string pTitle, string pInfo, QuestObjective questObjectives)
    {
        complete = false;
        title = pTitle;
        info = pInfo;

        List<QuestObjective> tempList = new List<QuestObjective>();
        tempList.Add(questObjectives);
        objectives = tempList;
        foreach (QuestObjective obj in tempList)
        {
            obj.attachParent(this);
        }

        StrangeLogger.Log("Quest created with a single objective");
    }

    public void TriggerQuest()
    {
        if (StrangeQuestSystem.activeQuests.Contains(this))
        {
            StrangeLogger.LogError("user attempting to give player a quest they already have, aborting");
        }
        else
        {
            StrangeLogger.Log("Quest '" + title+"' triggered");
            started = true;
            StrangeQuestSystem.activeQuests.Add(this);
            if(StrangeQuestSystem.trackedQuest == null)
            {
                StrangeQuestSystem.SetTrackedQuest(this);
                // if the user has not added the questHelper to their scene, do not execute the next line
                if(QuestHelper.singleton != null)
                    QuestHelper.singleton.UpdateGUI();
            }

            // if the user has not added the questLog to their scene, do not execute the next line
            if (QuestLog.singleton != null)
                QuestLog.singleton.UpdateGUI();

            // when a quest is triggered, check if the quest contains any FetchQuests and if so, check if the user already has the neccessary items to complete the objective
            StrangeQuestSystem.singleton.CheckForFetchQuestCompletion();
        }
    }

    public void UpdateQuestStatus()
    {
        bool isQuestComplete = true;
        foreach(QuestObjective obj in objectives)
        {
            if(obj.objectiveComplete != true)
            {
                isQuestComplete = false;
                break;
            }
        }
        if(isQuestComplete)
        {
            OnComplete();
        }
        // if the user has not added the questHelper to their scene, do not execute the next line
        if (QuestHelper.singleton != null)
            QuestHelper.singleton.UpdateGUI();

        // if the user has not added the questLog to their scene, do not execute the next line
        if (QuestLog.singleton != null)
            QuestLog.singleton.UpdateGUI();
    }

    private void OnComplete()
    {
        complete = true;
        StrangeLogger.Log("Quest '"+title+"' Complete");

        
    }
    public void FailQuest()
    {
        if (StrangeQuestSystem.activeQuests.Contains(this))
        {
            StrangeQuestSystem.activeQuests.Remove(this);
            StrangeQuestSystem.failedQuests.Add(this);

            // if the user has not added the questLog to their scene, do not execute the next line
            if (QuestLog.singleton != null)
                QuestLog.singleton.UpdateGUI();
        }
        else
        {
            StrangeLogger.LogError("use trying to fail a quest that is not active!");
        }
    }
    public void TurnIn()
    {
        isTurnedIn = true;
        StrangeLogger.Log("Quest '" + title + "' Turned in");
        foreach (Item i in rewards)
        {
            StrangeQuestSystem.singleton.playerInventory.AddItem(i);
        }

        // if the user is tracking this quest when it is turned in, stop tracking the quest (and thus clear the questhelper)
        if(StrangeQuestSystem.trackedQuest == this)
        {
            StrangeQuestSystem.trackedQuest = null;
        }

        // remove the quest from active to completed
        if (StrangeQuestSystem.activeQuests.Contains(this))
        {
            StrangeQuestSystem.activeQuests.Remove(this);
            StrangeQuestSystem.completedQuests.Add(this);
        }
        else
        {
            StrangeLogger.LogError("user trying to complete a quest that is not active!");
        }
    }
}

public class QuestObjective
{
    protected Quest parentQuest = null;

    public enum ObjectiveType                                //
    {                                                        // instead of casting - which can be very inneficient and typically involves slot try-catch phrases
        Null,                                                // this uses a variable where a simple if statement is run to determine what type of
        TalkQuest,                                           // quest objective this is
        FetchQuest                                           //
    }                                                        //
    public ObjectiveType objectiveType = ObjectiveType.Null; //

    public bool objectiveComplete = false; // true when this objective is complete
    public string title = "task title"; // the title of the objective
    public bool showTitle = true; // whether or not the title should show on the UI



    public QuestObjective(string pTitle)
    {
        title = pTitle;
    }

    public void attachParent(Quest parent)
    {
        parentQuest = parent;
    }
}
public class TalkQuest : QuestObjective
{
    public Dialogue questedDialogue;

    public TalkQuest(string pTitle, Dialogue pQuestedDialogue) : base(pTitle)
    {
        objectiveType = ObjectiveType.TalkQuest;
        questedDialogue = pQuestedDialogue;

        if(pQuestedDialogue == null)
        {
            StrangeLogger.LogError("The Dialogue passed into TalkQuest '" + title + "' is null... Please check the order of your code and make sure that you create and define the Dialogue before you attach it to the dialogue");
        }
    }
    public void QuestedDialogueRun()
    {
        objectiveComplete = true;
        StrangeLogger.Log("Quested Dialogue run, quest objective '"+title+"' completed for quest: " + parentQuest.title);
        parentQuest.UpdateQuestStatus();
    }
}
public class FetchQuest : QuestObjective
{
    public string baseTitle;
    public Item questedItem; // the item the player should Fetch
    public int questedQuantity; // the quantity the player should fetch

    public int quantityPlayerCollected; // the quantity the player has fetched

    /// <summary>
    /// note, your title will be overridden slightly: if you set your title as "Collect Sticks" and it will appear as "Collect Sticks [0/5]"
    /// </summary>
    /// <param name="pTitle"></param>
    public FetchQuest(string pTitle, Item pQuestedItem, int pQuestedQuantity) : base(pTitle)
    {
        objectiveType = ObjectiveType.FetchQuest;
        baseTitle = pTitle;
        questedItem = pQuestedItem;
        questedQuantity = pQuestedQuantity;
    }

    // checks the players inventory for the items required by the fetchquest
    // runs once when the quest is triggered and every time an item is picked up
    public void ItemCollected(int numberOfItems)
    {
        quantityPlayerCollected = numberOfItems;
        // update objective title
        title = baseTitle + " [" + quantityPlayerCollected + "/" + questedQuantity + "]"; 

        if(quantityPlayerCollected >= questedQuantity)
        {
            objectiveComplete = true;
        }


        // if the user has not added the questHelper to their scene, do not execute the next line
        if (QuestHelper.singleton != null)
            QuestHelper.singleton.UpdateGUI();

        // if the user has not added the questLog to their scene, do not execute the next line
        if (QuestLog.singleton != null)
            QuestLog.singleton.UpdateGUI();
    }
}