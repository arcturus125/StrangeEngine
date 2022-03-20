using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Text UItext;
    public Slider slider;
    float life;

    private const float lifetime = 5;



    // Start is called before the first frame update
    void Start()
    {
        EnemyComponent e = GetComponentInParent<EnemyComponent>();
        UItext.text = e.enemyReference.enemyName;


        float percent = e.currentHealth / e.enemyReference.health;
        slider.value = percent;
        life = 5;


        // anti-scaling - negates the scaling of the parent
        transform.localScale = new Vector3(
            transform.localScale.x / transform.lossyScale.x,
            transform.localScale.y / transform.lossyScale.y,
            transform.localScale.z / transform.lossyScale.z);

        this.transform.position += new Vector3(0, 1, 0);
    }


    public void UpdateHealth(EnemyComponent e)
    {
        life = 5;
        float percent = e.currentHealth / e.enemyReference.health;
        slider.value = percent;
    }

    private void Update()
    {
        life -= Time.deltaTime;
        if(life <= 0)
        {
            Destroy(this.gameObject);
        }

        Vector3 target = transform.position + (transform.position - Camera.main.transform.position);
        transform.LookAt(target);
    }
}
