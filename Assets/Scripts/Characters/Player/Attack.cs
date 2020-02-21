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

    Animator anim;
    Text message;
    GameObject MessagePanel;

    IEnumerator MessageClearer;

    bool AnimHasStarted = false;
    bool HasDmgBeenDealt = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        message = GameObject.Find("MessagePanel").transform.GetChild(0).GetComponent<Text>();
        MessagePanel = GameObject.Find("MessagePanel").gameObject;
        MessageClearer = ClearMessage();
        MessagePanel.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {



    }


    private void FixedUpdate()
    {
        //WHEN OTHER ANIMATIONS ARE PLAYED THIS GETS FALSE AND THE ATTACK HAPPENS
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !anim.IsInTransition(0) && !AnimHasStarted)
            AnimHasStarted = true;
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !anim.IsInTransition(0) && AnimHasStarted)
        {
            AnimHasStarted = false;
            HasDmgBeenDealt = false;
        }
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StopCoroutine(MessageClearer);

            if (Target.instance.getCurrTarget() == null)
            {
                MessagePanel.SetActive(true);
                message.text = Constants.NO_TARGET_SELECTED;
            }
            else if (!Target.instance.targetInRange)
            {
                MessagePanel.SetActive(true);
                message.text = Constants.OUT_OF_RANGE;
            }
            else if (!AnimHasStarted && !HasDmgBeenDealt)
            {
                anim.SetTrigger("BasicAttack");
                //Target.instance.getCurrEnemy().TakeDamage(GetComponent<PlayerData>().AttackPower);
                HasDmgBeenDealt = true;
            }

            MessageClearer = ClearMessage();
            StartCoroutine(MessageClearer);

        }

        if (AnimHasStarted)
        {
            anim.applyRootMotion = true;
            GetComponent<PlayerMovement>().enabled = false;
        }
        else
        {
            GetComponent<PlayerMovement>().enabled = true;
            anim.applyRootMotion = false;
        }

    }

    IEnumerator ClearMessage()
    {
        yield return new WaitForSeconds(2);
        message.text = "";
        MessagePanel.SetActive(false);
    }

    public void DealDamageToTarget()
    {
        if (Target.instance.getCurrEnemy())
        {
            Target.instance.getCurrEnemy().defaultStats.Hostile = true;
            Target.instance.getCurrEnemy().TakeDamage(GetComponent<PlayerData>().AttackPower);
        }

    }



}
