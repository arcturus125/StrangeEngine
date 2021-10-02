using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Strange/Enemy", order = 3)]
public class Enemy : ScriptableObject
{

    [Header("Hover over attributes for tooltips!")]
    [Tooltip("The default health of the enemy - the amount of health they will have when they spawn in")]
    public int health; // the default health of the enemy when they spawn in
    [Tooltip("The name of the enemy")]
    public string enemyName; // the name of the enemy
    [Tooltip("The speed the enemy moves")]
    public float speed;
    [Tooltip("The range at which the enemy will target the player")]
    public float aggroRange;

    [Tooltip("if true, enemy will chase the player indefinately, regardless of their distance to their spawner.\n\n" +
        " if false, they will chase the player, but when they run out of their spawners 'Return to spawn radius' they will stop chasing the player and return to their spawner")]
    public bool enraged = true;

    [Tooltip("the percentage chance of the enemy moving: 0.9 for an enemy that moves 90% of the time")]
    public float giddiness = 0.5f;
    [Tooltip("the time (in seconds) the enemy will decide to move or not")]
    public float agitatedness = 5;

    [Tooltip("the smoothness of the enemy's movement. higher = more erratic bahaviour")]
    [Range(0.01f, 1.0f)]
    public float erraticnessOfMovement;


    public enum AIType
    {
        Passive,
        Reactive,
        Agressive
    }
    [Tooltip("Passive = enemy will ignore the player and roam randomly.\n\n" +
        "Reactive = enemy will ignore the player, unless hit, then the enemy will chase the player.\n\n" +
        "Agressive = enemy will chase the player if the player is within aggroRange and enemy has line of sight to the player, otherwise enemy will roam randomly")]
    public AIType AI_type;


    [Header("Indexes must match!")]
    [Tooltip("The items that the enemy will drop")]
    public Item[] drops;
    [Tooltip("The percentage chance of dropping the item")]
    public float[] dropChances;

}
