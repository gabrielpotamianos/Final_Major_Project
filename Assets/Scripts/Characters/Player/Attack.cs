using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Attack : MonoBehaviour
{

    public GameObject projectile;
    public int costAbility1;
    public int costAbility2;
    public int costAbility3;
    public int costAbility4;

    [HideInInspector]
    public bool AbleToLoot;
    PlayerData playerData;

    bool AnimHasStarted = false;

    private void Awake()
    {
        playerData = GetComponent<PlayerData>();
    }


    // Update is called once per frame
    void Update()
    {
        //NOT CONSISTENT CODE
        if (Input.GetKeyDown(KeyCode.L) && AbleToLoot)
        {
            playerData.anim.SetBool("Looting", !playerData.anim.GetBool("Looting"));
            GetComponent<PlayerMovement>().enabled = !GetComponent<PlayerMovement>().enabled;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            Inventory.instance.inventory.SetActive(!Inventory.instance.inventory.activeSelf);
        }



    }



    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (Target.instance.getCurrTarget() == null)
                MessageManager.instance.DisplayMessage(Constants.NO_TARGET_SELECTED);
            else if (!Target.instance.targetInRange)
                MessageManager.instance.DisplayMessage(Constants.OUT_OF_RANGE);
            else
                playerData.anim.SetTrigger("BasicAttack");
        }
    }

    IEnumerator StopRootMotion(float length)
    {
        GetComponent<PlayerMovement>().enabled = false;
        playerData.anim.applyRootMotion = true;

        yield return new WaitForSeconds(length);

        playerData.anim.applyRootMotion = false;
        GetComponent<PlayerMovement>().enabled = true;
    }

    public void DealDamageToTarget()
    {
        if (Target.instance.getCurrEnemy())
        {
            Target.instance.getCurrEnemy().defaultStats.Hostile = true;
            Target.instance.getCurrEnemy().TakeDamage(playerData.AttackPower);
        }

    }



}
