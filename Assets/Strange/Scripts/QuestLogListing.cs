using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestLogListing : MonoBehaviour
{
    public Quest quest;

    [SerializeField]
    private Button btn;
    [SerializeField]
    private Text questTitle;
    [SerializeField]
    private Image icon;

    public Sprite completeIcon;
    public Sprite incompleteIcon;

    // Start is called before the first frame update
    void Start()
    {
        questTitle.text = quest.title;
        
        if(quest.complete)
        {
            icon.sprite = completeIcon;
        }
        else
        {
            icon.sprite = incompleteIcon;
        }
    }

    public void ListingClicked()
    {
        QuestLog.singleton.QuestListingClicked(quest);
    }
}
