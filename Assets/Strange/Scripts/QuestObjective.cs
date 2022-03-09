using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestObjective : ScriptableObject
{
    protected Quest parentQuest = null;

    public enum ObjectiveType                                //
    {                                                        // instead of casting - which can be very inneficient and typically involves a lot try-catch phrases
        Null,                                                // this uses a variable where a simple if statement is run to determine what type of
        TalkQuest,                                           // quest objective this is
        FetchQuest,                                          //
        KillQuest,                                           //
        DiscoveryQuest                                       //
    }                                                        //
    [HideInInspector]
    public ObjectiveType objectiveType = ObjectiveType.Null; //

    public bool objectiveComplete = false; // true when this objective is complete
    public string baseTitle = "task title"; // the title of the objective with no added suffixes
    [HideInInspector]
    public string title = "task title"; // the title of the objective including suffixes
    public bool showTitle = true; // whether or not the title should show on the UI

    

    public QuestObjective(string pTitle)
    {
        title = pTitle;
    }

    public virtual void QuestTriggered()
    {
        objectiveComplete = false;
        parentQuest.UpdateQuestStatus();
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
    protected void UpdateObjectiveTitle()
    {
        title = baseTitle;
    }

    protected void CompleteObjective()
    {
        objectiveComplete = true;
        parentQuest.UpdateQuestStatus();
    }
}
