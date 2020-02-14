using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public GameObject HealthBar;
    public float Health = 100;



    // Update is called once per frame
    void Update()
    {
        if (Health <= 0)
        {
            gameObject.SetActive(false);
            //HealthBar.SetActive(false);
        }
    }

    public void TakeDamage(float dmg)
    {
        Health -= dmg;
        UpdateBar(Health);

    }

    public void UpdateBar(float health)
    {
        HealthBar.GetComponent<Slider>().value = health / 100.0f;

    }
}
