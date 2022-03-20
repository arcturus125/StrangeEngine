using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScrollViewContentScaler : MonoBehaviour
{
    /* when using ScrollViews in unity, they can be a pain to use because the content gmeobject never scales correctly
     * this script fixes that, by setting the content's size to the sum of the size of its children
     * this script must be attached specifically to the auto-generated "content" game object
     */


    [SerializeField] RectTransform[] children;
    void Update()
    {
        float y = 0;
        foreach(RectTransform child in children)
        {
            y += (child.rect.height * child.localScale.y);
        }
        RectTransform rt =  this.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.rect.x, y);
    }
}
