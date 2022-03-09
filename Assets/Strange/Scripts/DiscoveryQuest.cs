using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DiscoveryQuest Objective", menuName = "Strange/DiscoveryQuest Objective", order = 5)]
public class DiscoveryQuest : QuestObjective
{
    public Vector3 DiscoveryLocation;
    public float DiscoveryRange;


    public override void QuestTriggered()
    {
        base.QuestTriggered();
        UpdateObjectiveTitle();
        objectiveType = ObjectiveType.DiscoveryQuest;
    }

    public DiscoveryQuest(string pTitle, Vector3 location, float range) : base(pTitle)
    {
        if (location == null)
            StrangeLogger.LogError("the location is null on the DiscoveryQuest '" + pTitle + ". please check the inspector of the entity");
        objectiveType = ObjectiveType.DiscoveryQuest;
        baseTitle = pTitle;

        DiscoveryLocation = location;
        DiscoveryRange = range;

        UpdateObjectiveTitle();// no suffix : set the main title
    }

    public void CheckLocation(Vector3 pos)
    {
        if(Vector3.Distance(pos,DiscoveryLocation) < DiscoveryRange)
        {
            CompleteObjective();
        }
    }

    

}
