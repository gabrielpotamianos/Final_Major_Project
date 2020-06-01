using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerCombat : Combat
{
    protected delegate void Ability();
    protected Ability CurrAbility;

    protected bool SpellCheckAssigned = false;
    protected bool SpellResourceRegen = true;
    IEnumerator SpellResourceRegenCoroutine;
    public PlayerData playerData;
    public float CombatAngle;

    float Rage = 15;
    Button BasicAttackButton;
    Button Spell1Button;
    Button Spell2Button;
    Button Spell3Button;
    Button Spell4Button;

    public delegate void ComboPointsCallback(int comboPoints);

    protected override void Start()
    {
        SpellChecks.CombatAngle = CombatAngle;
        base.Start();
        playerData = GetComponent<PlayerData>();
    }

    protected override void Update()
    {
        if (playerData.IsItAlive(playerData.statistics.CurrentHealth, playerData.statistics.MaxHealth))
        {
            playerData.IsHealthRegenNeeded(ref IsRegenHealth, playerData.statistics.CurrentHealth, playerData.statistics.MaxHealth);
            if (SpellResourceRegen)
            {
                if (SpellResourceRegenCoroutine != null)
                    StopCoroutine(SpellResourceRegenCoroutine);
                SpellResourceRegenCoroutine = RegenerateSpellResource(CharacterSelection.ChosenCharacter.breed == CharacterInfo.Breed.Warrior);
                StartCoroutine(SpellResourceRegenCoroutine);
            }

            if (playerData.statistics.CurrentSpellResource == 0)
                playerData.GetAbilityResouce().transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
            else if (playerData.statistics.CurrentSpellResource > 0)
                playerData.GetAbilityResouce().transform.GetChild(1).GetChild(0).gameObject.SetActive(true);


            if (CurrAbility != null && !SpellCheckAssigned)
            {
                CurrAbility.Invoke();
                CurrAbility = null;
            }

            if (InCombat && InCombatCoroutine == null)
            {
                InCombatCoroutine = this.CombatCooldown(CombatCooldownTime);
                StartCoroutine(InCombatCoroutine);
            }
            else if (IsRegenHealth && !InCombat && HealthRegenCoroutine == null)
            {
                HealthRegenCoroutine = HealthRegen();
                StartCoroutine(HealthRegenCoroutine);
            }
        }
        else Die();
    }



    public void AddRage(float Rage)
    {
        if (gameObject.tag == CharacterInfo.Breed.Warrior.ToString())
        {
            playerData.statistics.CurrentSpellResource += Rage;
            Mathf.Clamp(playerData.statistics.CurrentSpellResource, 0, Constants.WARRIOR_MAX_RAGE);
        }
    }

    public void DealDamage(EnemyCombat enemy, float damage, Action<int> AddOrUseComboPoints = null, int comboPoints = 0)
    {
        if (enemy)
        {
            ResetCombatCoroutine();
            if (playerData.statistics.HitChance > UnityEngine.Random.Range(0, 100))
                if (playerData.statistics.CriticalStrike > UnityEngine.Random.Range(0, 100))
                {
                    enemy.TakeDamage(playerData.statistics.AttackPower * damage / 100 * 2);
                    if (AddOrUseComboPoints != null)
                        AddOrUseComboPoints.Invoke(comboPoints);

                }
                else
                {
                    enemy.TakeDamage(playerData.statistics.AttackPower * damage / 100);
                    if (AddOrUseComboPoints != null)
                        AddOrUseComboPoints.Invoke(comboPoints);
                }
            else enemy.TakeDamage(0);
        }
    }

    public void DealDamageAnimationEvent()
    {
        if (playerData.statistics.HitChance > UnityEngine.Random.Range(0, 100))
            if (playerData.statistics.CriticalStrike > UnityEngine.Random.Range(0, 100))
                Target.instance.getCurrEnemy().TakeDamage(playerData.statistics.AttackPower * 2);
            else Target.instance.getCurrEnemy().TakeDamage(playerData.statistics.AttackPower);
        else Target.instance.getCurrEnemy().TakeDamage(0);
        ResetCombatCoroutine();
    }

    public override void TakeDamage(float damage)
    {
        if (damage > 0)
            if (playerData.statistics.Dodge < UnityEngine.Random.Range(0, 100))
                if (playerData.statistics.Parry < UnityEngine.Random.Range(0, 100))
                {
                    damage = damage * (damage / (damage + playerData.statistics.Armour));
                    ResetCombatCoroutine();
                    DisplayDamageText(damage);
                    playerData.UpdateCurrentHealth(-damage);
                    playerData.statistics.CurrentSpellResource += Rage / 2
                    ;
                }
                else DisplayDamageText("Blocked");
            else DisplayDamageText("Dodged");
        else DisplayDamageText("Missed");
    }

    public override void DealDamage()
    {
        if (playerData.statistics.HitChance > UnityEngine.Random.Range(0, 100))
            Target.instance.getCurrEnemy().TakeDamage(playerData.statistics.AttackPower);
        else Target.instance.getCurrEnemy().TakeDamage(0);
        ResetCombatCoroutine();

    }

    public override void DisplayDamageText(float Damage)
    {
        Vector3 TextPosition = Camera.main.WorldToScreenPoint(transform.position + transform.up * 2) + new Vector3(UnityEngine.Random.Range(-300, 300), 0, 0);
        GameObject DamageTextGameObject = Instantiate(DamageTextPrefab, TextPosition, Quaternion.identity, playerData.GetCanvasRoot().transform);
        Text DamageText = DamageTextGameObject.transform.GetChild(0).GetComponent<Text>();
        DamageText.text = "-" + Damage.ToString();
        DamageText.color = Color.red;
    }

    public void DisplayDamageText(string Message, Color? color = null)
    {
        Vector3 TextPosition = Camera.main.WorldToScreenPoint(transform.position + transform.up * 2) + new Vector3(UnityEngine.Random.Range(-300, 300), 0, 0);
        GameObject DamageTextGameObject = Instantiate(DamageTextPrefab, TextPosition, Quaternion.identity, playerData.GetCanvasRoot().transform);
        Text DamageText = DamageTextGameObject.transform.GetChild(0).GetComponent<Text>();
        DamageText.text = Message;
        DamageText.color = color ?? Color.red;
    }


    public override void ResetCombatCoroutine()
    {
        InCombat = true;
        if (HealthRegenCoroutine != null)
        {
            StopCoroutine(HealthRegenCoroutine);
            HealthRegenCoroutine = null;
        }
        if (InCombatCoroutine != null)
            StopCoroutine(InCombatCoroutine);
        InCombatCoroutine = CombatCooldown(CombatCooldownTime);
        StartCoroutine(InCombatCoroutine);
    }

    public override void Die()
    {
        playerData.Alive = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public override IEnumerator HealthRegen()
    {
        while (playerData.statistics.CurrentHealth < playerData.statistics.MaxHealth)
        {
            yield return new WaitForSeconds(Constants.TICK);
            playerData.UpdateCurrentHealth((playerData.statistics.MaxHealth * playerData.statistics.HealthRegenerationPercentage) / 100);
        }
        IsRegenHealth = false;
        HealthRegenCoroutine = null;
    }

    public override IEnumerator CombatCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        InCombat = false;
        IsRegenHealth = playerData.statistics.CurrentHealth < playerData.statistics.MaxHealth;
        InCombatCoroutine = null;
    }

    protected void BasicAttack()
    {
        if (SpellChecks.CheckSpell(Target.instance.GetCurrentTarget(), playerData, Target.instance.MeleeAttackRange))
        {
            playerData.animator.SetBool("BasicAttack", true);
            SpellCheckAssigned = true;
        }
    }

    protected void StopBasicAttack()
    {
        playerData.animator.SetBool("BasicAttack", false);
        SpellCheckAssigned = false;
    }

    protected void DisableMovement()
    {
        GetComponent<PlayerMovement>().enabled = false;
        playerData.animator.applyRootMotion = true;
    }

    protected void EnableMovement()
    {
        playerData.animator.applyRootMotion = false;
        GetComponent<PlayerMovement>().enabled = true;
    }

    protected void GetInput(Ability Spell1, Ability Spell2, Ability Spell3, Ability Spell4 = null)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && CurrAbility == null && !SpellCheckAssigned)
            CurrAbility = BasicAttack;
        else if (Input.GetKeyDown(KeyCode.Alpha2) && CurrAbility == null && !SpellCheckAssigned)
            CurrAbility = Spell1;
        else if (Input.GetKeyDown(KeyCode.Alpha3) && CurrAbility == null && !SpellCheckAssigned)
            CurrAbility = Spell2;
        else if (Input.GetKeyDown(KeyCode.Alpha4) && CurrAbility == null && !SpellCheckAssigned)
            CurrAbility = Spell3;
        else if (Spell4 != null && Input.GetKeyDown(KeyCode.Alpha5) && CurrAbility == null && !SpellCheckAssigned)
            CurrAbility = Spell4;

        SpellChecks.SpellAssigned = SpellCheckAssigned;

    }

    protected void AssignSpellsOnButtons(Ability Spell1, Ability Spell2, Ability Spell3, Ability Spell4 = null)
    {
        Spell1Button = playerData.Spell1.transform.parent.GetComponent<Button>();
        Spell2Button = playerData.Spell2.transform.parent.GetComponent<Button>();
        Spell3Button = playerData.Spell3.transform.parent.GetComponent<Button>();
        Spell4Button = playerData.Spell4.transform.parent.GetComponent<Button>();

        Spell1Button.onClick.AddListener(() => Spell1());
        Spell2Button.onClick.AddListener(() => Spell2());
        Spell3Button.onClick.AddListener(() => Spell3());
        Spell4Button.onClick.AddListener(() => Spell4());
    }

    protected void AssignSpellsOnButtons(Ability Spell1, string Spell1Description, Ability Spell2, string Spell2Description, Ability Spell3, string Spell3Description, Ability Spell4 = null, string Spell4Description = null)
    {
        Spell1Button = playerData.Spell1.transform.parent.GetComponent<Button>();
        Spell2Button = playerData.Spell2.transform.parent.GetComponent<Button>();
        Spell3Button = playerData.Spell3.transform.parent.GetComponent<Button>();
        Spell4Button = playerData.Spell4.transform.parent.GetComponent<Button>();

        playerData.Spell1.transform.parent.GetComponent<HooverOver>().Message = Spell1Description;
        playerData.Spell2.transform.parent.GetComponent<HooverOver>().Message = Spell2Description;
        playerData.Spell3.transform.parent.GetComponent<HooverOver>().Message = Spell3Description;
        playerData.Spell4.transform.parent.GetComponent<HooverOver>().Message = Spell4Description;

        Spell1Button.onClick.AddListener(() => Spell1());
        Spell2Button.onClick.AddListener(() => Spell2());
        Spell3Button.onClick.AddListener(() => Spell3());
        Spell4Button.onClick.AddListener(() => Spell4());
    }



    protected IEnumerator SpellCooldown(Image image, float cooldonwTime, System.Action<bool> CooldownBool)
    {
        CooldownBool(true);
        float temporaryCooldownTime = cooldonwTime;
        while (temporaryCooldownTime >= 0.0000f)
        {
            temporaryCooldownTime -= Time.deltaTime;
            image.fillAmount = temporaryCooldownTime / cooldonwTime;
            yield return null;
        }
        image.fillAmount = 0;
        CooldownBool(false);
    }

    IEnumerator RegenerateSpellResource(bool IsItWarrior)
    {
        SpellResourceRegen = false;
        if (!IsItWarrior)
        {
            while (playerData.statistics.CurrentSpellResource < playerData.statistics.MaxSpellResource)
            {
                yield return new WaitForSeconds(Constants.TICK);
                playerData.UpdateSpellResource(playerData.statistics.CurrentSpellResource / playerData.statistics.MaxSpellResource * playerData.statistics.AbilityRegenerationRate);
            }
        }
        else
        {
            while (playerData.statistics.CurrentSpellResource > 0)
            {
                yield return new WaitForSeconds(Constants.TICK);
                playerData.UpdateSpellResource(-Constants.WARRIOR_DISCHARGE_RATE);
            }
        }
        SpellResourceRegen = true;
    }

    public void IsSpellRegenNeeded(ref bool spellRegen, float spellResource, float MaxSpellResource, bool IsItWarrior)
    {
        if (!IsItWarrior)
            spellRegen = spellResource < MaxSpellResource;
        else spellRegen = spellResource > 0;
    }

    public bool IsPlayerCastingSpell()
    {
        return CurrAbility == null;
    }


}
