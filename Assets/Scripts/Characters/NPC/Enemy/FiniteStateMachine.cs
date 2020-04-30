using UnityEngine;

public class FiniteStateMachine
{
    public abstract class State
    {
        public string name;
        abstract public void Begin(EnemyCombat enemy);
        abstract public void End(EnemyCombat enemy);
        abstract public void Execute(EnemyCombat enemy);

    }

    public sealed class GoTo : State
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
        public override void Begin(EnemyCombat enemy)
        {
            name = "GoTo";
            enemy.RandomPatrolPoint(enemy.enemyData.CentrePatrolPointPatrol.transform.position, enemy.enemyData.rangeSphere, out randomPoint);
            enemy.enemyData.agent.SetDestination(randomPoint);
            enemy.enemyData.animator.SetFloat("Speed", enemy.enemyData.agent.velocity.magnitude);


        }

        public override void Execute(EnemyCombat enemy)
        {

            if (Vector3.Distance(enemy.enemyData.playerCombat.transform.position, enemy.transform.position + enemy.transform.forward * enemy.enemyData.rangeSphere) <= enemy.enemyData.rangeSphere && enemy.enemyData.Hostile && PlayerData.instance.Alive)
            {
                randomPoint = enemy.enemyData.playerCombat.transform.position;
                if (enemy.enemyData.agent.remainingDistance <= enemy.enemyData.agent.stoppingDistance)
                    enemy.enemyData.FSMMachine.ChangeState(FiniteStateMachine.AttackState.Instance);
            }
            else if (enemy.enemyData.agent.remainingDistance <= enemy.enemyData.agent.stoppingDistance)
                enemy.RandomPatrolPoint(enemy.enemyData.CentrePatrolPointPatrol.transform.position, enemy.enemyData.rangeSphere, out randomPoint);

            if (enemy.enemyData.agent.destination != randomPoint)
                enemy.enemyData.agent.SetDestination(randomPoint);

        }

        public override void End(EnemyCombat enemy)
        {
            enemy.enemyData.animator.SetFloat("Speed", 0);
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
        public override void Begin(EnemyCombat enemy)
        {
            name = "Attack";
            enemy.enemyData.animator.SetBool("Attack", true);
        }


        public override void End(EnemyCombat enemy)
        {
            enemy.enemyData.animator.SetBool("Attack", false);
        }

        public override void Execute(EnemyCombat enemy)
        {
            if (Vector3.Distance(enemy.enemyData.playerCombat.transform.position, enemy.transform.position) > enemy.enemyData.agent.stoppingDistance || !PlayerData.instance.Alive)
                enemy.enemyData.FSMMachine.ChangeState(GoTo.Instance);
        }
    }





        private EnemyCombat enemy;
        private State currState;

        public FiniteStateMachine(EnemyCombat enemy, State initialState)
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