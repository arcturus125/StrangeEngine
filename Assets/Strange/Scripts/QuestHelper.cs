using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestHelper : MonoBehaviour
{
    public static QuestHelper singleton;

    public float listingHeight = 0;

    [SerializeField]
    private Transform _content;
    [SerializeField]
    private QuestHelperListing _listing;
    [SerializeField]
    private Text questTitle;

    public int xPadding = 10;
    public int yPadding = -150;
    public int ContentBottomPadding = 5; // used to edit the gap between the bottom of the content window and the bottom of the quest listings. this it purely for QOL reasons, but it makes it look nicer


    List<QuestHelperListing> listings = new List<QuestHelperListing>();
    // Start is called before the first frame update
    void Start()
    {
        singleton = this;
    }

    int i = 0;
    // Update is called once per frame
    void Update()
    {
        if (StrangeQuestSystem.trackedQuest == null)
            this.gameObject.transform.GetChild(0).gameObject.SetActive(false); // you must change the active state of the child,
        else                                                                   // if you change the active state of this.gameObject
            this.gameObject.transform.GetChild(0).gameObject.SetActive(true);  // then this script will stop running and the quest helper will never appear again

        //UpdateGUI();
    }

    public void UpdateGUI()
    {
        if (StrangeQuestSystem.trackedQuest.complete)
            questTitle.text = "[COMPLETE] "+StrangeQuestSystem.trackedQuest.title;
        else 
            questTitle.text = StrangeQuestSystem.trackedQuest.title;

        // delete previous listings
        foreach(QuestHelperListing qhl in listings)
        {
            Destroy(qhl.gameObject);
        }
        listings.Clear();
        // create new listings
        int listingNumber = 0;
        foreach(QuestObjective qo in StrangeQuestSystem.trackedQuest.objectives)
        {
            QuestHelperListing temp = Instantiate(_listing,_content);
            RectTransform rt = temp.GetComponent<RectTransform>();
            //anchoring
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            //positioning
            rt.anchoredPosition += new Vector2( (rt.sizeDelta.x/2)+xPadding, yPadding + (-listingHeight * listingNumber));
            temp.objective = qo;
            listingNumber++;
            listings.Add(temp);
        }


        //resize content rect
        _content.GetComponent<RectTransform>().sizeDelta = new Vector2(
            _content.GetComponent<RectTransform>().sizeDelta.x,
            listingHeight * listingNumber + 5);
    }
}
