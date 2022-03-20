using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactible
{
    Quest q1;
    Quest q2;


    // Unity Monobehaviour Functions
    public override void Start()
    {
        q1.TriggerQuest();
    }
    public override void Update()
    {
        base.Update();


        if(Input.GetKeyDown(KeyCode.G))
        {
            q2.TriggerQuest();
        }
    }


    // Strange Interactible Functions
    public override void Use()
    {
    }
    public override void WhileNearby()
    {
    }
    public override void OnNearby()
    {
        base.OnNearby();
    }
    public override void NoLongerNearby()
    {
        base.NoLongerNearby();
    }
}
