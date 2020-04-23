using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : CharacterStats
{
    [HideInInspector]
    public bool AbleToLoot;

    public GameObject HealthBar;
    public GameObject AbilityResourceBar;

    //A.R. - Ability Resource
    public float currAR = 100;
    public float RegenRateAR = 1.75f;
    public float RegenDelayAR = 1;
    public float RegenDelayHealth = 2;
    float MaxAR = 100;

    bool IsRegenAR = true;
    bool IsRegenHealth = false;
    public bool InCombat = false;

    private IEnumerator HealthRegenCoroutine;
    private IEnumerator InCombatCoroutine;






    //
    // Combat Data
    //
    public GameObject SpellBar;
    public List<Image> SpellImages;
    public List<Image> SpellCooldownImages;




    Text AbilityResourceBarName;












    public override void Awake()
    {
        HealthRegenCoroutine = null;
        InCombatCoroutine = null;
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        HealthBar = GameObject.Find("PlayerHealthSlider");
        AbilityResourceBar = GameObject.Find("PlayerAbilitySlider");
        AbilityResourceBarName = GameObject.Find("AbilitySliderText").GetComponent<Text>();
        //SpellBar = GameObject.FindGameObjectWithTag("SpellBar");
        // SpellImages.AddRange(SpellBar.GetComponentsInChildren<Image>());
        //SpellCooldownImages.AddRange(SpellImages.)
        switch (CharacterSelection.ChosenCharacter.breed)
        {
            case CharacterInfo.Breed.Mage:
                {
                    AbilityResourceBar.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.blue + new Color(0.4f, 0.4f, 0);
                    AbilityResourceBarName.text = "Mana";
                }
                break;
            case CharacterInfo.Breed.Rogue:
                {
                    AbilityResourceBar.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.yellow;
                    AbilityResourceBarName.text = "Energy";
                }
                break;
            case CharacterInfo.Breed.Warrior:
                {
                    AbilityResourceBar.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.red;
                    currAR = 0;
                    AbilityResourceBarName.text = "Rage";
                }
                break;
            default:
                break;
        }

    }


    // Update is called once per frame
    public override void Update()
    {
        print(InCombat + " this is the state of In combat ");

        base.Update();
        if (defaultStats.Alive)
        {
            if (IsRegenAR)
                StartCoroutine(RegenAR(CharacterSelection.ChosenCharacter.breed == CharacterInfo.Breed.Warrior));

            if (InCombat && InCombatCoroutine == null)
            {
                InCombatCoroutine = CombatCooldown(5);
                StartCoroutine(InCombatCoroutine);
            }
            else if (IsRegenHealth && HealthRegenCoroutine == null)
            {
                HealthRegenCoroutine = RegenHealth();
                StartCoroutine(HealthRegenCoroutine);
            }


        }
        UpdateBar(AbilityResourceBar, currAR / 100.0f);
        UpdateBar(HealthBar, defaultStats.Health / 100.0f);
        anim.SetFloat("Health", defaultStats.Health);

    }





    public void TakeDamage(float dmg)
    {
        if (HealthRegenCoroutine != null)
            StopCoroutine(HealthRegenCoroutine);
        if (InCombatCoroutine != null)
            StopCoroutine(InCombatCoroutine);

        //Activate combate mode if you take damage
        InCombat = true;
        

        //update health
        defaultStats.Health -= defaultStats.Health - dmg >= 0 ? dmg : defaultStats.Health;
    }

    public void ConsumeAR(float cost)
    {
        currAR -= currAR - cost >= 0 ? cost : currAR;
    }

    public void AddAR(float Rage)
    {
        if (gameObject.tag == CharacterInfo.Breed.Warrior.ToString())
            currAR += currAR + Rage <= Constants.WARRIOR_MAX_RAGE ? Rage : Constants.WARRIOR_MAX_RAGE - currAR;
    }


    IEnumerator RegenAR(bool IsItWarrior)
    {
        IsRegenAR = false;

        if (!IsItWarrior)
        {
            while (currAR < MaxAR)
            {
                yield return new WaitForSeconds(RegenDelayAR);
                AbilityResourceRecharge(RegenRateAR * 100 / 100);
            }
        }
        else
        {
            while (currAR > 0)
            {
                yield return new WaitForSeconds(RegenDelayAR);
                AbilityResourceDischarge(Constants.WARRIOR_DISCHARGE_RATE);
            }
        }
        IsRegenAR = true;
    }



    IEnumerator RegenHealth()
    {
        print("got here");
        while (defaultStats.Health < defaultStats.maxHealth)
        {
            yield return new WaitForSeconds(RegenDelayHealth);
            HealthRecharge(defaultStats.maxHealth / 100);
        }
        IsRegenHealth = false;
        HealthRegenCoroutine = null;
    }

    IEnumerator CombatCooldown(float time)
    {
        yield return new WaitForSeconds(time);

        InCombat = false;
        //Activate health regen if you take damage
        IsRegenHealth = true;

        InCombatCoroutine = null;

    }


    public void UpdateBar(GameObject bar, float value)
    {
        bar.GetComponent<Slider>().value = value;
    }

    public void AbilityResourceRecharge(float RechargeValue)
    {
        currAR += currAR + RechargeValue <= 100 ? RechargeValue : MaxAR - currAR;
    }

    public void AbilityResourceDischarge(float DischargeValue)
    {
        currAR -= currAR - DischargeValue > 0 ? DischargeValue : currAR;
    }

    public void HealthRecharge(float RechargeValue)
    {
        defaultStats.Health += defaultStats.Health + RechargeValue <= defaultStats.maxHealth ? RechargeValue : defaultStats.maxHealth - defaultStats.Health;
    }



    public void ToogleLoot()
    {
        if (AbleToLoot)
        {
            anim.SetBool("Looting", !anim.GetBool("Looting"));
            GetComponent<PlayerMovement>().enabled = !anim.GetBool("Looting");
            Target.instance.enabled = !anim.GetBool("Looting");
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            Inventory.instance.inventory.SetActive(anim.GetBool("Looting"));
        }
    }



    #region TEST METHODS

    public void DealDMGToPlayer(float dmg)
    {
        TakeDamage(dmg);
    }

    public void ConsumeEnergy1(float energy)
    {
        ConsumeAR(energy);
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

    public const float minValue = 0;
    public const float maxValue = 100;

    #endregion
}

