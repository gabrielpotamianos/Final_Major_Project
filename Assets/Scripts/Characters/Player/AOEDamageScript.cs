using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AOEDamageScript : MonoBehaviour
{
    public List<EnemyCombat> enemiesToHit;


    public void AOE_Damage(float damage)
    {
        if (enemiesToHit.Count > 0)
        {
            foreach (EnemyCombat enemy in enemiesToHit.ToList())
            {
                if (enemy.enemyData.Alive)
                    enemy.TakeDamage(damage);
                else
                    enemiesToHit.Remove(enemy);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyCombat currEnemy = other.GetComponent<EnemyCombat>() ? other.GetComponent<EnemyCombat>() : null;
        if (currEnemy && !enemiesToHit.Contains(currEnemy) && currEnemy.enemyData.Alive)
            enemiesToHit.Add(other.GetComponent<EnemyCombat>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<EnemyCombat>() && enemiesToHit.Contains(other.GetComponent<EnemyCombat>()))
            enemiesToHit.Remove(other.GetComponent<EnemyCombat>());
    }
}
