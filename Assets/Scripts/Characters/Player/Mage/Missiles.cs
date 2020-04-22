using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Missiles : MonoBehaviour
{
    public bool IceMissiles;
    public AOEDamageScript AOEDamage;
    public float Speed;

    float time;

    [HideInInspector]
    public float FireballMultiplier;

    [HideInInspector]
    public float BlizzardDamage;

    public static Enemy CurrTarger;

    private void OnEnable()
    {
        if (IceMissiles)
            Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);
        else Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), false);

        time = 0;
    }

    void Update()
    {
        if (!IceMissiles && CurrTarger)
        {
            if (!CurrTarger.defaultStats.Alive)
            {
                CurrTarger = null;
                return;
            }
            else
            {
                time += Time.deltaTime * 1.0f / Speed;
                transform.position = CalculateBezierPoint(time, transform.position, CurrTarger.gameObject.transform.position + new Vector3(0, 1, 0), Vector3.zero);
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (IceMissiles && collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);

            if (AOEDamage)
                AOEDamage.AOE_Damage(BlizzardDamage);
            StartCoroutine(Explode());
        }

        else if (!IceMissiles && collision.gameObject.tag.Equals("Enemy"))
        {
            //CHANGE TO ATTACKDMG
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);

            CurrTarger.TakeDamage(GameObject.FindGameObjectWithTag(CharacterSelection.ChosenCharacter.breed.ToString()).GetComponent<PlayerData>().AttackPower * FireballMultiplier);
            StartCoroutine(Explode());
        }

    }


    IEnumerator Explode()
    {
        yield return new WaitForSeconds(1);
        // PlayerCombat.activeMissiles--;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.position = GameObject.FindGameObjectWithTag("Mage").gameObject.transform.position;

        // CurrTarger=null;
        MageCombatSystem.activeMissiles--;
        this.enabled = false;
        gameObject.SetActive(false);

    }


    private Vector3 CalculateBezierPoint(float t, Vector3 startPosition, Vector3 endPosition, Vector3 controlPoint)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * startPosition;
        point += 2 * u * t * startPosition;
        point += tt * endPosition;

        return point;
    }
}
