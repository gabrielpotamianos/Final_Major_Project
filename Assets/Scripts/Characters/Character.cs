using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public abstract class Character : MonoBehaviour
{
    public bool Alive;

    public GameObject DamageTextPrefab;
    public GameObject HealthBar;

    public Animator anim;

    public float CombatCooldownTime;

    public bool InCombat = false;
    public bool IsRegenHealth = false;


    protected IEnumerator HealthRegenCoroutine;
    protected IEnumerator InCombatCoroutine;





    public virtual void Awake()
    {
        HealthRegenCoroutine = null;
        InCombatCoroutine = null;
        anim = GetComponent<Animator>();
    }


    public bool IsItAlive(float health,float MaxHealth)
    {
        if(MaxHealth>health && !InCombat) IsRegenHealth=true;
        return Alive = health > 0;
    }

    public void Die()
    {
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;

    }

    protected abstract IEnumerator RegenHealth();
    protected abstract IEnumerator CombatCooldown(float time);
    public abstract void TakeDamage(float damage);

    protected abstract void HealthRecharge(float RechargeValue);
    protected abstract void UpdateBar(GameObject bar, float value);
    protected abstract void ShowUpDamageText(float Damage);
    public abstract void ResetCombatCoroutine();

}
