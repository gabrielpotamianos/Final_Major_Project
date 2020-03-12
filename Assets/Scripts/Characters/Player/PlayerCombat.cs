using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PlayerCombat : MonoBehaviour
{

    public GameObject projectile;
    public int costAbility1;
    public float Ability1Time;
    public float customRange;
    public int costAbility2;
    public int costAbility3;
    public int costAbility4;

    [HideInInspector]
    public bool AbleToLoot;
    PlayerData playerData;

    bool AnimHasStarted = false;

    private void Awake()
    {
       // CharacterSelection.ChosenCharacter = new CharacterInfo();
       // CharacterSelection.ChosenCharacter.breed = (CharacterInfo.Breed)Enum.Parse(typeof(CharacterInfo.Breed), gameObject.tag);
        playerData = GetComponent<PlayerData>();
    }


    // Update is called once per frame
    void Update()
    {
      //  CharacterSelection.ChosenCharacter.breed = (CharacterInfo.Breed)Enum.Parse(typeof(CharacterInfo.Breed), gameObject.tag);

        //NOT CONSISTENT CODE
        if (Input.GetKeyDown(KeyCode.L) && AbleToLoot)
        {
            playerData.anim.SetBool("Looting", !playerData.anim.GetBool("Looting"));
            GetComponent<PlayerMovement>().enabled = !playerData.anim.GetBool("Looting");
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            Inventory.instance.inventory.SetActive(playerData.anim.GetBool("Looting"));
        }



    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, customRange);
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
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (Target.instance.getCurrTarget() == null)
                MessageManager.instance.DisplayMessage(Constants.NO_TARGET_SELECTED);
            //else if (!Target.instance.targetInRange)
            //    MessageManager.instance.DisplayMessage(Constants.OUT_OF_RANGE);
            else if(Vector3.Distance(gameObject.transform.position, Target.instance.getCurrEnemy().gameObject.transform.position) <customRange )
            {
                playerData.anim.SetBool("Ability 1", true);
                StartCoroutine(Charge());
            }

        }
    }

    #region Warrior Abilities

    IEnumerator Charge()
    {
        while(Vector3.Distance(gameObject.transform.position, Target.instance.getCurrEnemy().gameObject.transform.position)>Target.instance.RangeDistance)
        {
            GetComponent<Rigidbody>().position = Vector3.MoveTowards(transform.position, Target.instance.getCurrEnemy().gameObject.transform.position, Time.deltaTime * Ability1Time);
            gameObject.transform.LookAt(Target.instance.getCurrEnemy().transform, transform.up);
            yield return null;

        }
        GameObject.Find("PlasmaMissileRed").GetComponent<ParticleSystem>().Play();
        playerData.anim.SetBool("Ability 1 Condition", true);
        yield return null;

        
    }

    public void EndAbility1()
    {
        playerData.anim.SetBool("Ability 1", false);
        playerData.anim.SetBool("Ability 1 Condition", false);
        GameObject.Find("PlasmaMissileRed").GetComponent<ParticleSystem>().Stop();


    }





    #endregion















    public void ApplyRootMotion()
    {
        GetComponent<PlayerMovement>().enabled = false;
        playerData.anim.applyRootMotion = true;
    }

    public void StopRootMotion()
    {
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
