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

    [TextArea]
    public string SinisterStrikeDescription;


    [Space(10)]

    public Sprite EviscerateSprite;
    public float EviscerateDamageMultiplier;
    public int EviscerateEnergyCost;
    public float EviscerateCooldownTime;
    public bool EviscerateOnCooldown = false;

    [TextArea]
    public string EviscerateDescription;


    [Space(10)]
    public Sprite VanishSprite;
    public GameObject SmokePrefab;
    public float VanishTime;
    public float VanishDamageMultiplier;
    public float VanishCooldownTime;
    public bool VanishOnCooldown = false;
    IEnumerator VanishCoroutine;
    public static bool stealth = false;

    [TextArea]
    public string VanishDescription;



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

    [TextArea]
    public string BackstabDescription;



    float stealthDamage = 0;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        playerData.SetSpellsUI(SinisterStrikeSprite, EviscerateSprite, VanishSprite, BackstabSprite);

        SinisterStrikeDescription = SinisterStrikeDescription.Replace("AmountOfDamage", ((int)SinisterStrikeDamageMultiplier).ToString());
        SinisterStrikeDescription = SinisterStrikeDescription.Replace("AmountOfCost", SinisterStrikeEnergyCost.ToString());

        EviscerateDescription = EviscerateDescription.Replace("AmountOfDamage", ((int)EviscerateDamageMultiplier).ToString());
        EviscerateDescription = EviscerateDescription.Replace("AmountOfCost", EviscerateEnergyCost.ToString());

        VanishDescription = VanishDescription.Replace("AmountOfDamage", ((int)VanishDamageMultiplier).ToString());
        VanishDescription = VanishDescription.Replace("AmountOfSeconds", VanishTime.ToString());

        BackstabDescription = BackstabDescription.Replace("AmountOfDamage", ((int)BackstabDamageMultiplier).ToString());
        BackstabDescription = BackstabDescription.Replace("AmountOfCost", BackstabEnergyCost.ToString());


        AssignSpellsOnButtons(SinisterStrike, SinisterStrikeDescription, Eviscerate, EviscerateDescription, Vanish, VanishDescription, Backstab, BackstabDescription);
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (playerData.Alive)
            GetInput(SinisterStrike, Eviscerate, Vanish, Backstab);
        base.Update();
    }



    void SinisterStrike()
    {
        if (SpellChecks.CheckSpell(Target.instance.GetCurrentTarget(), playerData, Target.instance.MeleeAttackRange,
        SinisterStrikeOnCooldown, SinisterStrikeEnergyCost))
        {
            SinisterStrikeStart();
            StartCoroutine(SpellCooldown(playerData.Spell1, SinisterStrikeCooldownTime, (x) => { SinisterStrikeOnCooldown = x; }));
        }
    }

    void Eviscerate()
    {
        if (SpellChecks.CheckSpell(Target.instance.GetCurrentTarget(), playerData, Target.instance.MeleeAttackRange,
        EviscerateOnCooldown, EviscerateEnergyCost, ComboPoints))
        {
            EviscerateStart();
            StartCoroutine(SpellCooldown(playerData.Spell2, EviscerateCooldownTime, (x) => { EviscerateOnCooldown = x; }));
        }
    }

    void Vanish()
    {
        if (SpellChecks.CheckSpell(VanishOnCooldown, "Spell on cooldown!")
        && SpellChecks.CheckSpell(stealth, "You are invisible already!")
        && SpellChecks.CheckSpell(SpellCheckAssigned, "You cannot use that now!")
        && SpellChecks.CheckSpell(!PlayerMovement.instance.OnGround, "You cannot do that now!"))
        {
            StartCoroutine(VanishStart());
            StartCoroutine(SpellCooldown(playerData.Spell3, VanishCooldownTime, (x) => { VanishOnCooldown = x; }));
        }
    }

    void Backstab()
    {
        if (SpellChecks.CheckSpell(Target.instance.GetCurrentTarget(), playerData, DirectionDotProductThreshold,
         PositionDotProductThreshold, BackstabDistanceTreshold, BackstabOnCooldown, BackstabEnergyCost, stealth))
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
        stealthDamage = stealth ? VanishDamageMultiplier : 0;

        if (stealth) ToogleInvisibility();
        SpellCheckAssigned = true;
        playerData.animator.SetBool("SinisterStrike", true);
        playerData.UpdateSpellResource(-SinisterStrikeEnergyCost);
        SpellResourceRegen = true;

    }

    /// <summary>
    /// Deals Damage
    /// </summary>
    void SinisterStrikeDamage()
    {
        DealDamage(Target.instance.getCurrEnemy(), SinisterStrikeDamageMultiplier + stealthDamage, AddComboPoints, SinisterStrikeComboPointsToAdd);
    }

    /// <summary>
    /// Resets all Animations and Add ComboPoints
    /// </summary>
    void SinisterStrikeEnd()
    {
        playerData.animator.SetBool("SinisterStrike", false);
        SpellCheckAssigned = false;
    }


    #endregion

    #region Eviscerate - Body Functions

    /// <summary>
    /// Eviscerate - Animation Start + Energy Consume
    /// </summary>
    void EviscerateStart()
    {
        stealthDamage = stealth ? VanishDamageMultiplier : 0;

        if (stealth) ToogleInvisibility();
        playerData.animator.SetBool("Eviscerate", true);
        playerData.UpdateSpellResource(-EviscerateEnergyCost);
        SpellResourceRegen = true;
        SpellCheckAssigned = true;
    }


    /// <summary>
    /// Eviscerate - Deal Damage
    /// </summary>
    void EviscerateDamage()
    {
        DealDamage(Target.instance.getCurrEnemy(), EviscerateDamageMultiplier * ComboPoints + stealthDamage, UseComboPoints, ComboPoints);
    }

    /// <summary>
    /// Eviscerate - Set Animation Off + Comsume Combo Points
    /// </summary>
    void EviscerateEnd()
    {
        playerData.animator.SetBool("Eviscerate", false);
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
        stealthDamage = stealth ? VanishDamageMultiplier : 0;

        if (stealth) ToogleInvisibility();
        playerData.UpdateSpellResource(-BackstabEnergyCost);
        SpellResourceRegen = true;
        playerData.animator.SetBool("Backstab", true);
    }

    void BackstabDamage()
    {
        DealDamage(Target.instance.getCurrEnemy(), stealthDamage + BackstabDamageMultiplier, AddComboPoints, BackstabComboPointsToAdd);
    }

    void BackstabEnd()
    {
        playerData.animator.SetBool("Backstab", false);
        SpellCheckAssigned = false;
    }

    #endregion


    public void AddComboPoints(int combopoints)
    {
        ComboPoints += combopoints;
        ComboPoints = Mathf.Clamp(ComboPoints, 0, MaxComboPoints);
        DisplayDamageText(ComboPoints.ToString() + " Combo", Color.yellow);
    }

    public void UseComboPoints(int ComboPointsToUse)
    {
        DisplayDamageText(ComboPointsToUse.ToString() + " Combo used", Color.yellow);

        ComboPoints -= ComboPointsToUse;
        ComboPoints = Mathf.Clamp(ComboPoints, 0, MaxComboPoints);
    }


}
