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
        enemyData.FSMMachine = new FiniteStateMachine(enemyData, new FiniteStateMachine.Idle());

    }

    protected override void Update()
    {
        //Check if the enemy is alive based on the current health
        if (enemyData.IsItAlive(enemyData.statistics.CurrentHealth))
        {
            //Checks if health regen is needed
            enemyData.IsHealthRegenNeeded(ref IsRegenHealth, enemyData.statistics.CurrentHealth, enemyData.statistics.MaxHealth);

            //Updates the AI
            enemyData.FSMMachine.UpdateFSM();

            //Check Regeneration if needed
            CheckRegen();
        }
        else Die();


    }

    private void CheckRegen()
    {
        //Check if it is in combat and the combat cooldown coroutine is null
        if (InCombat && InCombatCoroutine == null)
        {
            //Assigns the combat cooldown coroutine
            InCombatCoroutine = this.CombatCooldown(CombatCooldownTime);

            //Starts it
            StartCoroutine(InCombatCoroutine);
        }
        //When the combat cooldown is off and the enemy is out of combat check if regen is needed or if it is happnening
        else if (IsRegenHealth && !InCombat && HealthRegenCoroutine == null)
        {
            //If is is needed and is not happening start regen coroutine
            HealthRegenCoroutine = HealthRegen();
            StartCoroutine(HealthRegenCoroutine);
        }
    }


    public override void DealDamage()
    {
        //If the enemy has a reference to the player
        if (enemyData.playerCombat)
        {
            //The hit chance must be passed to hit the player
            if (enemyData.statistics.HitChance > Random.Range(0, 100))
                enemyData.playerCombat.TakeDamage(enemyData.statistics.AttackPower);
            else enemyData.playerCombat.TakeDamage(0);

            //Resets the combat stats so the regen will not happen
            ResetCombatCoroutine();
        }
    }


    public override void TakeDamage(float damage)
    {

        //If the enemy is not returning to origin
        if (enemyData.FSMMachine != null && enemyData.FSMMachine.GetCurrState() != FiniteStateMachine.ReturnOrigin.Instance)
        {
            //Resets the combat stats so the regen will not happen
            ResetCombatCoroutine();

            if (damage > 0)
            {
                //Do some math to absorb damage instead of subtracting the armour
                damage = damage * (damage / (damage + enemyData.statistics.Armour));

                //Display the damage text pop up
                DisplayDamageText(damage);

                enemyData.UpdateCurrentHealth(-damage);

                enemyData.Hostile = true;

                //If the state is not chase or attack, change it to chase
                if (enemyData.FSMMachine.GetCurrState() != FiniteStateMachine.Chase.Instance && enemyData.FSMMachine.GetCurrState() != FiniteStateMachine.AttackState.Instance)
                    enemyData.FSMMachine.ChangeState(FiniteStateMachine.Chase.Instance);
            }
            else
                DisplayDamageText(Constants.HIT_MISS);
        }
    }

    public override void DisplayDamageText(float Damage)
    {
        //Get a random Text Position from the screen
        Vector3 TextPosition = Camera.main.WorldToScreenPoint(transform.position + transform.up * Constants.ENEMY_COMBAT_TEXT_HEIGHT_POS) 
        + new Vector3(UnityEngine.Random.Range(-Constants.ENEMY_COMBAT_TEXT_RANGE, Constants.ENEMY_COMBAT_TEXT_RANGE), 0, 0);
        
        //Instantiate a text Obejct
        GameObject DamageTextGameObject = Instantiate(DamageTextPrefab, TextPosition, Quaternion.identity, enemyData.GetCanvasRoot().transform);
        
        Text DamageText = DamageTextGameObject.transform.GetChild(0).GetComponent<Text>();
        
        //Change colour and text of the object
        DamageText.text = Damage.ToString();
        DamageText.color = Color.yellow;
    }

    public void DisplayDamageText(string Message)
    {
        //Get a random Text Position from the screen
        Vector3 TextPosition = Camera.main.WorldToScreenPoint(transform.position + transform.up * Constants.ENEMY_COMBAT_TEXT_HEIGHT_POS) 
        + new Vector3(UnityEngine.Random.Range(-Constants.ENEMY_COMBAT_TEXT_RANGE, Constants.ENEMY_COMBAT_TEXT_RANGE), 0, 0);

        //Instantiate a text Obejct
        GameObject DamageTextGameObject = Instantiate(DamageTextPrefab, TextPosition, Quaternion.identity, enemyData.GetCanvasRoot().transform);
        Text DamageText = DamageTextGameObject.transform.GetChild(0).GetComponent<Text>();
      
        //Change colour and message of the object
        DamageText.text = Message;
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
        //Die once by checking if alive and if has a null state
        if (!enemyData.Alive && enemyData.FSMMachine.GetCurrState() != null)
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
            collider.center = new Vector3(collider.center.x, Constants.ENEMY_COMBAT_LOOTINGBOX_HEIGHT, Constants.ENEMY_COMBAT_LOOTINGBOX_DEPTH);
            
            //When laying dead the height changes with depth
            collider.size = new Vector3(collider.size.x, collider.size.z, collider.size.y);
        }
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
