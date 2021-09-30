using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
    //### set by the user

    public Enemy enemyReference; // the reference to the enemy file. contains the default details of the enemy
    public Enemy.AIType AI_type;
    public bool hit = false; // has the enemy been hit by the player?
    public float maxSpeed = 2; // the fastest the enemy can possibly move
    public float steerStrength = 2; // the speed the enemy can turn around
    public float wanderStrength = 0.1f; // the frequency a new direction to move is picked (higher = more errattic , lower = smoother movement)

    //### set by the code

    public EnemySpawner parentSpawner; // the reference to the spawner that spawned this enemy

    float currentHealth;
    Vector2 position;         //} used in enemy movement code
    Vector2 velocity;         //} using forces that counter out 
    Vector2 desiredDirection; //} for more realistic movement
    bool returnToSpawner = false;



    void Start()
    {
        position = new Vector2(transform.position.x, transform.position.z);

        currentHealth = enemyReference.health;
        AI_type = enemyReference.AI_type;
        maxSpeed = enemyReference.speed;

    }

    void Update()
    {
        // if the enemy hs wandered too far from their spawner
        float distanceFromSpawner = Vector3.Distance(this.transform.position, parentSpawner.transform.position);
        if((distanceFromSpawner > parentSpawner.returnToSpawnRadius) && !enemyReference.enraged )
        {
            returnToSpawner = true;
        }

        if (returnToSpawner)
            SpawnerRecall();
        else
            EnemyAI();
    }

    void EnemyAI()
    {

        if(AI_type == Enemy.AIType.Passive)
        {
            Wander();
        }


        else if(AI_type == Enemy.AIType.Reactive)
        {
            if (hit)
            {
                ChasePlayer();
            }
            else
            {
                Wander();
            }
        }


        else if (AI_type == Enemy.AIType.Agressive)
        {
            RaycastHit hit;
            Vector3 direction = StrangeEnemySystem.singleton.playerGameObject.transform.position - this.transform.position;
            if (Physics.Raycast(this.transform.position,  direction , out hit, enemyReference.aggroRange ))
            {
                if(hit.collider.transform.IsChildOf( StrangeEnemySystem.singleton.playerGameObject.transform) )
                {
                    // if LOS to player
                    Debug.DrawRay(this.transform.position, direction, Color.green);

                    ChasePlayer();
                }
                else
                {
                    // if LOS to player is broken
                    Debug.DrawRay(this.transform.position, direction, Color.red);

                    Wander();
                }
            }
            else
            {
                Wander();
            }
        }
    }



    // these functions just set the target, then they call Movement(), since the movement code is identical, only the target changes
    private void Wander()
    {
        desiredDirection = (desiredDirection + Random.insideUnitCircle * wanderStrength).normalized;
        Movement();
    }
    private void ChasePlayer()
    {
        desiredDirection = (new Vector2(StrangeEnemySystem.singleton.playerGameObject.transform.position.x, StrangeEnemySystem.singleton.playerGameObject.transform.position.z) - position).normalized;
        Movement();
    }
    private void SpawnerRecall()
    {
        desiredDirection = (new Vector2(parentSpawner.transform.position.x, parentSpawner.transform.position.z) - position).normalized;
        Movement();


        float distanceFromSpawner = Vector3.Distance(this.transform.position, parentSpawner.transform.position);
        if (distanceFromSpawner < parentSpawner.spawnRadius)
            returnToSpawner = false;
    }



    private void Movement()
    {
        // sebastian lague : ants video
        Vector2 desiredVelocity = desiredDirection * maxSpeed;
        Vector2 desiredSteeringForce = (desiredVelocity - velocity) * steerStrength;
        Vector2 acceleration = Vector2.ClampMagnitude(desiredSteeringForce, steerStrength) / 1;

        velocity = Vector2.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
        position += velocity * Time.deltaTime;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.SetPositionAndRotation(new Vector3(position.x, transform.position.y, position.y), Quaternion.Euler(0, -angle, 0));
    }
}
