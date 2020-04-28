using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : Character
{

    public Statistics statistics;




    public GameObject SpellBar;
    Text AbilityResourceBarName;
    [HideInInspector]
    public bool AbleToLoot;
    public GameObject AbilityResourceBar;

    //A.R. - Ability Resource
    public float currAR = 100;
    public float RegenRateAR = 1.75f;
    public float RegenDelayAR = 1;
    float MaxAR = 100;

    bool IsRegenAR = true;
    public override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {
        HealthBar = GameObject.Find("PlayerHealthSlider");
        AbilityResourceBar = GameObject.Find("PlayerAbilitySlider");
        AbilityResourceBarName = GameObject.Find("AbilitySliderText").GetComponent<Text>();
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
    public void Update()
    {
        if (IsItAlive(statistics.Health,statistics.MaxHealth))
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
        else Die();
        UpdateBar(AbilityResourceBar, currAR / MaxAR);
        UpdateBar(HealthBar, statistics.Health / statistics.MaxHealth);
        anim.SetFloat("Health", statistics.Health);

    }





    public override void TakeDamage(float damage)
    {
        ResetCombatCoroutine();
        ShowUpDamageText(damage);

        statistics.Health -= statistics.Health - damage >= 0 ? damage : statistics.Health;
    }

    protected override IEnumerator RegenHealth()
    {
        while (statistics.Health < statistics.MaxHealth)
        {
            yield return new WaitForSeconds(Constants.TICK);
            HealthRecharge((statistics.MaxHealth * statistics.HealthRegenerationPercentage) / 100);
        }
        IsRegenHealth = false;
        HealthRegenCoroutine = null;
    }

    protected override IEnumerator CombatCooldown(float time)
    {
        yield return new WaitForSeconds(time);

        InCombat = false;
        //Activate health regen if you take damage
        IsRegenHealth = true;

        InCombatCoroutine = null;

    }

    protected override void UpdateBar(GameObject bar, float value)
    {
        bar.GetComponent<Slider>().value = value;
    }

    protected override void HealthRecharge(float RechargeValue)
    {
        statistics.Health += statistics.Health + RechargeValue <= statistics.MaxHealth ? RechargeValue : statistics.MaxHealth - statistics.Health;
    }

    protected override void ShowUpDamageText(float Damage)
    {
        Vector3 TextPosition = Camera.main.WorldToScreenPoint(transform.position + transform.up * 2) + new Vector3(Random.Range(-300, 300), 0, 0);
        GameObject DamageTextGameObject = Instantiate(DamageTextPrefab, TextPosition, Quaternion.identity, HealthBar.transform.parent.transform.parent);
        Text DamageText = DamageTextGameObject.transform.GetChild(0).GetComponent<Text>();
        DamageText.text = Damage.ToString();
        DamageText.color = Color.red;

    }

    public override void ResetCombatCoroutine()
    {
        InCombat = true;
        if (HealthRegenCoroutine != null)
        {
            StopCoroutine(HealthRegenCoroutine);
            HealthRegenCoroutine = null;
        }
        if(InCombatCoroutine!=null)
            StopCoroutine(InCombatCoroutine);
        InCombatCoroutine = CombatCooldown(CombatCooldownTime);
        StartCoroutine(InCombatCoroutine);

    }


    public void AbilityResourceRecharge(float RechargeValue)
    {
        currAR += currAR + RechargeValue <= 100 ? RechargeValue : MaxAR - currAR;
    }

    public void AbilityResourceDischarge(float DischargeValue)
    {
        currAR -= currAR - DischargeValue > 0 ? DischargeValue : currAR;
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

    public void ToogleLoot()
    {
        if (AbleToLoot)
        {
            //bool AnimatorLoot=anim.GetBool("Looting");
            anim.SetBool("Looting", !anim.GetBool("Looting"));
            GetComponent<PlayerMovement>().enabled = !anim.GetBool("Looting");
            Target.instance.enabled = !anim.GetBool("Looting");
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            Inventory.instance.inventory.SetActive(anim.GetBool("Looting"));
        }
    }



}


