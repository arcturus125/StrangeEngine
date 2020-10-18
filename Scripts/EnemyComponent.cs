// Copyright(c) 2020 arcturus125 & StrangeDevTeam
// Free to use and modify as you please, Not to be published, distributed, licenced or sold without permission from StrangeDevTeam
// Requests for the above to be made here: https://www.reddit.com/r/StrangeDev/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
    public Enemy enemyReference;


    public void Use()
    {
        enemyReference.health = 0; //TODO: this needs changing
        bool isDed = enemyReference.CheckforKill();
        if (isDed)
        {
            Kill();
        }
    }
    void Kill()
    {
        Destroy(this.gameObject);
        PlayerInteraction.previousColliders.Remove(this.gameObject.GetComponent<Collider>());
    }
}
