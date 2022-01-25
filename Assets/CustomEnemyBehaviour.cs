using UnityEngine;

public class CustomEnemyBehaviour : EnemyComponent
{
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

    // typically used for animating models
    protected override void WhileDosile() { base.WhileDosile(); }
    protected override void WhileMoving(Vector3 velocity) { base.WhileMoving(velocity);}
}
