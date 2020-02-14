using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject projectile;
    public int costAbility1;
    public int costAbility2;
    public int costAbility3;
    public int costAbility4;


    // Update is called once per frame
    void Update()
    {
        // print(transform.GetChild(2).gameObject.name);
        //GetTarget



        if (Input.GetKeyDown(KeyCode.Alpha1) && GetComponent<PlayerData>().currAR >= costAbility1)
        {
            GameObject instance;
            instance = Instantiate(projectile, transform.position + transform.forward, Quaternion.identity);
            instance.GetComponent<Projectile>().directionProjectile = gameObject.transform.forward;
            GetComponent<PlayerData>().ConsumeEnergy(costAbility1);
        }
    }

}
