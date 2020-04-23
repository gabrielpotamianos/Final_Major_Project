using System.Collections;
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
    public override void Start()
    {
        base.Start();
        SetSpellsUI(SinisterStrikeSprite, EviscerateSprite, VanishSprite,BackstabSprite);
    }

    // Update is called once per frame
    public override void Update()
    {
        GetInput(SinisterStrike, Eviscerate, Vanish,Backstab);
        base.Update();
    }



    void SinisterStrike()
    {
        if (PlayerUtilities.CheckSpell(Target.instance.getCurrEnemy(), playerData, Target.instance.MeleeAttackRange, SinisterStrikeOnCooldown, SinisterStrikeEnergyCost))
        {
            SinisterStrikeStart();
            StartCoroutine(SpellCooldown(Spell1, SinisterStrikeCooldownTime, (x) => { SinisterStrikeOnCooldown = x; }));
        }
    }

    void Eviscerate()
    {
        if (PlayerUtilities.CheckSpell(Target.instance.getCurrEnemy(), playerData, Target.instance.MeleeAttackRange, EviscerateOnCooldown, EviscerateEnergyCost, ComboPoints))
        {
            EviscerateStart();
            StartCoroutine(SpellCooldown(Spell2, EviscerateCooldownTime, (x) => { EviscerateOnCooldown = x; }));
        }
    }

    void Vanish()
    {
        if (!stealth && !VanishOnCooldown && !playerData.InCombat)
        {
            StartCoroutine(VanishStart());
            StartCoroutine(SpellCooldown(Spell3, VanishCooldownTime, (x) => { VanishOnCooldown = x; }));
        }
    }

    void Backstab()
    {
        if (PlayerUtilities.CheckSpell(Target.instance.getCurrEnemy(), playerData, DirectionDotProductThreshold, PositionDotProductThreshold, BackstabDistanceTreshold, BackstabOnCooldown, BackstabEnergyCost,stealth))
        {
            BackstabStart();
            StartCoroutine(SpellCooldown(Spell4, BackstabCooldownTime, (x) => { BackstabOnCooldown = x; }));
        }
    }

    /// <summary>
    /// Callback sent to all game objects before the application is quit.
    /// </summary>
    void OnApplicationQuit()
    {
        if(stealth) ToogleInvisibility();
    }





    #region Sinister Strike - Body Functions

    /// <summary>
    /// Starts the Sinister Strike Animation
    /// </summary>
    void SinisterStrikeStart()
    {
        if (stealth) ToogleInvisibility();
        SpellCheckAssigned = true;
        playerData.anim.SetBool("SinisterStrike", true);
        playerData.ConsumeAR(SinisterStrikeEnergyCost);
    }

    /// <summary>
    /// Deals Damage
    /// </summary>
    void SinisterStrikeDamage()
    {
        DealDamageToTarget(SinisterStrikeDamageMultiplier);
    }

    /// <summary>
    /// Resets all Animations and Add ComboPoints
    /// </summary>
    void SinisterStrikeEnd()
    {
        playerData.anim.SetBool("SinisterStrike", false);
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
        playerData.anim.SetBool("Eviscerate", true);
        playerData.ConsumeAR(EviscerateEnergyCost);
        SpellCheckAssigned = true;
    }

    /// <summary>
    /// Eviscerate - Deal Damage
    /// </summary>
    void EviscerateDamage()
    {
        DealDamageToTarget(EviscerateDamageMultiplier * ComboPoints);
    }

    /// <summary>
    /// Eviscerate - Set Animation Off + Comsume Combo Points
    /// </summary>
    void EviscerateEnd()
    {
        playerData.anim.SetBool("Eviscerate", false);
        ComboPoints = 0;
        SpellCheckAssigned = false;
    }

    #endregion

    #region Vanish - Body Functions

    IEnumerator VanishStart()
    {
        SpellCheckAssigned = true;
        playerData.anim.SetBool("Vanish", true);

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
        playerData.anim.SetBool("Vanish", false);
        SpellCheckAssigned = false;
    }


    #endregion

    #region Backstab

    void BackstabStart()
    {
        SpellCheckAssigned = true;
        if (stealth) ToogleInvisibility();
        playerData.ConsumeAR(BackstabEnergyCost);
        playerData.anim.SetBool("Backstab", true);
    }

    void BackstabDamage()
    {
        DealDamageToTarget(VanishDamageMultiplier * BackstabDamageMultiplier);
    }

    void BackstabEnd()
    {
        ComboPoints += BackstabComboPointsToAdd;
        playerData.anim.SetBool("Backstab", false);
        SpellCheckAssigned = false;
    }

    #endregion

}
