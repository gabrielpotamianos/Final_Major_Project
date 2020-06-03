using UnityEngine;

public class NPC : CharacterData
{
    public BaseStatistics statistics;

    void Update()
    {
        if (HealthBar)
            UpdateBar(HealthBar, statistics.CurrentHealth / statistics.MaxHealth);

    }
}