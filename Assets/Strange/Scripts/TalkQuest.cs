using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TalkQuest Objective", menuName = "Strange/TalkQuest Objective", order = 4)]
public class TalkQuest : QuestObjective
{
    //public Dialogue questedDialogue;
    public string quetedDialogueGUID;

    private void OnEnable()
    {
        objectiveType = ObjectiveType.TalkQuest;
        title = baseTitle;
    }

    public TalkQuest(string pTitle, Dialogue pQuestedDialogue) : base(pTitle)
    {
        objectiveType = ObjectiveType.TalkQuest;
        quetedDialogueGUID = pQuestedDialogue.GUID;

        if (pQuestedDialogue == null)
        {
            StrangeLogger.LogError("The Dialogue passed into TalkQuest '" + title + "' is null... Please check the order of your code and make sure that you create and define the Dialogue before you attach it to the dialogue");
        }
    }

    public void QuestedDialogueRun()
    {
        objectiveComplete = true;
        StrangeLogger.Log("Quested Dialogue run, quest objective '" + title + "' completed for quest: " + parentQuest.title);
        parentQuest.UpdateQuestStatus();
    }
}

