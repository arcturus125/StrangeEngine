using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
    public Enemy enemyReference; // the reference to the enemy file. contains the default details of the enemy

    public float currentHealth;
    public bool hit = false; // has the enemy been hit by the player?


    public Enemy.AIType AI_type;



    void Start()
    {
        position = new Vector2(transform.position.x, transform.position.z);

        currentHealth = enemyReference.health;
        AI_type = enemyReference.AI_type;

    }

    void Update()
    {
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


    public float maxSpeed = 2;
    public float steerStrength = 2;
    public float wanderStrength = 0.1f;

    Vector2 position;
    Vector2 velocity;
    Vector2 desiredDirection;

    private void Wander()
    {
        desiredDirection = (desiredDirection + Random.insideUnitCircle * wanderStrength).normalized;

        Vector2 desiredVelocity = desiredDirection * maxSpeed;
        Vector2 desiredSteeringForce = (desiredVelocity - velocity) * steerStrength;
        Vector2 acceleration = Vector2.ClampMagnitude(desiredSteeringForce, steerStrength) / 1;

        velocity = Vector2.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
        position += velocity * Time.deltaTime;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.SetPositionAndRotation( new Vector3(position.x, transform.position.y,position.y) , Quaternion.Euler(0, -angle, 0));
    }

    private void ChasePlayer()
    {
        desiredDirection = (new Vector2(StrangeEnemySystem.singleton.playerGameObject.transform.position.x, StrangeEnemySystem.singleton.playerGameObject.transform.position.z) - position).normalized;

        Vector2 desiredVelocity = desiredDirection * maxSpeed;
        Vector2 desiredSteeringForce = (desiredVelocity - velocity) * steerStrength;
        Vector2 acceleration = Vector2.ClampMagnitude(desiredSteeringForce, steerStrength) / 1;

        velocity = Vector2.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
        position += velocity * Time.deltaTime;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.SetPositionAndRotation(new Vector3(position.x, transform.position.y, position.y), Quaternion.Euler(0, -angle, 0));
    }
}
