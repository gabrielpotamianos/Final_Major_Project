using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Enemy : Character
{
    public BaseStatistics statistics;

    public GameObject CentrePatrolPointPatrol;
    public PlayerData player;

    [HideInInspector]
    public NavMeshAgent agent;

    [HideInInspector]
    public ArtificialIntelligence.FiniteStateMachine FSMMachine;

    public float rangeSphere = 10.0f;
    public bool Hostile;

    [HideInInspector]
    public Looting LootInventory;

    GameObject CanvasRoot;

    public override void Awake()
    {
        IsItAlive(statistics.Health,statistics.MaxHealth);
        CanvasRoot=GameObject.Find("CanvasHUD");
        LootInventory = GetComponent<Looting>();
        agent = GetComponent<NavMeshAgent>();
        base.Awake();
    }

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag(CharacterSelection.ChosenCharacter.breed.ToString()).GetComponent<PlayerData>();
        FSMMachine = new ArtificialIntelligence.FiniteStateMachine(this, new ArtificialIntelligence.GoTo());
    }

    public void Update()
    {
        if (IsItAlive(statistics.Health,statistics.MaxHealth))
        {
            FSMMachine.UpdateFSM();
            if (InCombat && InCombatCoroutine == null)
            {
                InCombatCoroutine = CombatCooldown(5);
                StartCoroutine(InCombatCoroutine);
            }
            else if (IsRegenHealth && HealthRegenCoroutine == null)
            {
                HealthRegenCoroutine = RegenHealth();
                StartCoroutine(HealthRegenCoroutine);
            }
        }
        else agent.isStopped = true;

        if (HealthBar)
            UpdateBar(HealthBar, statistics.Health / statistics.MaxHealth);
        anim.SetFloat("Health", statistics.Health);

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



    public override void TakeDamage(float damage)
    {
        ResetCombatCoroutine();
        ShowUpDamageText(damage);
        statistics.Health -= statistics.Health - damage >= 0 ? damage : statistics.Health;
        Hostile = true;
        if (FSMMachine != null && FSMMachine.GetCurrState() != ArtificialIntelligence.GoTo.Instance)
        {
            transform.LookAt(player.gameObject.transform);
            FSMMachine.ChangeState(ArtificialIntelligence.GoTo.Instance);
        }

    }

    public void DealDamage()
    {
        if (player)
        {
            player.TakeDamage(statistics.AttackPower);
            ResetCombatCoroutine();
        }
    }

    protected override void UpdateBar(GameObject bar, float value)
    {
        bar.GetComponent<Slider>().value = value;
    }

    protected override void ShowUpDamageText(float Damage)
    {
        Vector3 TextPosition = Camera.main.WorldToScreenPoint(transform.position + transform.up * 2) + new Vector3(Random.Range(-300, 300), 0, 0);
        GameObject DamageTextGameObject = Instantiate(DamageTextPrefab, TextPosition, Quaternion.identity, CanvasRoot.transform);
        Text DamageText = DamageTextGameObject.transform.GetChild(0).GetComponent<Text>();
        DamageText.text = Damage.ToString();
        DamageText.color = Color.yellow;
    }


    protected override IEnumerator RegenHealth()
    {
        while (statistics.Health < statistics.MaxHealth)
        {
            yield return new WaitForSeconds(Constants.TICK);
            HealthRecharge((statistics.MaxHealth * statistics.HealthRegenerationPercentage) / 100);
        }
        IsRegenHealth = false;
        HealthRegenCoroutine = null;
    }

    protected override IEnumerator CombatCooldown(float time)
    {
        yield return new WaitForSeconds(time);

        InCombat = false;
        //Activate health regen if you take damage
        IsRegenHealth = true;

        InCombatCoroutine = null;
    }

    protected override void HealthRecharge(float RechargeValue)
    {
        statistics.Health += statistics.Health + RechargeValue <= statistics.MaxHealth ? RechargeValue : statistics.MaxHealth - statistics.Health;
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
