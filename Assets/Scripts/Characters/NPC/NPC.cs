using UnityEngine;

public class NPC : CharacterData
{
    public BaseStatistics statistics;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (HealthBar)
            UpdateBar(HealthBar, statistics.CurrentHealth / statistics.MaxHealth);

    }
}