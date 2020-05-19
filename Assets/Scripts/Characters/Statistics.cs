using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class Statistics : BaseStatistics
{
    public float Strength;
    public float Agility;
    public float Intellect;
    public float Stamina;
    public float Spirit;

    public float CriticalStrike;
    public float AttackSpeed;
    public float Parry;

    public float CurrentSpellResource;
    public float MaxSpellResource;
    public float AbilityRegenerationRate;


    public Dictionary<string, float> GetStatisticsDictionary()
    {
        Dictionary<string, float> stats = new Dictionary<string, float>();

        stats.Add("Max Health", MaxHealth);
        stats.Add("Health Regen", HealthRegenerationPercentage);
        stats.Add("Attack Power", AttackPower);
        stats.Add("Hit Chance", HitChance);
        stats.Add("Armour", Armour);
        stats.Add("Dodge", Dodge);
        stats.Add("Strength", Strength);
        stats.Add("Agility", Agility);
        stats.Add("Intellect", Intellect);
        stats.Add("Stamina", Stamina);
        stats.Add("Spirit", Spirit);
        stats.Add("Critical Strike", CriticalStrike);
        stats.Add("Attack Speed", AttackSpeed);
        stats.Add("Parry", Parry);
        stats.Add("CurrentSpellResource", CurrentSpellResource);
        stats.Add("Maximum Spell Resource", MaxSpellResource);
        stats.Add("Spell Resource Regen", AbilityRegenerationRate);

        return stats;
    }


    public void RecalculateStats()
    {
        switch (CharacterSelection.ChosenCharacter.breed)
        {
            case CharacterInfo.Breed.Mage:
                AttackPower += Intellect;
                CriticalStrike += Intellect / 59.5f;
                MaxSpellResource += Intellect * 15;
                AbilityRegenerationRate += Spirit / 4 + 12.5f;
                break;
            case CharacterInfo.Breed.Rogue:
                AttackPower += Agility + Strength;
                CriticalStrike += Agility / 29.0f;
                Dodge += Agility / 14.5f;
                AbilityRegenerationRate += 10f;

                break;
            case CharacterInfo.Breed.Warrior:
                AttackPower += Strength * 2 + Agility;
                CriticalStrike += Agility / 20.0f;
                Dodge += Agility / 20;
                break;
        }

        MaxHealth += Stamina * 10;
        Armour += Agility / 2;

    }


    public void GetBaseStats()
    {

        switch (CharacterSelection.ChosenCharacter.breed)
        {
            case CharacterInfo.Breed.Mage:
                AttackPower -= Strength + Intellect / 2;
                CriticalStrike -= Intellect / 59.5f;
                MaxSpellResource -= Intellect * 15;
                AbilityRegenerationRate -= Spirit / 4 + 12.5f;
                break;
            case CharacterInfo.Breed.Rogue:
                AttackPower -= Agility + Strength;
                CriticalStrike -= Agility / 29.0f;
                Dodge -= Agility / 14.5f;
                AbilityRegenerationRate -= 10f;
                break;
            case CharacterInfo.Breed.Warrior:
                AttackPower -= Strength * 2 + Agility;
                CriticalStrike -= Agility / 20.0f;
                Dodge -= Agility / 20;
                break;
        }
        MaxHealth -= Stamina * 10;
        Armour -= Agility / 2;


    }

}