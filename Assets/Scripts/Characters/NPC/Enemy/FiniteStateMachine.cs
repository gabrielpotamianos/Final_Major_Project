using UnityEngine;
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
        Vector3 randomPoint;

        static readonly Wander instance = new Wander();

        public static Wander Instance
        {
            get
            {
                return instance;
            }
        }

        public override void Begin(EnemyData enemy)
        {
            name = "Wander";
            enemy.agent.stoppingDistance = Constants.FSM_WANDER_STOPPING_DISTANCE;
            GetRandomPoint(enemy);
        }

        public override void Execute(EnemyData enemy)
        {
            if (enemy.agent.pathStatus == NavMeshPathStatus.PathComplete && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
                enemy.FSMMachine.ChangeState(Idle.Instance);


        }

        public override void End(EnemyData enemy)
        {
            enemy.animator.SetFloat("Speed", 0);
        }

        private void GetRandomPoint(EnemyData enemy)
        {
            enemy.agent.velocity = Vector3.zero;

            enemy.RandomPatrolPoint(enemy.CentrePatrolPointPatrol.transform.position, enemy.rangeSphere, out randomPoint);
            enemy.agent.SetDestination(randomPoint);
            enemy.transform.LookAt(randomPoint);
            enemy.animator.SetFloat("Speed", 1);
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

        public override void Begin(EnemyData enemy)
        {
            if (!enemy.playerCombat.playerData.Alive)
                enemy.FSMMachine.ChangeState(Wander.Instance);
                
            name = "Chase";
            enemy.agent.stoppingDistance = Constants.FSM_CHASE_STOPPING_DISTANCE;
            ChasePlayer(enemy);
        }

        public override void Execute(EnemyData enemy)
        {
            if (!enemy.playerCombat.playerData.Alive)
                enemy.FSMMachine.ChangeState(Wander.Instance);
            else if (CharacterSelection.ChosenCharacter.breed == CharacterInfo.Breed.Rogue)
            {
                if (!RogueCombatSystem.stealth)
                {
                    enemy.agent.SetDestination(enemy.playerCombat.transform.position);

                    enemy.GetComponent<Combat>().InCombat = true;

                    if (enemy.agent.pathStatus != NavMeshPathStatus.PathComplete && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
                        ChasePlayer(enemy);
                    else if (enemy.agent.remainingDistance >= enemy.SightRange || Vector3.Distance(enemy.transform.position, enemy.CentrePatrolPointPatrol.transform.position) > enemy.MaxCombatDistance)
                        enemy.FSMMachine.ChangeState(ReturnOrigin.Instance);
                    else if (enemy.agent.pathStatus == NavMeshPathStatus.PathComplete && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
                    {
                        enemy.agent.velocity = Vector3.zero;
                        enemy.FSMMachine.ChangeState(AttackState.Instance);
                    }

                }
                else enemy.FSMMachine.ChangeState(Wander.Instance);
            }
            else
            {
                enemy.agent.SetDestination(enemy.playerCombat.transform.position);

                enemy.GetComponent<Combat>().InCombat = true;

                if (enemy.agent.pathStatus != NavMeshPathStatus.PathComplete && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
                    ChasePlayer(enemy);
                else if (enemy.agent.remainingDistance >= enemy.SightRange || Vector3.Distance(enemy.transform.position, enemy.CentrePatrolPointPatrol.transform.position) > enemy.MaxCombatDistance)
                    enemy.FSMMachine.ChangeState(ReturnOrigin.Instance);
                else if (enemy.agent.pathStatus == NavMeshPathStatus.PathComplete && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
                {
                    enemy.agent.velocity = Vector3.zero;
                    enemy.FSMMachine.ChangeState(AttackState.Instance);
                }

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
            enemy.animator.SetFloat("Speed", 1);
            enemy.agent.SetDestination(enemy.playerCombat.transform.position);
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
            if (!enemy.playerCombat.playerData.Alive)
                enemy.FSMMachine.ChangeState(Wander.Instance);
            name = "Attack";
            enemy.animator.SetBool("Attack", true);
        }

        public override void End(EnemyData enemy)
        {
            enemy.animator.SetBool("Attack", false);
        }

        public override void Execute(EnemyData enemy)
        {
            if (!enemy.playerCombat.playerData.Alive)
                enemy.FSMMachine.ChangeState(Wander.Instance);
            else
            {
                if (Vector3.Distance(enemy.playerCombat.transform.position, enemy.transform.position) > enemy.AttackRange || !PlayerData.instance.Alive)
                    enemy.FSMMachine.ChangeState(Chase.Instance);

                if (CharacterSelection.ChosenCharacter.breed == CharacterInfo.Breed.Rogue)
                    if (RogueCombatSystem.stealth)
                        enemy.FSMMachine.ChangeState(Wander.Instance);

            }
        }
    }

    public sealed class Idle : State
    {
        bool coroutineRunning = false;
        IEnumerator IdleCoroutine;
        static readonly Idle instance = new Idle();

        public static Idle Instance
        {
            get
            {
                return instance;
            }
        }

        public override void Begin(EnemyData enemy)
        {
            name = "Idle";
            enemy.animator.SetFloat("Speed", 0);
            enemy.agent.velocity = Vector3.zero;
            if (IdleCoroutine != null) enemy.StopCoroutine(IdleCoroutine);
            IdleCoroutine = IdleFor(enemy.PatrolFrequencyTime, enemy);
            enemy.StartCoroutine(IdleCoroutine);
        }

        public override void End(EnemyData enemy)
        {
            if (IdleCoroutine != null) enemy.StopCoroutine(IdleCoroutine);
            IdleCoroutine = null;
        }

        public override void Execute(EnemyData enemy)
        {
        }

        private IEnumerator IdleFor(float time, EnemyData enemy)
        {
            yield return new WaitForSeconds(time);

            if (enemy.FSMMachine.GetCurrState() == this)
                enemy.FSMMachine.ChangeState(Wander.Instance);
            else yield break;


        }
    }

    public sealed class ReturnOrigin : State
    {
        static readonly ReturnOrigin instance = new ReturnOrigin();

        public static ReturnOrigin Instance
        {
            get
            {
                return instance;
            }
        }

        public override void Begin(EnemyData enemy)
        {
            name = "ReturnOrigin";
            enemy.agent.stoppingDistance = Constants.FSM_WANDER_STOPPING_DISTANCE;
            Return(enemy);
            enemy.GetComponent<Combat>().InCombat = false;
        }

        public override void Execute(EnemyData enemy)
        {
            if (enemy.agent.pathStatus == NavMeshPathStatus.PathComplete && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
                enemy.FSMMachine.ChangeState(Wander.Instance);
        }

        public override void End(EnemyData enemy)
        {
            enemy.animator.SetFloat("Speed", 0);
        }

        private void Return(EnemyData enemy)
        {
            enemy.agent.velocity = Vector3.zero;
            enemy.agent.SetDestination(enemy.CentrePatrolPointPatrol.transform.position);
            enemy.transform.LookAt(enemy.CentrePatrolPointPatrol.transform.position);
            enemy.animator.SetFloat("Speed", 1);
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
            currState.End(enemy);

        currState = newState;

        if (currState != null) currState.Begin(enemy);
    }

    public State GetCurrState()
    {
        return currState;
    }




}