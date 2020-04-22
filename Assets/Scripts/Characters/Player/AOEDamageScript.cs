using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AOEDamageScript : MonoBehaviour
{
    public List<Enemy> enemiesToHit;


    public void AOE_Damage(float damage)
    {
        if (enemiesToHit.Count > 0)
        {
            foreach (Enemy enemy in enemiesToHit.ToList())
            {
                if(enemy.defaultStats.Alive)
                    enemy.TakeDamage(damage);
                else
                    enemiesToHit.Remove(enemy);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy currEnemy = other.GetComponent<Enemy>() ? other.GetComponent<Enemy>() : null;
        if (currEnemy && !enemiesToHit.Contains(currEnemy) && currEnemy.defaultStats.Alive)
            enemiesToHit.Add(other.GetComponent<Enemy>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Enemy>() && enemiesToHit.Contains(other.GetComponent<Enemy>()))
            enemiesToHit.Remove(other.GetComponent<Enemy>());
    }
}
