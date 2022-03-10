using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Quest", menuName = "Strange/Quest", order = 3)]
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
        if(StrangeQuestSystem.singleton == null)
        {
            StrangeLogger.LogError("Cannot trigger Quest Properly. Please drag StrangeQuestSystem into the Scene and update its properties");
            return;
        }

        if (StrangeQuestSystem.activeQuests.Contains(this))
        {
            StrangeLogger.LogWarning("user attempting to give player a quest they already have, aborting");
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

            for (int i = 0; i < objectives.Count; i++)
            {
                objectives[i].QuestTriggered();
            }
            

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
        else
        {
            // reset the quest status if it is givem multiple times
            if (complete) complete = false;
        }

        UpdateQuestUI();
    }

    public void UpdateQuestUI()
    {
        if (StrangeQuestSystem.singleton != null)
        {
            foreach (GameObject UI in StrangeQuestSystem.singleton.QuestUI)
            {
                if (!UI.activeInHierarchy)
                {
                    StrangeLogger.LogWarning($"Cannot Update Quest UI {UI.name} as it was disabled");
                }
                else
                    UI.SendMessage("UpdateGUI");
            }
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


    /// <summary>
    /// when quest objectives are dragged into a quest in the inspector, the objectives still have a null parentQuest. this fixes that :)
    /// </summary>
    private void OnValidate()
    {
        foreach(QuestObjective qo in objectives)
        {
            qo.attachParent(this);
        }
    }
}
