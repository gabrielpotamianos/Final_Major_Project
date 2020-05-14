using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : CharacterData
{
    GameObject SpellBar;
    Slider AbilityResourceBar;
    Text AbilityResourceBarName;


    public Statistics statistics;
    public bool AbleToLoot;

    public Image Spell1;
    public Image Spell2;
    public Image Spell3;
    public Image Spell4;

    public float gold;

    public static PlayerData instance;

    protected override void Awake()
    {
        base.Awake();



    }

    void Start()
    {
        if (instance != null)
            Debug.LogError("More than one PlayerData Instances!");
        else instance = this;

        Spell1 = GameObject.Find(Constants.FIRST_SPELL).transform.GetChild(0).GetComponent<Image>();
        Spell2 = GameObject.Find(Constants.SECOND_SPELL).transform.GetChild(0).GetComponent<Image>();
        Spell3 = GameObject.Find(Constants.THIRD_SPELL).transform.GetChild(0).GetComponent<Image>();
        Spell4 = GameObject.Find(Constants.FORTH_SPELL).transform.GetChild(0).GetComponent<Image>();

        HealthBar = GameObject.Find("PlayerHealthSlider").GetComponent<Slider>();
        AbilityResourceBar = GameObject.Find("PlayerAbilitySlider").GetComponent<Slider>();
        AbilityResourceBarName = GameObject.Find("AbilitySliderText").GetComponent<Text>();
        GameObject.Find("PlayerNameText").GetComponent<Text>().text = Name;

        switch (CharacterSelection.ChosenCharacter.breed)
        {
            case CharacterInfo.Breed.Mage:
                PlayerData.instance.GetAbilityResouce().transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.blue + new Color(0.4f, 0.4f, 0);

                PlayerData.instance.GetAbilityResouce().transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.blue + new Color(0.4f, 0.4f, 0);
                PlayerData.instance.GetAbilityResouceText().text = "Mana";


                break;
            case CharacterInfo.Breed.Rogue:
                PlayerData.instance.GetAbilityResouce().transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.yellow;
                {
                    PlayerData.instance.GetAbilityResouce().transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.yellow;
                    PlayerData.instance.GetAbilityResouceText().text = "Energy";
                }
                break;
            case CharacterInfo.Breed.Warrior:
                PlayerData.instance.GetAbilityResouce().transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.red;
                PlayerData.instance.statistics.CurrentSpellResource = 0;
                {
                    PlayerData.instance.GetAbilityResouce().transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.red;
                    PlayerData.instance.statistics.CurrentSpellResource = 0;
                    PlayerData.instance.GetAbilityResouceText().text = "Rage";
                }
                break;
            default:
                break;
        }
    }

    void Update()
    {
        UpdateBar(AbilityResourceBar, statistics.CurrentSpellResource / statistics.MaxSpellResource);
        UpdateBar(HealthBar, statistics.CurrentHealth / statistics.MaxHealth);
        animator.SetFloat("Health", statistics.CurrentHealth);
    }

    public void UpdateCurrentHealth(float Health)
    {
        statistics.CurrentHealth += Health;
        statistics.CurrentHealth = Mathf.Clamp(statistics.CurrentHealth, 0, statistics.MaxHealth);

    }

    public void UpdateSpellResource(float Value)
    {
        statistics.CurrentSpellResource += Value;
        statistics.CurrentSpellResource = Mathf.Clamp(statistics.CurrentSpellResource, 0, statistics.MaxSpellResource);
    }

    public void ToogleLoot(bool looting)
    {
        AbleToLoot = looting;
        animator.SetBool("Looting", AbleToLoot);
        if (AbleToLoot)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            PlayerInventory.instance.ShowInventory();
        }
        else PlayerInventory.instance.HideInventory();

    }

    public void DisplaySpellsUI()
    {
        Spell1.gameObject.transform.parent.gameObject.SetActive(!(Spell1.gameObject.transform.parent.GetComponent<Image>().sprite == null && Spell1.sprite == null));
        Spell2.gameObject.transform.parent.gameObject.SetActive(!(Spell2.gameObject.transform.parent.GetComponent<Image>().sprite == null && Spell2.sprite == null));
        Spell3.gameObject.transform.parent.gameObject.SetActive(!(Spell3.gameObject.transform.parent.GetComponent<Image>().sprite == null && Spell3.sprite == null));
        Spell4.gameObject.transform.parent.gameObject.SetActive(!(Spell4.gameObject.transform.parent.GetComponent<Image>().sprite == null && Spell4.sprite == null));

    }

    public void SetSpellsUI(Sprite Ability1, Sprite Ability2, Sprite Ability3, Sprite Ability4 = null)
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

    public bool IsItemAffordable(float sum)
    {
        return gold - sum >= 0;
    }

    public void UpdateGold(float sum)
    {
        gold += sum;
    }

    public Slider GetAbilityResouce()
    {
        return AbilityResourceBar;
    }

    public Text GetAbilityResouceText()
    {
        return AbilityResourceBarName;
    }

}
