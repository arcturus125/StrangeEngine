using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrangeEnemySystem : MonoBehaviour
{
    public static StrangeEnemySystem singleton;

    [Header("Drag the player in here so that enemies can track the players position")]
    public GameObject playerGameObject;

    void Start()
    {
        singleton = this;
    }

    void Update()
    {    }
}
