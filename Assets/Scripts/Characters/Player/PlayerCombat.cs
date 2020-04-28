using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerCombat : MonoBehaviour
{
    protected delegate void Ability();
    protected Ability CurrAbility;

    protected GameObject projectile;


    protected Image Spell1;
    protected Image Spell2;
    protected Image Spell3;
    protected Image Spell4;
    protected bool SpellCheckAssigned = false;


    [HideInInspector]
    protected PlayerData playerData;

    public void Awake()
    {
        CharacterSelection.ChosenCharacter = new CharacterInfo();
        CharacterSelection.ChosenCharacter.breed = (CharacterInfo.Breed)Enum.Parse(typeof(CharacterInfo.Breed), gameObject.tag);

        playerData = GetComponent<PlayerData>();
    }

    public virtual void Start()
    {
        Spell1 = GameObject.Find(Constants.FIRST_SPELL).transform.GetChild(0).GetComponent<Image>();
        Spell2 = GameObject.Find(Constants.SECOND_SPELL).transform.GetChild(0).GetComponent<Image>();
        Spell3 = GameObject.Find(Constants.THIRD_SPELL).transform.GetChild(0).GetComponent<Image>();
        Spell4 = GameObject.Find(Constants.FORTH_SPELL).transform.GetChild(0).GetComponent<Image>();
    }


    public void DisplaySpellsUI()
    {
        Spell1.gameObject.transform.parent.gameObject.SetActive(!(Spell1.gameObject.transform.parent.GetComponent<Image>().sprite == null && Spell1.sprite == null));
        Spell2.gameObject.transform.parent.gameObject.SetActive(!(Spell2.gameObject.transform.parent.GetComponent<Image>().sprite == null && Spell2.sprite == null));
        Spell3.gameObject.transform.parent.gameObject.SetActive(!(Spell3.gameObject.transform.parent.GetComponent<Image>().sprite == null && Spell3.sprite == null));
        Spell4.gameObject.transform.parent.gameObject.SetActive(!(Spell4.gameObject.transform.parent.GetComponent<Image>().sprite == null && Spell4.sprite == null));

    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (CurrAbility != null && !SpellCheckAssigned)
        {
            CurrAbility.Invoke();
            CurrAbility = null;
        }
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

    }

    protected void SetSpellsUI(Sprite Ability1, Sprite Ability2, Sprite Ability3, Sprite Ability4 = null)
    {
        GetComponent<Animator>().runtimeAnimatorController = Resources.Load(CharacterSelection.ChosenCharacter.breed.ToString()) as RuntimeAnimatorController;

        Spell1.gameObject.transform.parent.GetComponentInParent<Image>().sprite = Ability1;
        Spell2.gameObject.transform.parent.GetComponentInParent<Image>().sprite = Ability2;
        Spell3.gameObject.transform.parent.GetComponentInParent<Image>().sprite = Ability3;
        Spell4.gameObject.transform.parent.GetComponentInParent<Image>().sprite = Ability4 ? Ability4 : null;

        Spell1.sprite = Ability1;
        Spell2.sprite = Ability2;
        Spell3.sprite = Ability3;
        Spell4.sprite = (Ability4) ? Ability4 : null;
        DisplaySpellsUI();
    }

    protected void BasicAttack()
    {
        if (SpellChecks.CheckSpell(Target.instance.getCurrEnemy(), playerData, Target.instance.MeleeAttackRange))
        {
            playerData.anim.SetBool("BasicAttack", true);
            SpellCheckAssigned = true;
        }
    }
    protected void StopBasicAttack()
    {
        playerData.anim.SetBool("BasicAttack", false);
        SpellCheckAssigned = false;

    }


    protected void ApplyRootMotion()
    {
        GetComponent<PlayerMovement>().enabled = false;
        playerData.anim.applyRootMotion = true;
    }

    protected void StopRootMotion()
    {
        playerData.anim.applyRootMotion = false;
        GetComponent<PlayerMovement>().enabled = true;
    }


    protected void DealDamageToTarget()
    {
        Target.instance.getCurrEnemy().TakeDamage(playerData.statistics.AttackPower);
        playerData.ResetCombatCoroutine();

    }

    public void DealDamageToTarget(float Multiplier)
    {
        if (Target.instance.getCurrEnemy())
        {
            playerData.ResetCombatCoroutine();
            Target.instance.getCurrEnemy().TakeDamage(playerData.statistics.AttackPower * Multiplier);
        }
    }

    protected IEnumerator SpellCooldown(Image image, float cooldonwTime, System.Action<bool> CooldownBool)
    {
        CooldownBool(true);
        float temporaryCooldownTime = cooldonwTime;
        while (temporaryCooldownTime >= 0.000f)
        {
            temporaryCooldownTime -= Time.deltaTime;
            image.fillAmount = temporaryCooldownTime / cooldonwTime;
            yield return null;
        }
        image.fillAmount = 0;


        CooldownBool(false);
    }


}
