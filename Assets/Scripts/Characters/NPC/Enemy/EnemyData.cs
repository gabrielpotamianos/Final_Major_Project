using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemyData : CharacterData
{
    public float SightRange;
    public float AttackRange;
    public int PatrolFrequencyTime;
    public List<Item> AllPossibleItems;
    public List<int> ChanceOfItemDrop;

    public BaseStatistics statistics;
    public GameObject CentrePatrolPointPatrol;
    public NavMeshAgent agent;


    public FiniteStateMachine FSMMachine;
    public float rangeSphere = 10.0f;
    public bool Hostile;
    public PlayerCombat playerCombat;

    public Looting LootInventory;

    bool CanLoot = false;

    public static EnemyData CurrentLootingEnemy;

    protected override void Awake()
    {
        CanvasRoot = GameObject.Find("CanvasHUD").GetComponent<Canvas>();
        LootInventory = GameObject.FindObjectOfType<Looting>();
        agent = GetComponent<NavMeshAgent>();
        base.Awake();

    }

    void Start()
    {
    }

    void Update()
    {
        if (playerCombat == null)
            playerCombat = GameObject.FindObjectOfType<PlayerCombat>();
        if (HealthBar)
            UpdateBar(HealthBar, statistics.CurrentHealth / statistics.MaxHealth);
        animator.SetFloat("Health", statistics.CurrentHealth);


        if (Input.GetKeyDown(KeyCode.L) && CanLoot)
        {
            if (this != CurrentLootingEnemy)
            {
                LootInventory.AddLootingItems(ref AllPossibleItems, ref ChanceOfItemDrop);
                CurrentLootingEnemy = this;
            }
            if (LootInventory.visible) LootInventory.HideInventory();
            else if (!LootInventory.visible) LootInventory.ShowInventory();
            PlayerData.instance.ToogleLoot(LootInventory.visible);
        }
        else if (Input.GetKeyDown(KeyCode.R) && !Alive && CanLoot)
            LootInventory.GatherAllItems(ref AllPossibleItems, ref ChanceOfItemDrop);
    }

    public void UpdateCurrentHealth(float Health)
    {
        statistics.CurrentHealth += Health;
        statistics.CurrentHealth = Mathf.Clamp(statistics.CurrentHealth, 0, statistics.MaxHealth);
    }




    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerData>() && other.GetComponent<PlayerData>().Alive)
        {
            CanLoot = true;
            other.GetComponent<PlayerData>().AbleToLoot = CanLoot;
            MessageManager.instance.DisplayMessage("Press L to Loot", 5);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerData>() && other.GetComponent<PlayerData>().Alive)
        {
            CanLoot = false;
            other.GetComponent<PlayerData>().AbleToLoot = CanLoot;
            if(LootInventory.visible)
                LootInventory.HideInventory();

            MessageManager.instance.KillMessage();
        }
    }

    public bool IsLooting()
    {
        return CanLoot;
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



}