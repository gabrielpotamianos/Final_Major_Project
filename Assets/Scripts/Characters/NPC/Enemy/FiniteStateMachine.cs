using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;

public class FiniteStateMachine
{
    public abstract class State
    {
        public string name;
        abstract public void Begin(EnemyData enemy);
        abstract public void End(EnemyData enemy);
        abstract public void Execute(EnemyData enemy);

    }

    public sealed class Wander : State
    {
        static readonly Wander instance = new Wander();
        public static Wander Instance
        {
            get
            {
                return instance;
            }
        }

        Vector3 randomPoint;
        public override void Begin(EnemyData enemy)
        {
            name = "Wander";
            enemy.agent.stoppingDistance = 0.5f;
            GetRandomPoint(enemy);
        }

        public override void Execute(EnemyData enemy)
        {
            if (enemy.agent.pathStatus == NavMeshPathStatus.PathComplete && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
                GetRandomPoint(enemy);
        }

        public override void End(EnemyData enemy)
        {
            enemy.animator.SetFloat("Speed", 0);
        }

        private void GetRandomPoint(EnemyData enemy)
        {
            enemy.RandomPatrolPoint(enemy.CentrePatrolPointPatrol.transform.position, enemy.rangeSphere, out randomPoint);
            enemy.agent.velocity = Vector3.zero;
            enemy.agent.SetDestination(randomPoint);
            enemy.transform.LookAt(randomPoint);
            enemy.animator.SetFloat("Speed", enemy.agent.velocity.magnitude);
        }


    }

    public sealed class Chase : State
    {
        static readonly Chase instance = new Chase();
        public static Chase Instance
        {
            get
            {
                return instance;
            }
        }

        Vector3 randomPoint;
        public override void Begin(EnemyData enemy)
        {
            name = "Chase";
            enemy.agent.stoppingDistance = 1.5f;
            ChasePlayer(enemy);
        }

        public override void Execute(EnemyData enemy)
        {
            enemy.agent.SetDestination(enemy.playerCombat.transform.position);

            if (enemy.agent.pathStatus != NavMeshPathStatus.PathComplete && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
                ChasePlayer(enemy);
            else if (enemy.agent.remainingDistance >= enemy.SightRange)
                enemy.FSMMachine.ChangeState(Wander.Instance);
            else if (enemy.agent.pathStatus == NavMeshPathStatus.PathComplete && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
            {
                enemy.agent.velocity = Vector3.zero;
                enemy.FSMMachine.ChangeState(AttackState.Instance);
            }
        }

        public override void End(EnemyData enemy)
        {
            enemy.animator.SetFloat("Speed", 0);
        }

        private void ChasePlayer(EnemyData enemy)
        {
            enemy.agent.velocity = Vector3.zero;
            enemy.transform.LookAt(enemy.playerCombat.transform.position);
            enemy.animator.SetFloat("Speed", enemy.agent.velocity.magnitude);
        }


    }
    public sealed class AttackState : State
    {
        static readonly AttackState instance = new AttackState();
        public static AttackState Instance
        {
            get
            {
                return instance;
            }
        }
        public override void Begin(EnemyData enemy)
        {
            name = "Attack";
            enemy.animator.SetBool("Attack", true);
        }


        public override void End(EnemyData enemy)
        {
            enemy.animator.SetBool("Attack", false);
        }

        public override void Execute(EnemyData enemy)
        {
            //CHANGE THIS
            //
            //
            //
            //
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //
            //
            //
            //
            if (Vector3.Distance(enemy.playerCombat.transform.position, enemy.transform.position) > enemy.AttackRange)
            {
                Debug.Log(Vector3.Distance(enemy.playerCombat.transform.position, enemy.transform.position) > enemy.AttackRange);
                Debug.Log(Vector3.Distance(enemy.playerCombat.transform.position, enemy.transform.position));
            }
            if (Vector3.Distance(enemy.playerCombat.transform.position, enemy.transform.position) > enemy.AttackRange || !PlayerData.instance.Alive)
                enemy.FSMMachine.ChangeState(Chase.Instance);
        }
    }





    private EnemyData enemy;
    private State currState;

    public FiniteStateMachine(EnemyData enemy, State initialState)
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

    public State GetCurrState()
    {
        return currState;
    }




}