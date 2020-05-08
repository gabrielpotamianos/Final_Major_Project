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
        enemyData.FSMMachine = new FiniteStateMachine(this, new FiniteStateMachine.GoTo());

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



    public void RandomPatrolPoint(Vector3 center, float range, out Vector3 result)
    {
        while (true)
        {
            Vector3 randomPatrolPoint = center + UnityEngine.Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPatrolPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                break;
            }
        }
    }

    public override void DealDamage()
    {
        if (enemyData.playerCombat)
        {
            enemyData.playerCombat.TakeDamage(enemyData.statistics.AttackPower);
            ResetCombatCoroutine();
        }
    }

    public override void TakeDamage(float damage)
    {

        ResetCombatCoroutine();
        DisplayDamageText(damage);
        enemyData.UpdateCurrentHealth(-damage);
        enemyData.Hostile = true;
        if (enemyData.FSMMachine != null && enemyData.FSMMachine.GetCurrState() != FiniteStateMachine.GoTo.Instance)
        {
            transform.LookAt(enemyData.playerCombat.playerData.transform);
            enemyData.FSMMachine.ChangeState(FiniteStateMachine.GoTo.Instance);
        }
    }
    public static int test = 0;
    public override void DisplayDamageText(float Damage)
    {
        Vector3 TextPosition = Camera.main.WorldToScreenPoint(transform.position + transform.up * 2) + new Vector3(UnityEngine.Random.Range(-300, 300), 0, 0);
        GameObject DamageTextGameObject = Instantiate(DamageTextPrefab, TextPosition, Quaternion.identity, enemyData.GetCanvasRoot().transform);
        Text DamageText = DamageTextGameObject.transform.GetChild(0).GetComponent<Text>();
        DamageText.text = Damage.ToString();
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
        collider.center = new Vector3(collider.center.x, 0, collider.center.z);
        collider.size = new Vector3(collider.size.x, 1, collider.size.z);
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


}
