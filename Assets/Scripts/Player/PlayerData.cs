using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : CharacterStats
{
    public GameObject HealthBar;
    public GameObject AbilityResourceBar;

    //A.R. - Ability Resource
    public float currAR=100;
    public float RegenRateAR = 1.75f;
    public float RegenDelayAR = 1;
    public float RegenDelayHealth = 2;
    float MaxAR = 100;

    bool IsRegenAR=true;
    bool IsRegenHealth = false;
    bool InCombat = false;

    private IEnumerator HealthRegenCoroutine;
    private IEnumerator InCombatCoroutine;

    private void Awake()
    {
        HealthRegenCoroutine = RegenHealth();
        InCombatCoroutine = CombatCooldown(5);
    }

    // Update is called once per frame
    void Update()
    {

        if (IsRegenAR)
            StartCoroutine(RegenAR());

        if (InCombat)
        {
            InCombatCoroutine = CombatCooldown(5);
            StartCoroutine(InCombatCoroutine);
        }
        else if (IsRegenHealth)
        {
            HealthRegenCoroutine = RegenHealth();
            StartCoroutine(HealthRegenCoroutine);
        }

        UpdateBar(AbilityResourceBar, currAR / 100.0f);
        UpdateBar(HealthBar, currHealth / 100.0f);
    }




    public void TakeDamage(float dmg)
    {
        if(HealthRegenCoroutine!=null)
            StopCoroutine(HealthRegenCoroutine);
        if(InCombatCoroutine!=null)
            StopCoroutine(InCombatCoroutine);

        //Activate combate mode if you take damage
        InCombat = true;

        //update health
        currHealth -= currHealth - dmg >= 0 ? dmg : currHealth;
    }

    public void ConsumeEnergy(float cost)
    {
        //Activate Regen energy if you consume energy
        //IsRegenAR = true;

        //update energy
        currAR -= cost;
    }


    IEnumerator RegenAR()
    {
        IsRegenAR = false;
        while (currAR < MaxAR)
        {
            yield return new WaitForSeconds(RegenDelayAR);
            AbilityResourceRecharge(RegenRateAR * 100 / 100);
        }
        IsRegenAR = true;
    }


    IEnumerator RegenHealth()
    {

        IsRegenHealth = false;
        while (currHealth < maxHealth)
        {
            yield return new WaitForSeconds(RegenDelayHealth);
            HealthRecharge(maxHealth / 100 );
        }
    }

    IEnumerator CombatCooldown(float time)
    {
        InCombat = false;
        yield return new WaitForSeconds(time);

        //Activate health regen if you take damage
        IsRegenHealth = true;

    }


    public void UpdateBar(GameObject bar,float value)
    {
        bar.GetComponent<Slider>().value = value;
    }

    public void AbilityResourceRecharge(float RechargeValue)
    {
        currAR += currAR+RechargeValue<=100? RechargeValue : MaxAR-currAR;
    }

    public void HealthRecharge(float RechargeValue)
    {
        currHealth += currHealth + RechargeValue <= maxHealth ? RechargeValue : maxHealth-currHealth;
    }







    #region TEST METHODS

    public void DealDMGToPlayer(float dmg)
    {
        TakeDamage(dmg);
    }

    public void ConsumeEnergy1(float energy)
    {
        ConsumeEnergy(energy);
    }



    IEnumerator Corooutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            print("This coroutine is working");
        }
    }
    #endregion

}

public struct Statistics
{
    #region Variables

    public float Health;
    public float currAR;

    public float Strength;
    public float Agility;
    public float Intellect;
    public float Stamina;

    public float AttackPower;
    public float Dodge;
    public float CriticalStrike;

    //Mana & Energy
    public float AbilityRegenerationRate;
    public float HealthRegenerationRate;

    //Rage/Fury
    public float IncreaseRate;

    public const float minValue= 0;
    public const float maxValue= 100;

    #endregion
}

