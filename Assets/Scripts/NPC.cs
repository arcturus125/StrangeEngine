using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactible
{
    public DialogueTree dtree;


    // Unity Monobehaviour Functions
    public override void Start()
    {

    }
    public override void Update()
    {
        base.Update();
    }


    // Strange Interactible Functions
    public override void Use()
    {
        if (!StrangeChatSystem.isInDialogue)
        {
            dtree.Play();
        }
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
