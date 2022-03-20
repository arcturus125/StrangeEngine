using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomContentResize : ContentSizeFitter
{
    /* the default content size fitter built into unity works great, but it resets the postion of the rectTransform.
     * this extension of that script recalculates the position based onthe content's size
     */
    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        RectTransform rt = this.GetComponent<RectTransform>();
        float height = rt.rect.height;
        rt.transform.localPosition = new Vector3(rt.transform.localPosition.x, -5 -height/2 * rt.localScale.y,0);
    }
}
