using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    Dialogue d;

    // Start is called before the first frame update
    void Start()
    {
        Dialogue d2 = new Dialogue("dialogue two");
        d = new Dialogue("dialogue one", d2);

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            d.Play();
        }
    }
}
