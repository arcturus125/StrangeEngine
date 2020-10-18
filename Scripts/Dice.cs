// Copyright(c) 2020 arcturus125 & StrangeDevTeam
// Free to use and modify as you please, Not to be published, distributed, licenced or sold without permission from StrangeDevTeam
// Requests for the above to be made here: https://www.reddit.com/r/StrangeDev/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    //returns an integer equal to diceroll (inputting 3d20 will output the sum of 3 numbers between 1 and 20)
    public static int Roll(string dicename)
    {
        string[] diceNumbers = dicename.Split('d');
        int numberOfDice;
        int numberOfSides;
        bool isValidNumberOfDice = int.TryParse(diceNumbers[0], out numberOfDice);
        bool isValidNumberOfSides = int.TryParse(diceNumbers[1], out numberOfSides);
        if(isValidNumberOfDice && isValidNumberOfSides)
        {
            int total = 0;
            for(int i = 0; i < numberOfDice; i++)
            {
                int randomNumber = (int)(Random.Range(1, numberOfSides +1 ));
                if(randomNumber > 20)
                {
                    randomNumber = 20;
                }
                total += randomNumber;
            }
            return total;
        }
        return -1;
    }
}
