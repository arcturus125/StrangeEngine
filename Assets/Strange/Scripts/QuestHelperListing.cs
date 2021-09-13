using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestHelperListing : MonoBehaviour
{
    public QuestObjective objective;

    [SerializeField]
    private Text title;
    [SerializeField]
    private Image icon;

    [SerializeField]
    private Sprite incompleteObjectiveIcon;
    [SerializeField]
    private Sprite completeObjectiveIcon;


    // Start is called as the prefab is generated
    void Start()
    {
        title.text = objective.title;
        if (objective.objectiveComplete)
            icon.sprite = completeObjectiveIcon;
        else
            icon.sprite = incompleteObjectiveIcon;
    }
}
