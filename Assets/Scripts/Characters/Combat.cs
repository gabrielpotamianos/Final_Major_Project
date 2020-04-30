using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public abstract class Combat : MonoBehaviour
{
    public bool InCombat = false;
    public bool IsRegenHealth = false;
    public float CombatCooldownTime;
    public GameObject DamageTextPrefab;

    protected IEnumerator HealthRegenCoroutine;
    protected IEnumerator InCombatCoroutine;


    protected virtual void Start()
    {
        HealthRegenCoroutine = null;
        InCombatCoroutine = null;
    }

    protected abstract void Update();
    public abstract void TakeDamage(float damage);
    public abstract void DealDamage();
    public abstract void DisplayDamageText(float Damage);
    public abstract void ResetCombatCoroutine();
    public abstract void Die();
    public abstract IEnumerator CombatCooldown(float time);
    public abstract IEnumerator HealthRegen();

}