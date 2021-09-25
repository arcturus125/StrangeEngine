using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Strange/Enemy", order = 3)]
public class Enemy : ScriptableObject
{
    public int health;
    public string Name;


    [Header("Indexes must match!")]
    public Item[] drops;
    public float[] dropChances;

}
