using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemyData : CharacterData
{
    [HideInInspector]
    public int PatrolFrequencyTime;
    public float SightRange;
    public float MaxCombatDistance;
    public float AttackRange;
    public float rangeSphere = Constants.ENEMY_DATA_RANGE_SPHERE;
    public bool Hostile;

    public List<Item> AllPossibleItems;
    public List<int> ChanceOfItemDrop;

    public BaseStatistics statistics;
    public GameObject CentrePatrolPointPatrol;
    public NavMeshAgent agent;
    public FiniteStateMachine FSMMachine;
    public PlayerCombat playerCombat;
    public Looting LootInventory;


    bool CanLoot = false;

    public static EnemyData CurrentLootingEnemy;

    protected override void Awake()
    {
        PatrolFrequencyTime = Random.Range(0, Constants.ENEMY_DATA_PATROL_FREQUENCY_MAX);
        CanvasRoot = GameObject.Find("CanvasHUD").GetComponent<Canvas>();
        LootInventory = GameObject.FindObjectOfType<Looting>();
        agent = GetComponent<NavMeshAgent>();
        base.Awake();

    }


    void Update()
    {
        //Gather Player reference in update because objects are not initialized yet
        if (playerCombat == null)
            playerCombat = GameObject.FindObjectOfType<PlayerCombat>();

        //If has been targeted by the player update health bar
        if (HealthBar)
            UpdateBar(HealthBar, statistics.CurrentHealth / statistics.MaxHealth);
        animator.SetFloat("Health", statistics.CurrentHealth);

        //If has died and and L is pressed within the trigger box can loot
        if (Input.GetKeyDown(KeyCode.L) && CanLoot)
        {
            //Save the current looting enemy for the reference to other scripts
            if (this != CurrentLootingEnemy)
            {
                //Add looting items to the inventory once only
                LootInventory.AddLootingItems(ref AllPossibleItems, ref ChanceOfItemDrop);
                CurrentLootingEnemy = this;
            }

            //Toggle Invetory
            if (LootInventory.visible) LootInventory.HideInventory();
            else if (!LootInventory.visible) LootInventory.ShowInventory();
  
            //Toggle loot for player inventory and enable idle animation
            PlayerData.instance.ToogleLoot(LootInventory.visible);
        }

        // The loot inventory must be visible and the player inventory must be visible
        // for the loot to happen when pressed R
        else if (Input.GetKeyDown(KeyCode.R) && LootInventory.visible && PlayerInventory.instance.visible && !Alive && CanLoot)
            LootInventory.GatherAllItems(ref AllPossibleItems, ref ChanceOfItemDrop);
        
        //If looting inventory has been closed, close player inventory too and enable idle animation
        else if (!LootInventory.visible && CanLoot)
            PlayerData.instance.ToogleLoot(LootInventory.visible);
    }

    public void UpdateCurrentHealth(float Health)
    {
        statistics.CurrentHealth += Health;
        statistics.CurrentHealth = Mathf.Clamp(statistics.CurrentHealth, 0, statistics.MaxHealth);
    }




    private void OnTriggerEnter(Collider other)
    {
        //If within the trigger is the player
        if (other.GetComponent<PlayerData>() && other.GetComponent<PlayerData>().Alive)
        {
            //Enable looting mode
            CanLoot = true;
            MessageManager.instance.DisplayMessage(Constants.ENEMY_DATA_LOOTING_MESSAGE
            ,Constants.ENEMY_DATA_LOOTING_MESSAGE_TIME);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //If the player has quitted the trigger box
        if (other.GetComponent<PlayerData>() && other.GetComponent<PlayerData>().Alive)
        {
            //Disable looting mode
            CanLoot = false;
            if (LootInventory.visible)
                LootInventory.HideInventory();
            
            //Kill the looting message
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
            //Get a random position on the nav mesh area within the given range
            Vector3 randomPatrolPoint = center + UnityEngine.Random.insideUnitSphere * range;
            NavMeshHit hit;

            //if it hit somehting
            if (NavMesh.SamplePosition(randomPatrolPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                //return result through out parameters
                result = hit.position;
                break;
            }
        }
    }



}