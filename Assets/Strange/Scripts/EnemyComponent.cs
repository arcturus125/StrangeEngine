using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
    public float currentHealth;
    public bool hit = false; // has the enemy been hit by the player?

    public enum AIType
    {
        Passive,
        Reactive,
        Agressive
    }
    public AIType AI_type;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        EnemyAI();
    }

    void EnemyAI()
    {
        if(AI_type == AIType.Passive)
        {
            // randomly roam
        }
        else if(AI_type == AIType.Reactive)
        {
            if (hit)
            {// chase player
            }
            else
            {   // randomly roam
            }
        }
        else if (AI_type == AIType.Agressive)
        {
            // check fo LOS  to player
            // if LOS:
                // chase player
            // else:
                // randomly roam
        }
    }
}
