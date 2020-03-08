using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;


[System.Serializable]
public class Enemy : CharacterStats
{

    public GameObject HealthBar;
    public GameObject CentrePatrolPointPatrol;
    public PlayerData player;
    public float rangeSphere = 10.0f;

    public NavMeshAgent agent;
    public FiniteStateMachine FSMMachine;
    Vector3 PatrolPoint;


    public override void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        // player = GameObject.FindGameObjectWithTag(SelectCharacter.SelectedGameObject);
        player = GameObject.FindGameObjectWithTag("Warrior").GetComponent<PlayerData>();
        base.Awake();
        FSMMachine = new FiniteStateMachine(this, new GoTo());

    }

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Warrior").GetComponent<PlayerData>();

    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (defaultStats.Alive)
            FSMMachine.UpdateFSM();
        else agent.isStopped = true;
        anim.SetFloat("Health", defaultStats.Health);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(CentrePatrolPointPatrol.transform.position, rangeSphere);
        Gizmos.DrawWireSphere(transform.position + transform.forward * rangeSphere, rangeSphere);
    }

    public void RandomPatrolPoint(Vector3 center, float range, out Vector3 result)
    {
        while (true)
        {
            Vector3 randomPatrolPoint = center + UnityEngine.Random.insideUnitSphere * range;
            Debug.DrawRay(randomPatrolPoint, Vector3.up, Color.blue, 14.0f);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPatrolPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                break;
            }

        }

    }


    public void TakeDamage(float dmg)
    {
        defaultStats.Health -= defaultStats.Health - dmg >= 0 ? dmg : defaultStats.Health;
        UpdateBar(defaultStats.Health);
    }

    public void DealDamage()
    {
        if (player)
            player.TakeDamage(AttackPower);
    }

    public void UpdateBar(float health)
    {
        HealthBar.GetComponent<Slider>().value = health / 100.0f;

    }
}


public class FiniteStateMachine
{
    private Enemy enemy;
    private State currState;

    public FiniteStateMachine(Enemy enemy, State initialState)
    {
        this.enemy = enemy;
        ChangeState(initialState);
    }

    public void UpdateFSM()
    {
        if (currState != null) currState.Execute(enemy);
    }

    public void ChangeState(State newState)
    {
        if (currState != null)
        {
            currState.End(enemy);
        }

        currState = newState;

        if (currState != null) currState.Begin(enemy);
    }
}






public abstract class State
{
    public string name;
    abstract public void Begin(Enemy enemy);
    abstract public void End(Enemy enemy);
    abstract public void Execute(Enemy enemy);

}


sealed class GoTo : State
{
    static readonly GoTo instance = new GoTo();
    public static GoTo Instance
    {
        get
        {
            return instance;
        }
    }

    Vector3 randomPoint;
    public override void Begin(Enemy enemy)
    {
        name = "GoTo";
        enemy.RandomPatrolPoint(enemy.CentrePatrolPointPatrol.transform.position, enemy.rangeSphere, out randomPoint);
        enemy.agent.SetDestination(randomPoint);
        enemy.anim.SetFloat("Speed", enemy.agent.velocity.magnitude);


    }

    public override void Execute(Enemy enemy)
    {

        if (Vector3.Distance(enemy.player.transform.position, enemy.transform.position) <= enemy.rangeSphere && enemy.defaultStats.Hostile && enemy.player.defaultStats.Alive)
        {
            randomPoint = enemy.player.transform.position;
            if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance )
                enemy.FSMMachine.ChangeState(AttackState.Instance);
        }
        else if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
            enemy.RandomPatrolPoint(enemy.CentrePatrolPointPatrol.transform.position, enemy.rangeSphere, out randomPoint);

        if (enemy.agent.destination != randomPoint)
            enemy.agent.SetDestination(randomPoint);

    }

    public override void End(Enemy enemy)
    {
        enemy.anim.SetFloat("Speed", 0);
    }

}


sealed class AttackState : State
{
    static readonly AttackState instance = new AttackState();
    public static AttackState Instance
    {
        get
        {
            return instance;
        }
    }
    public override void Begin(Enemy enemy)
    {
        name = "Attack";
        enemy.anim.SetBool("Attack", true);
    }


    public override void End(Enemy enemy)
    {
        enemy.anim.SetBool("Attack", false);
    }

    public override void Execute(Enemy enemy)
    {
        if (Vector3.Distance(enemy.player.transform.position, enemy.transform.position) > enemy.agent.stoppingDistance || !enemy.player.defaultStats.Alive)
            enemy.FSMMachine.ChangeState(GoTo.Instance);
    }
}