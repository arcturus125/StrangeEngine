// Copyright(c) 2020 arcturus125 & StrangeDevTeam
// Free to use and modify as you please, Not to be published, distributed, licenced or sold without permission from StrangeDevTeam
// Requests for the above to be made here: https://www.reddit.com/r/StrangeDev/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "StrangeEngine/Enemy", order = 1)]
public class Enemy : ScriptableObject
{
    public int health = 100;
    public string enemyName = "geoff";

    /// <summary>
    /// create an enemy with a name and health
    /// </summary>
    /// <param name="pHealth">health of the enemy</param>
    /// <param name="pName">name of the enemy</param>
    public Enemy(int pHealth, string pName)
    {
        health = pHealth;
        enemyName = pName;
    }

    //check if the enemy has been killed
    public bool CheckforKill()
    {
        if (health <= 0)
        {
            OnKill();
            return true;
        }
        return false;
    }


    //run when the enemy is killed
    void OnKill()
    {
        //if active quest is tracking kills of this enemy, increment the amount of this enemy killed
        Quest q = Quest.ActiveQuest;
        for (int i = 0; i <q.objectives.Count; i++)
        {
            KillQuest kq = converttoKillQuest(q.objectives[i]);
            if(kq != null)
            {
                for(int j = 0; j < kq.targets.Count; j++)
                {
                    if (this == kq.targets[j])
                    {
                        kq.TargetKilled();
                    }
                }
            }
        }
        
    }

    /// <summary>
    /// converts QuestStep to KillQuest, returns null if not possible
    /// </summary>
    /// <param name="pQuestStep">the Quest step to convert</param>
    /// <returns></returns>
    KillQuest converttoKillQuest (QuestObjective pQuestStep)
    {
        try
        {
            KillQuest temp = (KillQuest)pQuestStep;
            return temp;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
