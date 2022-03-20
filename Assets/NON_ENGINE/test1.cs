using UnityEngine;

public class test1 : EnemyComponent
{

    //  ##############################################################################
    //  #   Diagram to explain the order of execution of the base class' functions   #
    //  ##############################################################################
    /*    
     *           update() ---> Enemy AI()                   // decides whether to run Wander, SpawnerRecall or ChasePlayer based on enemy's state
     *                            |                                                                                                           |
     *         /------------------|---------------\            States: - Agressive (chase player, wander when no player in LOS)               |
     *         |                  |               |                    - Passive   (ignore player, wander all the time)          <------------/
     *         |                  |               |                    - Reactive  (Passive until Hit, then Agressive)                                                          
     *         V                  V               V                                                                               
     *      Wander2D()  SpawnerRecall2D()   ChasePlayer2D()       // sets the direction of movement, then uses the Movement2D() function to move 
     *       |    |               |               |                   NOTE: If an enemy wanders(or chases a player) too far from their spawn point,
     *       |    |               |               |                          they turn back and walk back to their spawn point. this is what SpawnerRecall() does
     *       |    \---------------|---------------/ 
     *       |                    V
     *       |               Movement2D(direction) //parameter: direction the enemy moves
     *       |                    |
     *       |                    |
     *       V                    V
     *  WhileDosile()        WhileMoving(direction)    // WhileDosile = run every frame when stood still
     *                                                 // WhileMoving = run every frame when moving, parameter: direction the enemy is moving
     *      
     */



    // basic unity functions
    protected override void Start() { base.Start(); }
    protected override void Update() { base.Update(); }

    // runs all the functions below based on the enemy settings
    protected override void EnemyAI() { base.EnemyAI(); }

    // movement for grounded enemies
    protected override void Wander2D() { base.Wander2D(); }
    protected override void ChasePlayer2D() { base.ChasePlayer2D(); }
    protected override void SpawnerRecall2D() { base.SpawnerRecall2D(); }
    protected override void Movement2D(Vector2 directionOfMovement) { base.Movement2D(directionOfMovement); }

    // movement for flying enemies
    protected override void Wander3D() { base.Wander3D(); }
    protected override void ChasePlayer3D() { base.ChasePlayer3D(); }
    protected override void SpawnerRecall3D() { base.SpawnerRecall3D(); }
    protected override void Movement3D(Vector3 directionOfMovement) { base.Movement3D(directionOfMovement); }

    // attacking
    protected override void AttackState() { base.AttackState();

    }
    protected override void Attack() { base.Attack();
        Debug.Log("Attack");
    }

    // typically used for animating models
    protected override void WhileDosile() { base.WhileDosile(); }
    protected override void WhileMoving(Vector3 velocity) { base.WhileMoving(velocity);}
}
