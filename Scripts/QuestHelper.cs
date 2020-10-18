using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestHelper : MonoBehaviour
{
    const float lineHeight = 14;
    static float lineLength = 35.0f;


    static float padding = 5;
    static List<GameObject> UIObjectives = new List<GameObject>();
    static List<float> heights = new List<float>();

    static GameObject content;
    static GameObject scrollView;
    static RectTransform scrollUIDetails;

    void Start()
    {
        content = GameObject.Find("QHContent"); // the content within the scroll menu
        scrollView = GameObject.Find("QHScrollView"); // the size and dimentions of the window the user sees
        scrollUIDetails = scrollView.GetComponent<RectTransform>();
    }
    // Update is called once per frame
    void Update()
    {
        UpdateHelper();
    }


    public static void UpdateHelper()
    {
        Text[] texts = UIController.QuestHelperPanel.GetComponentsInChildren<Text>();
        Text title = texts[0];

        

        //destroy objectives before making duplicates
        DestroyObjectives();

        if (Quest.ActiveQuest != null)
        {
            for (int i = 0; i < Quest.ActiveQuest.objectives.Count; i++)
            {
                //creating the gameobject, and dealing with inheritence
                GameObject newGO = new GameObject("Objective");
                newGO.transform.SetParent(content.transform);

                //text
                Text newGOtext = newGO.AddComponent<Text>();
                if (!Quest.ActiveQuest.objectives[i].objectiveComplete)
                {
                    newGOtext.text = "- " + Quest.ActiveQuest.objectives[i].title;
                }
                else
                {
                    newGOtext.text = "+ (Completed)   " + Quest.ActiveQuest.objectives[i].title;
                }
                newGOtext.verticalOverflow = VerticalWrapMode.Overflow;
                

                //anchoring
                RectTransform UIdetails = newGO.GetComponent<RectTransform>();
                UIdetails.anchorMin = new Vector2(0, 1);
                UIdetails.anchorMax = new Vector2(0, 1);


                //positioning
                Canvas.ForceUpdateCanvases();

                int lines =  (int)Mathf.Ceil(newGOtext.text.Length/lineLength) +1;
                float tempHeight = lineHeight * lines;
                    //sizing
                UIdetails.sizeDelta = new Vector2(content.GetComponent<RectTransform>().rect.width, tempHeight);
                heights.Add(tempHeight);

                newGO.transform.position =
                    new Vector3(
                    content.transform.position.x + (UIdetails.rect.width / 2) + padding,
                    content.transform.position.y - (tempHeight/2) - calculateHeight(),
                    content.transform.position.z);

                //without a font, the text cannot render
                Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
                newGOtext.font = ArialFont;
                //newGOtext.material = ArialFont.material;

                //add items to list
                UIObjectives.Add(newGO);
            }
            //after adding the texts. adjust the height of the content
            RectTransform contentSize = content.GetComponent<RectTransform>();
            contentSize.sizeDelta = new Vector2(
                contentSize.sizeDelta.x,
                calculateHeight()
                );

        }
    }
    static float calculateHeight()
    {
        float height = 0;

        for(int i = 0; i < UIObjectives.Count; i++)
        {
            height += UIObjectives[i].GetComponent<RectTransform>().sizeDelta.y;
            //height += UIObjectives[i].GetComponent<RectTransform>().rect.height;
            //height += heights[i];
        }
        return height;
    }
    static void DestroyObjectives()
    {
        for(int i = 0; i<UIObjectives.Count; i++)
        {
            Destroy(UIObjectives[i]);
        }
        UIObjectives = new List<GameObject>();
        heights = new List<float>();
    }
}
