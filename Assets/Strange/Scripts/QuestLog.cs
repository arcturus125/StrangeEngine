using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestLog : MonoBehaviour
{
    public KeyCode toggleQuestLog = KeyCode.L;

    public static QuestLog singleton;

    public Dropdown dropdown;
    public int listingHeight = 0;
    enum QuestLists
    {
        Active,
        Completed,
        Failed
    }
    QuestLists selectedQuestList = QuestLists.Active;

    [SerializeField]
    private Transform _content;
    [SerializeField]
    private QuestLogListing _prefab;

    private List<QuestLogListing> activeprefabs = new List<QuestLogListing>();

    private bool isMenuOpen = false;

    // Start is called before the first frame update

    private void Awake()
    {
        singleton = this;
    }
    void Start()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(toggleQuestLog))
        {
            isMenuOpen = !isMenuOpen;
            this.transform.GetChild(0).gameObject.SetActive(isMenuOpen);
        }
    }

    public void OnDropdownChanged()
    {
        selectedQuestList = (QuestLists)dropdown.value;
        UpdateGUI();
    }

    public void UpdateGUI()
    {
        //delete previous listings
        foreach(QuestLogListing qll in activeprefabs)
        {
            Destroy(qll.gameObject);
        }
        activeprefabs.Clear();

        // get the correct list of quests
        List<Quest> questList = new List<Quest>();
        if (selectedQuestList == QuestLists.Active)
        {
            questList = StrangeQuestSystem.activeQuests;
        }
        else if(selectedQuestList == QuestLists.Completed)
        {
            questList = StrangeQuestSystem.completedQuests;
        }
        else if (selectedQuestList == QuestLists.Failed)
        {
            questList = StrangeQuestSystem.failedQuests;
        }
        else
        {
            StrangeLogger.LogError("Attempting to show a quest list that doesnt exist!");
            return;
        }

        // create new listings
        int listingNumber = 0;
        foreach (Quest q in questList)
        {
            QuestLogListing temp = Instantiate(_prefab, _content);
            temp.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -listingHeight * listingNumber);
            temp.quest = q;
            listingNumber++;
            activeprefabs.Add(temp);
        }
        

    }
}
