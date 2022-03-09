using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrangeEnemySystem : MonoBehaviour
{
    public static StrangeEnemySystem singleton;
    public GameObject damageIndicatorPrefab;
    public GameObject HealthbarPrefab;

    [Header("Drag the WHOLE player in here so that enemies can track the players position")]
    public GameObject playerGameObject;
    [Header("Drag the player's enemy target in here")]
    public Transform enemyTarget;

    void Awake()
    {
        singleton = this;
        if (playerGameObject == null) StrangeLogger.LogError("Strange enemy system is missing a reference to the player");
    }

    void Update()
    {    }
}
