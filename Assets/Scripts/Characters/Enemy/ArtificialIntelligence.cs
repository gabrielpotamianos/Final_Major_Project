using UnityEngine;
public class ArtificialIntelligence
{
    public abstract class State
    {
        public string name;
        abstract public void Begin(Enemy enemy);
        abstract public void End(Enemy enemy);
        abstract public void Execute(Enemy enemy);

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
        public override void Begin(Enemy enemy)
        {
            name = "GoTo";
            enemy.RandomPatrolPoint(enemy.CentrePatrolPointPatrol.transform.position, enemy.rangeSphere, out randomPoint);
            enemy.agent.SetDestination(randomPoint);
            enemy.anim.SetFloat("Speed", enemy.agent.velocity.magnitude);


        }

        public override void Execute(Enemy enemy)
        {

            if (Vector3.Distance(enemy.player.transform.position, enemy.transform.position + enemy.transform.forward * enemy.rangeSphere) <= enemy.rangeSphere && enemy.Hostile && enemy.player.Alive)
            {
                randomPoint = enemy.player.transform.position;
                if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
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
            if (Vector3.Distance(enemy.player.transform.position, enemy.transform.position) > enemy.agent.stoppingDistance || !enemy.player.Alive)
                enemy.FSMMachine.ChangeState(GoTo.Instance);
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

        public State GetCurrState()
        {
            return currState;
        }
    }
}

















/*
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

    public State GetCurrState()
    {
        return currState;
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

        if (Vector3.Distance(enemy.player.transform.position, enemy.transform.position + enemy.transform.forward * enemy.rangeSphere) <= enemy.rangeSphere && enemy.Hostile && enemy.player.Alive)
        {
            randomPoint = enemy.player.transform.position;
            if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
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
        if (Vector3.Distance(enemy.player.transform.position, enemy.transform.position) > enemy.agent.stoppingDistance || !enemy.player.Alive)
            enemy.FSMMachine.ChangeState(GoTo.Instance);
    }
}*/
