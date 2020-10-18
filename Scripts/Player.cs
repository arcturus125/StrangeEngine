// Copyright(c) 2020 arcturus125 & StrangeDevTeam
// Free to use and modify as you please, Not to be published, distributed, licenced or sold without permission from StrangeDevTeam
// Requests for the above to be made here: https://www.reddit.com/r/StrangeDev/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  Developer Note:
///     "this class is mostly empty because i am assuming you want to code your own player,
///         put all your code for the player in here and treat this as a class to hold all the players data
///         
///         if you are making a multiplayer game, you may need to create multiple instances of this class for each player in the world"
/// </summary>


// IMPORT:: attach this script to the player
public class Player : MonoBehaviour
{

    public static Inventory playerInv = new Inventory(); // create an inventory for the player

    void Start()
    {
    }

    void Update()
    {

    }
}
