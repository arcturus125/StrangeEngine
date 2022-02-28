using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "KillQuest Objective", menuName = "Strange/KillQuest Objective", order = 4)]
public class KillQuest : QuestObjective
{
    public List<Enemy> targetEnemies;
    public int requiredKills;
    [HideInInspector]
    public int killCounter;

    public override void QuestTriggered()
    {
        objectiveType = ObjectiveType.KillQuest;
        UpdateObjectiveTitle(" [" + killCounter + "/" + requiredKills + "]");
    }

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
        if (targetEnemies.Contains(enemyType))
        {

            killCounter++;
            if (killCounter >= requiredKills)
            {
                CompleteObjective();
            }
            UpdateObjectiveTitle(" [" + killCounter + "/" + requiredKills + "]");
        }
    }
}
