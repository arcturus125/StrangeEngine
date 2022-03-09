using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    public float speed = 0.5f;
    public float life = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        life -= Time.deltaTime;
        if (life < 0) Destroy(this.gameObject);

        transform.position += new Vector3(0, speed * Time.deltaTime, 0);
        Vector3 target = transform.position + ( transform.position - Camera.main.transform.position);
        transform.LookAt(target);
    }
}
