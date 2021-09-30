using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Strange/Enemy", order = 3)]
public class Enemy : ScriptableObject
{
    public int health; // the default health of the enemy when they spawn in
    public string enemyName; // the name of the enemy
    public float speed;
    public float aggroRange; // the range at which the enemy will start looking for the player
    public bool enraged = true; // if true, enemy will chase the player indefinately, regardless of their distance to their spawner


    public enum AIType
    {
        Passive,
        Reactive,
        Agressive
    }
    public AIType AI_type;


    [Header("Indexes must match!")]
    public Item[] drops;
    public float[] dropChances;

}
