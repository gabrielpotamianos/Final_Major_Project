using UnityEngine;
using UnityEngine.AI;

public class EnemyData : CharacterData
{
    public BaseStatistics statistics;
    public GameObject CentrePatrolPointPatrol;
    public NavMeshAgent agent;


    public FiniteStateMachine FSMMachine;
    public float rangeSphere = 10.0f;
    public bool Hostile;
    public PlayerCombat playerCombat;
    [HideInInspector]
    public Looting LootInventory;

    protected override void Awake()
    {
        CanvasRoot = GameObject.Find("CanvasHUD").GetComponent<Canvas>();
        LootInventory = GetComponent<Looting>();
        agent = GetComponent<NavMeshAgent>();
        base.Awake();

    }

    void Start()
    {
    }

    void Update()
    {
        if(playerCombat==null)
            playerCombat=GameObject.FindObjectOfType<PlayerCombat>();
        if (HealthBar)
            UpdateBar(HealthBar, statistics.CurrentHealth / statistics.MaxHealth);
        animator.SetFloat("Health", statistics.CurrentHealth);

    }

    public void UpdateCurrentHealth(float Health)
    {
        statistics.CurrentHealth += Health;
        statistics.CurrentHealth = Mathf.Clamp(statistics.CurrentHealth, 0, statistics.MaxHealth);
    }
}