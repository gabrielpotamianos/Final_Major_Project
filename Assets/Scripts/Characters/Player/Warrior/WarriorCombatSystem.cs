using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;


public class WarriorCombatSystem : PlayerCombat
{
    [Space(20)]
    [Header("Warrior")]


    public Sprite ChargeSprite;


    public float ChargeAbilityTime;
    public float ChargeCooldownTime;
    public float ChargeDamageMultiplier;
    public float AddUpRage;
    public float ChargeRange;
    public bool ChargeOnCooldown = false;

    [Space(10)]

    public Sprite MortalStrikeSprite;
    public float MortalStrikeCooldownTime;
    public float MortalStrikeDamageMultiplier;
    public float MortalStrikeAbilityCost;
    public bool MortalStrikeOnCooldown = false;

    [Space(10)]
    public Sprite BladestormSprite;
    public float BladestormCooldownTime;
    public int BladestormAbilityTime;
    public float BladestormDamageMultiplier;
    public float BladestormAbilityCost;
    public float BladestormRadius;
    public LayerMask BladestormLayers;
    public bool BladestormOnCooldown = false;

    [HideInInspector]
    public static int BladestormSeconds = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        playerData.SetSpellsUI(ChargeSprite, MortalStrikeSprite, BladestormSprite);
    }

    // Update is called once per frame
    protected override void Update()
    {
        GetInput(Charge, MortalStrike, Bladestorm);
        base.Update();
    }

    #region Abilities Checks

    /// <summary>
    /// Charge Ability 
    /// </summary>
    void Charge()
    {
        if (SpellChecks.CheckSpell(Target.instance.GetCurrentTarget(), playerData, ChargeRange, ChargeOnCooldown))
        {
            StartCoroutine(ChargeCoroutine());
        }
    }

    void MortalStrike()
    {
        if (SpellChecks.CheckSpell(Target.instance.GetCurrentTarget(), playerData, Target.instance.MeleeAttackRange, MortalStrikeOnCooldown, MortalStrikeAbilityCost))
        {
            MortalStrikeStart();
        }
    }

    void Bladestorm()
    {
        if (SpellChecks.CheckSpell(playerData, BladestormOnCooldown, BladestormAbilityCost))
        {
            StartCoroutine(BladestormStart());
        }
    }

    #endregion

    



    #region Charge - Body Function

    /// <summary>
    /// <para>Warrior's First Spell </para>
    /// Description: Starts Charge Running Until Reaches Target
    /// </summary>
    /// <returns></returns>
    IEnumerator ChargeCoroutine()
    {
        SpellCheckAssigned = true;

        playerData.animator.SetBool("ChargeStart", true);

        StartCoroutine(SpellCooldown(playerData.Spell1, ChargeCooldownTime, (x) => { ChargeOnCooldown = x; }));

        while (Vector3.Distance(gameObject.transform.position, Target.instance.getCurrEnemy().gameObject.transform.position) > Target.instance.MeleeAttackRange)
        {
            GetComponent<Rigidbody>().position = Vector3.MoveTowards(transform.position, Target.instance.getCurrEnemy().gameObject.transform.position, Time.deltaTime * ChargeAbilityTime);
            gameObject.transform.LookAt(Target.instance.getCurrEnemy().transform, transform.up);
            yield return null;
        }

        playerData.animator.SetBool("ChargeEnd", true);
        yield return null;
    }

    /// <summary>
    /// Deals Charge Damage [Animation Event]
    /// </summary>
    void ChargeDamage()
    {
        DealDamage(Target.instance.getCurrEnemy(),ChargeDamageMultiplier);
        AddRage(AddUpRage);

    }

    /// <summary>
    /// Sets Charge Animation Off [Animation Event]
    /// </summary>
    private void ChargeEnd()
    {
        playerData.animator.SetBool("ChargeStart", false);
        playerData.animator.SetBool("ChargeEnd", false);
        SpellCheckAssigned = false;
    }

    #endregion






    #region Mortal Strike - Body Function

    /// <summary>
    /// <para>Warrior's Second Ability</para>
    /// 
    /// Description: Starts the animation of The Mortal Strike Spell + Begin Cooldown Count 
    /// </summary>
    void MortalStrikeStart()
    {
        SpellCheckAssigned = true;
        StartCoroutine(SpellCooldown(playerData.Spell2, MortalStrikeCooldownTime, (x) => { MortalStrikeOnCooldown = x; }));
        playerData.animator.SetBool("MortalStrike", true);
    }

    /// <summary>
    /// Deals Damage Mortal Strike [Animation Event]
    /// </summary>
    void MortalStrikeDamage()
    {
        DealDamage(Target.instance.getCurrEnemy(),MortalStrikeDamageMultiplier);
        playerData.UpdateSpellResource(-MortalStrikeAbilityCost);
    }

    /// <summary>
    /// Sets Mortal Strike Animation Off [Animaiton Event]
    /// </summary>
    void StopMortalStrike()
    {
        playerData.animator.SetBool("MortalStrike", false);
        SpellCheckAssigned = false;
    }


    #endregion










    #region Bladestorm - Body Function

    /// <summary>
    /// Sets The Animation On
    /// Starts Dealing Damage To Enemies in Range
    /// Does it for BladestormAbilityTime Seconds
    /// </summary>
    /// <returns></returns>
    IEnumerator BladestormStart()
    {
        StartCoroutine(SpellCooldown(playerData.Spell3, BladestormCooldownTime, (x) => { BladestormOnCooldown = x; }));
        SpellCheckAssigned = true;
        BladestormSeconds = 0;
        playerData.animator.SetBool("BladeStormStart", true);


        while (BladestormSeconds < BladestormAbilityTime)
        {            
            BladestormSeconds++;
            playerData.animator.SetInteger("BladeStormLoop", BladestormSeconds);
            AOE_Bladestorm_Damage();
            yield return new WaitForSeconds(1);

        }
    }


    /// <summary>
    /// Sets Animation Off
    /// Resets the counter
    /// </summary>
    void EndBladestorm()
    {
        playerData.animator.SetInteger("BladeStormLoop", 0);
        playerData.animator.SetBool("BladeStormStart", false);
        SpellCheckAssigned = false;
    }


    /// <summary>
    /// Deals Damage to All Objects In Casted Sphere (BladeStorm Setup)
    /// </summary>
    void AOE_Bladestorm_Damage()
    {
        RaycastHit[] results;
        results = Physics.SphereCastAll(gameObject.transform.position, BladestormRadius, transform.forward, 1, BladestormLayers, QueryTriggerInteraction.UseGlobal);

        foreach (RaycastHit result in results)
        {
            result.collider.GetComponent<EnemyCombat>().TakeDamage(playerData.statistics.AttackPower * BladestormDamageMultiplier);
        }

    }


    #endregion


}
