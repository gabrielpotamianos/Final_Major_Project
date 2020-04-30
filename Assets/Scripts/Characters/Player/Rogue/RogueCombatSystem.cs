﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueCombatSystem : PlayerCombat
{

    [Space(20)]
    [Header("Rogue")]
    [Space(10)]

    public int MaxComboPoints;
    public int ComboPoints;
    public Material ObjectMaterial;

    [Space(10)]


    public Sprite SinisterStrikeSprite;
    public float SinisterStrikeDamageMultiplier;
    public int SinisterStrikeEnergyCost;
    public float SinisterStrikeCooldownTime;
    public int SinisterStrikeComboPointsToAdd;
    public bool SinisterStrikeOnCooldown = false;

    [Space(10)]

    public Sprite EviscerateSprite;
    public float EviscerateDamageMultiplier;
    public int EviscerateEnergyCost;
    public float EviscerateCooldownTime;
    public bool EviscerateOnCooldown = false;


    [Space(10)]
    public Sprite VanishSprite;
    public GameObject SmokePrefab;
    public float VanishTime;
    public float VanishDamageMultiplier;
    public float VanishCooldownTime;
    public bool VanishOnCooldown = false;
    IEnumerator VanishCoroutine;
    bool stealth = false;


    public float DirectionDotProductThreshold;
    public float PositionDotProductThreshold;
    public float BackstabDistanceTreshold;



    [Space(10)]
    public Sprite BackstabSprite;
    public float BackstabEnergyCost;
    public float BackstabCooldownTime;
    public float BackstabDamageMultiplier;
    public int BackstabComboPointsToAdd;
    public bool BackstabOnCooldown = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        playerData.SetSpellsUI(SinisterStrikeSprite, EviscerateSprite, VanishSprite, BackstabSprite);
    }

    // Update is called once per frame
    protected override void Update()
    {
        GetInput(SinisterStrike, Eviscerate, Vanish, Backstab);
        base.Update();
    }



    void SinisterStrike()
    {
        if (SpellChecks.CheckSpell(Target.instance.getCurrEnemy(), playerData, Target.instance.MeleeAttackRange, SinisterStrikeOnCooldown, SinisterStrikeEnergyCost))
        {
            SinisterStrikeStart();
            StartCoroutine(SpellCooldown(playerData.Spell1, SinisterStrikeCooldownTime, (x) => { SinisterStrikeOnCooldown = x; }));
        }
    }

    void Eviscerate()
    {
        if (SpellChecks.CheckSpell(Target.instance.getCurrEnemy(), playerData, Target.instance.MeleeAttackRange, EviscerateOnCooldown, EviscerateEnergyCost, ComboPoints))
        {
            EviscerateStart();
            StartCoroutine(SpellCooldown(playerData.Spell2, EviscerateCooldownTime, (x) => { EviscerateOnCooldown = x; }));
        }
    }

    void Vanish()
    {
        if (SpellChecks.CheckSpell(VanishOnCooldown, "Spell on cooldown!") &&
        SpellChecks.CheckSpell(InCombat, "You cannot use this in combat!")
        && SpellChecks.CheckSpell(stealth, "You are invisible already!"))
        {
            StartCoroutine(VanishStart());
            StartCoroutine(SpellCooldown(playerData.Spell3, VanishCooldownTime, (x) => { VanishOnCooldown = x; }));
        }
    }

    void Backstab()
    {
        if (SpellChecks.CheckSpell(Target.instance.getCurrEnemy(), playerData, DirectionDotProductThreshold, PositionDotProductThreshold, BackstabDistanceTreshold, BackstabOnCooldown, BackstabEnergyCost, stealth))
        {
            BackstabStart();
            StartCoroutine(SpellCooldown(playerData.Spell4, BackstabCooldownTime, (x) => { BackstabOnCooldown = x; }));
        }
    }

    /// <summary>
    /// Callback sent to all game objects before the application is quit.
    /// </summary>
    void OnApplicationQuit()
    {
        if (stealth) ToogleInvisibility();
    }





    #region Sinister Strike - Body Functions

    /// <summary>
    /// Starts the Sinister Strike Animation
    /// </summary>
    void SinisterStrikeStart()
    {
        if (stealth) ToogleInvisibility();
        SpellCheckAssigned = true;
        playerData.animator.SetBool("SinisterStrike", true);
        playerData.UpdateSpellResource(-SinisterStrikeEnergyCost);
    }

    /// <summary>
    /// Deals Damage
    /// </summary>
    void SinisterStrikeDamage()
    {
        DealDamage(Target.instance.getCurrEnemy(), SinisterStrikeDamageMultiplier);
    }

    /// <summary>
    /// Resets all Animations and Add ComboPoints
    /// </summary>
    void SinisterStrikeEnd()
    {
        playerData.animator.SetBool("SinisterStrike", false);
        ComboPoints += ComboPoints < MaxComboPoints ? SinisterStrikeComboPointsToAdd : 0;
        SpellCheckAssigned = false;
    }


    #endregion

    #region Eviscerate - Body Functions

    /// <summary>
    /// Eviscerate - Animation Start + Energy Consume
    /// </summary>
    void EviscerateStart()
    {
        if (stealth) ToogleInvisibility();
        playerData.animator.SetBool("Eviscerate", true);
        playerData.UpdateSpellResource(-EviscerateEnergyCost);
        SpellCheckAssigned = true;
    }

    /// <summary>
    /// Eviscerate - Deal Damage
    /// </summary>
    void EviscerateDamage()
    {
        DealDamage(Target.instance.getCurrEnemy(), EviscerateDamageMultiplier * ComboPoints);
    }

    /// <summary>
    /// Eviscerate - Set Animation Off + Comsume Combo Points
    /// </summary>
    void EviscerateEnd()
    {
        playerData.animator.SetBool("Eviscerate", false);
        ComboPoints = 0;
        SpellCheckAssigned = false;
    }

    #endregion

    #region Vanish - Body Functions

    IEnumerator VanishStart()
    {
        SpellCheckAssigned = true;
        playerData.animator.SetBool("Vanish", true);

        yield return new WaitForSeconds(0.5f);

        GameObject SmokeParticle = Instantiate(SmokePrefab, gameObject.transform.position, Quaternion.identity);
        StartCoroutine(DestroyParticle(SmokeParticle.GetComponent<ParticleSystem>()));
        if (!stealth) ToogleInvisibility();


        yield return new WaitForSeconds(VanishTime);

        if (stealth) ToogleInvisibility();

    }

    IEnumerator DestroyParticle(ParticleSystem Particle)
    {
        yield return new WaitForSeconds(Particle.main.startLifetime.constant);
        Destroy(Particle.gameObject);
    }

    void ToogleInvisibility()
    {
        stealth = !stealth;
        ObjectMaterial.SetFloat("_Transparency", (stealth ? 0.15f : 1));
    }

    public void VanishEnd()
    {
        playerData.animator.SetBool("Vanish", false);
        SpellCheckAssigned = false;
    }


    #endregion

    #region Backstab

    void BackstabStart()
    {
        SpellCheckAssigned = true;
        if (stealth) ToogleInvisibility();
        playerData.UpdateSpellResource(-BackstabEnergyCost);
        playerData.animator.SetBool("Backstab", true);
    }

    void BackstabDamage()
    {
        DealDamage(Target.instance.getCurrEnemy(), VanishDamageMultiplier * BackstabDamageMultiplier);
    }

    void BackstabEnd()
    {
        ComboPoints += BackstabComboPointsToAdd;
        playerData.animator.SetBool("Backstab", false);
        SpellCheckAssigned = false;
    }

    #endregion

}
