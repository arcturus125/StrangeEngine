using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;

public class DialogueNode : Node
{
    public string GUID;

    public string DialogueText;
    public Quest quest;

    public bool entrypoint;

    public void GetGUID()
    {
        GUIUtility.systemCopyBuffer = GUID;
    }
    public void SetQuest(Quest q)
    {
        for (int i = 0; i < extensionContainer.childCount; i++)
        {
            UnityEngine.UIElements.VisualElement element = extensionContainer.ElementAt(i);
            if(element.GetType() == typeof(ObjectField))
            {
                (element as ObjectField).value = q;
                quest = q;
            }
        }
    }
}
