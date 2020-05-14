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

        stats.Add("MaxHealth",MaxHealth);
        stats.Add("HealthRegenerationPercentage",HealthRegenerationPercentage);
        stats.Add("AttackPower",AttackPower);
        stats.Add("HitChance",HitChance);
        stats.Add("Defence",Defence);
        stats.Add("Armour",Armour);
        stats.Add("Dodge",Dodge);
        stats.Add("Strength", Strength);
        stats.Add("Agility", Agility);
        stats.Add("Intellect", Intellect);
        stats.Add("Stamina", Stamina);
        stats.Add("Spirit", Spirit);
        stats.Add("CriticalStrike", CriticalStrike);
        stats.Add("AttackSpeed", AttackSpeed);
        stats.Add("Parry", Parry);
        stats.Add("CurrentSpellResource", CurrentSpellResource);
        stats.Add("MaxSpellResource", MaxSpellResource);
        stats.Add("AbilityRegenerationRate", AbilityRegenerationRate);

        return stats;
    }

}