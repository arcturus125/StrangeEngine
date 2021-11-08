using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrangeChatSystem : MonoBehaviour
{
    public static StrangeChatSystem singleton;
    public GameObject[] dialogueUI;

    void Start()
    {
        singleton = this;
    }
}
