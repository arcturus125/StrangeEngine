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
    public Transform head; //raycasts for LOS checks are done from thsi transform

    //### set by the code

    public EnemySpawner parentSpawner; // the reference to the spawner that spawned this enemy

    float currentHealth;
    bool returnToSpawner = false;
    bool isDosile = false;

    // movement vectors for grounded enemies
    Vector2 position;         //} used in enemy movement code
    Vector2 velocity;         //} using forces that counter out 
    Vector2 desiredDirection; //} for more realistic movement

    // movement vectors for flying enemies
    Vector3 position3d;         //} used in enemy movement code for flying enemies
    Vector3 velocity3d;         //} using forces that counter out 
    Vector3 desiredDirection3d; //} for more realistic movement




    protected virtual void Start()
    {
        // if there is no StrangeEnemySystem in the scene, throw an error and disable all enemy scripts to stop continuously throwing thsi error.
        if (StrangeEnemySystem.singleton == null)
        {
            StrangeLogger.LogError("Attempting to use an Enemy without StrangeEnemySystem present in scene. Please drag StrangeEnemySystem into the scene");
            Destroy(this);
            if (parentSpawner) Destroy(parentSpawner);
        }
        position = new Vector2(transform.position.x, transform.position.z);
        position3d = transform.position;


        currentHealth = enemyReference.health;
        AI_type = enemyReference.AI_type;
        maxSpeed = enemyReference.speed;

        InvokeRepeating("CalculateGiddiness", 1, enemyReference.agitatedness);
    }

    protected virtual void Update()
    {
        EnemyAI();
    }

    /// <summary>
    /// Runs once every frame, decides on the movement patterns of the enemy:
    /// <para>Passive: ignores player, randomly wanders</para>
    /// <para>Reactive: ingores player until hit</para>
    /// <para>Aggressive: randomly wanders, until it has LOS of player, then enemy chases player</para>
    /// </summary>
    protected virtual void EnemyAI()
    {
        bool cancelAI = false;
        // ### Spawner Recall ###
        if (parentSpawner)
        {
            // if the enemy has wandered too far from their spawner, tell the enemy to return to their spawn area
            float distanceFromSpawner = Vector3.Distance(this.transform.position, parentSpawner.transform.position);
            if ((distanceFromSpawner > parentSpawner.returnToSpawnRadius) && !enemyReference.enraged)
            {
                returnToSpawner = true;
            }
            if (returnToSpawner)
            {
                if (enemyReference.isFlyingEnemy)
                    SpawnerRecall3D();
                else
                    SpawnerRecall2D();
                cancelAI = true;
            }
        }

        // ### Enemy AI ###
        if (!cancelAI)
        {
            bool runAgressive = false;
            if (AI_type == Enemy.AIType.Passive)
            {
                if (enemyReference.isFlyingEnemy)
                    Wander3D();
                else
                    Wander2D();
            }

            else if (AI_type == Enemy.AIType.Reactive)
            {
                if (hit)
                {
                    // the code here is essentially the same as agressive enemies
                    // instead of typing it twice, recycle the code
                    runAgressive = true;
                }
                else
                {
                    if (enemyReference.isFlyingEnemy)
                        Wander3D();
                    else
                        Wander2D();
                }
            }


            else if (AI_type == Enemy.AIType.Agressive || runAgressive)
            {
                RaycastHit hit;
                Vector3 direction = StrangeEnemySystem.singleton.playerGameObject.transform.position - this.transform.position;
                Vector3 raycastStart = head == null ? this.transform.position : head.position;
                if (Physics.Raycast(raycastStart, direction, out hit, enemyReference.aggroRange))
                {
                    if (hit.collider.transform.IsChildOf(StrangeEnemySystem.singleton.playerGameObject.transform))
                    {
                        // if LOS to player

                        float distance = Vector3.Distance(raycastStart, hit.point);
                        if (distance < enemyReference.attackRange)
                        {
                            Debug.DrawRay(raycastStart, direction, Color.blue);
                            AttackState();
                        }
                        else
                        {
                            Debug.DrawRay(raycastStart, direction, Color.green);
                            if (enemyReference.isFlyingEnemy)
                                ChasePlayer3D();
                            else
                                ChasePlayer2D();
                        }
                    }
                    else
                    {
                        // if LOS to player is broken
                        Debug.DrawRay(raycastStart, direction, Color.red);
                        if (enemyReference.isFlyingEnemy)
                            Wander3D();
                        else
                            Wander2D();
                    }
                }
                else
                {
                    if (enemyReference.isFlyingEnemy)
                        Wander3D();
                    else
                        Wander2D();
                }
            }
        }
    }

    /// <summary>
    /// decides whether the enemy remains still or not
    /// </summary>
    protected virtual void CalculateGiddiness()
    {
        float rng = Random.Range(0, 100);
        if ((rng / 100) > enemyReference.giddiness)
        {
            isDosile = true;
        }
        else
        {
            isDosile = false;
        }
    }

    // ##### 2d Movement ####

    /// <summary>
    /// selects random directions for the enemy to travel. Enemy only moves if isDosile is false
    /// </summary>
    protected virtual void Wander2D()
    {
        if (!isDosile)
        {
            desiredDirection = (desiredDirection + Random.insideUnitCircle * wanderStrength).normalized;
            Movement2D(desiredDirection);
        }
        else
            WhileDosile();
    }
    /// <summary>
    /// passes the players position onto Movement()
    /// </summary>
    protected virtual void ChasePlayer2D()
    {
        desiredDirection = (new Vector2(StrangeEnemySystem.singleton.playerGameObject.transform.position.x, StrangeEnemySystem.singleton.playerGameObject.transform.position.z) - position).normalized;
        Movement2D(desiredDirection);
    }
    /// <summary>
    /// passes the Spawner's position into Movement()
    /// </summary>
    protected virtual void SpawnerRecall2D()
    {
        desiredDirection = (new Vector2(parentSpawner.transform.position.x, parentSpawner.transform.position.z) - position).normalized;
        Movement2D(desiredDirection);


        float distanceFromSpawner = Vector3.Distance(this.transform.position, parentSpawner.transform.position);
        if (distanceFromSpawner < parentSpawner.spawnRadius)
            returnToSpawner = false;
    }

    /// <summary>
    /// enemy turns and moves in the desired direction of movement
    /// </summary>
    /// <param name="directionOfMovement"> the direction you want the enemy to move</param>
    protected virtual void Movement2D(Vector2 directionOfMovement)
    {
        // sebastian lague : ants video
        Vector2 desiredVelocity = directionOfMovement * maxSpeed;
        Vector2 desiredSteeringForce = (desiredVelocity - velocity) * steerStrength;
        Vector2 acceleration = Vector2.ClampMagnitude(desiredSteeringForce, steerStrength) / 1;

        velocity = Vector2.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
        position += velocity * Time.deltaTime;

        WhileMoving(velocity);

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.SetPositionAndRotation(new Vector3(position.x, transform.position.y, position.y), Quaternion.Euler(0, -angle, 0));
    }




    // ##### 3d Movement ####

    protected virtual void Wander3D()
    {
        if (!isDosile)
        {
            desiredDirection3d = (desiredDirection3d + Random.insideUnitSphere * wanderStrength).normalized;

            // if useWorldHeight is False, offset maxheight and minHeight calculations by the ground height
            // Note: if there is no ground under enemy, they will remember the offset of las time they were above ground
            float groundOffset = 0;
            if (!enemyReference.useWorldHeight)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit))
                {
                    groundOffset = hit.point.y;
                }
            }

            // cap maxheight
            if (desiredDirection3d.y + position3d.y > enemyReference.maxheight + groundOffset)
            {
                if (desiredDirection3d.y > 0)
                {
                    desiredDirection3d = new Vector3(desiredDirection3d.x, (enemyReference.maxheight + groundOffset) - position3d.y, desiredDirection3d.z);
                }
            }
            // cap minheight
            else if (desiredDirection3d.y + position3d.y < enemyReference.minheight + groundOffset)
            {
                if (desiredDirection3d.y < 0)
                    desiredDirection3d = new Vector3(desiredDirection3d.x, (enemyReference.minheight + groundOffset) - position3d.y, desiredDirection3d.z);
            }

            Movement3D(desiredDirection3d);
        }
        else
            WhileDosile();
    }
    protected virtual void ChasePlayer3D()
    {
        desiredDirection3d = (StrangeEnemySystem.singleton.playerGameObject.transform.position - position3d).normalized;
        Debug.DrawRay(position3d, StrangeEnemySystem.singleton.playerGameObject.transform.position, Color.red);
        Movement3D(desiredDirection3d);
    }
    protected virtual void SpawnerRecall3D()
    {
        desiredDirection3d = (parentSpawner.transform.position - position3d).normalized;
        Movement2D(desiredDirection3d);


        float distanceFromSpawner = Vector3.Distance(this.transform.position, parentSpawner.transform.position);
        if (distanceFromSpawner < parentSpawner.spawnRadius)
            returnToSpawner = false;
    }

    protected virtual void Movement3D(Vector3 directionOfMovement)
    {
        Vector3 desiredVelocity = directionOfMovement * maxSpeed;
        Vector3 desiredSteeringForce = (desiredVelocity - velocity3d) * steerStrength;
        Vector3 acceleration = Vector3.ClampMagnitude(desiredSteeringForce, steerStrength) / 1;

        velocity3d = Vector3.ClampMagnitude(velocity3d + acceleration * Time.deltaTime, maxSpeed);
        position3d += velocity3d * Time.deltaTime;


        WhileMoving(velocity);

        if (!enemyReference.lockEnemyTilt)
        {
            transform.SetPositionAndRotation(new Vector3(position3d.x, position3d.y, position3d.z), Quaternion.Euler(0, 0, 0));
            transform.LookAt(velocity3d + transform.position);
        }
        else
        {
            float angle = Mathf.Atan2(velocity3d.y, velocity3d.x) * Mathf.Rad2Deg;
            transform.SetPositionAndRotation(new Vector3(position3d.x, position3d.y, position3d.z), Quaternion.Euler(0, -angle, 0));
        }
    }


    // ##### Attacking #####

    float attackCooldown = 0;
    protected virtual void AttackState()
    {
        if(attackCooldown <= 0)
        {
            Attack();
            attackCooldown = enemyReference.attackCooldown;
        }
        else
        {
            attackCooldown -= Time.deltaTime;
        }
    }
    // ##### Interface Functions #####

    protected virtual void WhileDosile() { }
    protected virtual void WhileMoving(Vector3 velocity) { }
    protected virtual void Attack() { }


    private void OnDestroy()
    {
        OnKilled();
    }
    protected void OnKilled()
    {
        if (parentSpawner)
        {
            parentSpawner.enemies.Remove(this.gameObject);
            parentSpawner.currentNumberOfEnemies--;
        }

        // ####### killquest event
        // search through all the active quests
        for (int i = 0; i < StrangeQuestSystem.activeQuests.Count; i++)
        {
            // seach through the objectives of each quest
            for (int y = 0; y < StrangeQuestSystem.activeQuests[i].objectives.Count; y++)
            {
                if(StrangeQuestSystem.activeQuests[i].objectives[y].objectiveType == QuestObjective.ObjectiveType.KillQuest)
                {
                    KillQuest typecast = (KillQuest)(StrangeQuestSystem.activeQuests[i].objectives[y]);
                    typecast.CheckEnemyKill(enemyReference);
                }
            }
        }
    }
    

}
