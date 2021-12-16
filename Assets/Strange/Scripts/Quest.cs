using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : ScriptableObject
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
              
            }
            UpdateQuestUI();
            

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

        UpdateQuestUI();
    }

    public void UpdateQuestUI()
    {

        foreach(GameObject UI in StrangeQuestSystem.singleton.QuestUI)
        {
            UI.SendMessage("UpdateGUI");
        }
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

            UpdateQuestUI();
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
    {                                                        // instead of casting - which can be very inneficient and typically involves a lot try-catch phrases
        Null,                                                // this uses a variable where a simple if statement is run to determine what type of
        TalkQuest,                                           // quest objective this is
        FetchQuest,                                          //
        KillQuest                                            //
    }                                                        //
    public ObjectiveType objectiveType = ObjectiveType.Null; //

    public bool objectiveComplete = false; // true when this objective is complete
    public string baseTitle = "task title"; // the title of the objective with no added suffixes
    public string title = "task title"; // the title of the objective including suffixes
    public bool showTitle = true; // whether or not the title should show on the UI



    public QuestObjective(string pTitle)
    {
        title = pTitle;
    }

    public void attachParent(Quest parent)
    {
        parentQuest = parent;
        parentQuest.UpdateQuestUI(); // if the title is changed, it cannot be updated until now
    }

    protected void UpdateObjectiveTitle(string newSuffix)
    {
        title = baseTitle + newSuffix;
    }

    protected void CompleteObjective()
    {
        objectiveComplete = true;
        parentQuest.UpdateQuestStatus();
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
        UpdateObjectiveTitle(" [" + quantityPlayerCollected + "/" + questedQuantity + "]"); 

        if(quantityPlayerCollected >= questedQuantity)
        {
            CompleteObjective();
        }

    }
}
public class KillQuest : QuestObjective
{
    public List<Enemy> targetEnemies;
    public int requiredKills;
    public int killCounter;

    // create a killquest with one target
    public KillQuest(string pTitle, Enemy target, int pRequiredkills) : base(pTitle)
    {
        if (target == null)
            StrangeLogger.LogError("the target is null on the killquest '" + pTitle + ". please check the inspector of the entity");
        objectiveType = ObjectiveType.KillQuest;
        targetEnemies = new List<Enemy>();
        targetEnemies.Add(target);

        requiredKills = pRequiredkills;
        killCounter = 0;
        baseTitle = pTitle;
        UpdateObjectiveTitle(" [" + killCounter + "/" + requiredKills + "]");
    }
    // create a killquest with multiple targets
    public KillQuest(string pTitle, List<Enemy> targets, int pRequiredkills) : base(pTitle)
    {
        if (targets == null || targets.Count == 0)
            StrangeLogger.LogError("the target is null on the killquest '" + pTitle + ". please check the inspector of the entity");
        objectiveType = ObjectiveType.KillQuest;
        targetEnemies = targets;

        requiredKills = pRequiredkills;
        killCounter = 0;
        baseTitle = pTitle;
        UpdateObjectiveTitle(" [" + killCounter + "/" + requiredKills + "]");
    }

    // check if this enemy should increment the killquest kill counter
    public void CheckEnemyKill(Enemy enemyType)
    {
        if(targetEnemies.Contains(enemyType))
        {
            killCounter++;
            if(killCounter >= requiredKills)
            {
                CompleteObjective();
            }
            UpdateObjectiveTitle(" [" + killCounter + "/" + requiredKills + "]");
        }
    }
}
