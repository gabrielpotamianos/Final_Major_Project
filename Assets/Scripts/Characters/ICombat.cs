using UnityEngine;
using System.Collections;

public interface ICombat
{
    void TakeDamage(float damage);
    void DealDamage(EnemyCombat enemy,float damage);
    void ResetCombatCoroutine();
    void DisplayDamageText(float Damage);
    void Die();
    IEnumerator HealthRegen();
    IEnumerator CombatCooldown(float time);
}
