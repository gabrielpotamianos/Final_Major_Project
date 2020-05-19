using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AOEDamageScript : MonoBehaviour
{
    public List<EnemyCombat> enemiesToHit;
    PlayerCombat playerCombat;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        playerCombat = CharacterSelection.ChosenCharacter.Character.GetComponent<PlayerCombat>();
    }

    public void AOE_Damage(float damage)
    {
        if (enemiesToHit.Count > 0)
        {
            foreach (EnemyCombat enemy in enemiesToHit.ToList())
            {
                if (enemy.enemyData.Alive)
                    playerCombat.DealDamage(enemy, damage);

                else
                    enemiesToHit.Remove(enemy);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyCombat currEnemy = other.GetComponent<EnemyCombat>() ? other.GetComponent<EnemyCombat>() : null;
        if (currEnemy && !enemiesToHit.Contains(currEnemy) && currEnemy.enemyData.Alive)
            enemiesToHit.Add(currEnemy);
    }

    private void OnTriggerExit(Collider other)
    {
        print("working");
        EnemyCombat currEnemy = other.GetComponent<EnemyCombat>() ? other.GetComponent<EnemyCombat>() : null;
        if (currEnemy && enemiesToHit.Contains(currEnemy))
            enemiesToHit.Remove(currEnemy);
    }
}
