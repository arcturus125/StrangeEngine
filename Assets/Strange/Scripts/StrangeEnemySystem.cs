using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrangeEnemySystem : MonoBehaviour
{
    public static StrangeEnemySystem singleton;

    [Header("Drag the player in here so that enemies can track the players position")]
    public GameObject playerGameObject;

    void Awake()
    {
        singleton = this;
        if (playerGameObject == null) StrangeLogger.LogError("Strange enemy system is missing a reference to the player");
    }

    void Update()
    {    }
}
