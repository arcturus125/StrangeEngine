// Copyright(c) 2020 arcturus125 & StrangeDevTeam
// Free to use and modify as you please, Not to be published, distributed, licenced or sold without permission from StrangeDevTeam
// Requests for the above to be made here: https://www.reddit.com/r/StrangeDev/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Developer note:
///     " to use this correctly, you need to create an Enemy in unity
///         right click > create > StrangeEngine > enemy
///         then you should attach this component to the gameobject for your enemy
///         and in the inspector for unity under "EnemyReference", drag in the enemy you just created"
/// </summary>

public class EnemyComponent : MonoBehaviour
{

    /// <summary>
    /// a reference to the Enemy ScriptableObject you (should have) made in the unity file explorer
    /// </summary>
    public Enemy enemyReference;


    // runs when the user presses "F" or the Use Key within interaction distance of the enemy (see PlayerInteraction.cs)
    public void Use()
    {
        enemyReference.health = 0; //TODO: this needs changing
        bool isDed = enemyReference.CheckforKill();
        if (isDed)
        {
            Kill();
        }
    }
    //runs when the enemy is killed
    void Kill()
    {
        Destroy(this.gameObject);
        PlayerInteraction.previousColliders.Remove(this.gameObject.GetComponent<Collider>());
    }
}
