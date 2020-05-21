using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyCombat : Combat
{
    public EnemyData enemyData;


    protected void Awake()
    {
        enemyData = GetComponent<EnemyData>();
        enemyData.FSMMachine = new FiniteStateMachine(enemyData, new FiniteStateMachine.Idle());

    }

    protected override void Update()
    {
        if (enemyData.IsItAlive(enemyData.statistics.CurrentHealth, enemyData.statistics.MaxHealth))
        {
            enemyData.IsHealthRegenNeeded(ref IsRegenHealth, enemyData.statistics.CurrentHealth, enemyData.statistics.MaxHealth);
            enemyData.FSMMachine.UpdateFSM();
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


    public override void DealDamage()
    {
        if (enemyData.playerCombat)
        {
            if (enemyData.statistics.HitChance > Random.Range(0, 100))
                enemyData.playerCombat.TakeDamage(enemyData.statistics.AttackPower);
            else enemyData.playerCombat.TakeDamage(0);
            ResetCombatCoroutine();
        }
    }

    public override void TakeDamage(float damage)
    {
        if (enemyData.FSMMachine != null && enemyData.FSMMachine.GetCurrState() != FiniteStateMachine.ReturnOrigin.Instance)
        {
            ResetCombatCoroutine();

            if (damage > 0)
            {
                damage = damage * (damage / (damage + enemyData.statistics.Armour));

                DisplayDamageText(damage);
                enemyData.UpdateCurrentHealth(-damage);
                enemyData.Hostile = true;
                if (enemyData.FSMMachine.GetCurrState() != FiniteStateMachine.Chase.Instance && enemyData.FSMMachine.GetCurrState() != FiniteStateMachine.AttackState.Instance)
                    enemyData.FSMMachine.ChangeState(FiniteStateMachine.Chase.Instance);
            }
            else
            {
                DisplayDamageText("Missed");
            }
        }
    }

    public override void DisplayDamageText(float Damage)
    {
        Vector3 TextPosition = Camera.main.WorldToScreenPoint(transform.position + transform.up * 2) + new Vector3(UnityEngine.Random.Range(-300, 300), 0, 0);
        GameObject DamageTextGameObject = Instantiate(DamageTextPrefab, TextPosition, Quaternion.identity, enemyData.GetCanvasRoot().transform);
        Text DamageText = DamageTextGameObject.transform.GetChild(0).GetComponent<Text>();
        DamageText.text = Damage.ToString();
        DamageText.color = Color.yellow;
    }

    public void DisplayDamageText(string Message)
    {
        Vector3 TextPosition = Camera.main.WorldToScreenPoint(transform.position + transform.up * 2) + new Vector3(UnityEngine.Random.Range(-300, 300), 0, 0);
        GameObject DamageTextGameObject = Instantiate(DamageTextPrefab, TextPosition, Quaternion.identity, enemyData.GetCanvasRoot().transform);
        Text DamageText = DamageTextGameObject.transform.GetChild(0).GetComponent<Text>();
        DamageText.text = Message;
        DamageText.color = Color.yellow;

    }

    public override IEnumerator HealthRegen()
    {
        while (enemyData.statistics.CurrentHealth < enemyData.statistics.MaxHealth)
        {
            yield return new WaitForSeconds(Constants.TICK);
            enemyData.UpdateCurrentHealth((enemyData.statistics.MaxHealth * enemyData.statistics.HealthRegenerationPercentage) / 100);
        }
        IsRegenHealth = false;
        HealthRegenCoroutine = null;
    }

    public override IEnumerator CombatCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        InCombat = false;
        IsRegenHealth = enemyData.statistics.CurrentHealth < enemyData.statistics.MaxHealth;
        InCombatCoroutine = null;
    }

    public override void Die()
    {
        if (!enemyData.Alive && enemyData.FSMMachine.GetCurrState() != null)
        {
            enemyData.Alive = false;
            enemyData.agent.isStopped = true;
            enemyData.agent.velocity = Vector3.zero;
            enemyData.FSMMachine.ChangeState(null);
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            BoxCollider collider = GetComponent<BoxCollider>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            rigidbody.velocity = Vector3.zero;
            collider.isTrigger = true;

            //Shrinks the colliders size so you can select other targets with accuracy
            collider.center = new Vector3(collider.center.x, 1, -2);
            collider.size = new Vector3(collider.size.x, collider.size.z, collider.size.y);
        }
        //  GetComponent<Looting>().enabled = true;
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

    public void idlecoroutine()
    {
        StartCoroutine(thiscoroutine());
    }

    IEnumerator thiscoroutine()
    {

        yield return new WaitForSeconds(3);
        print("working");
    }


}
