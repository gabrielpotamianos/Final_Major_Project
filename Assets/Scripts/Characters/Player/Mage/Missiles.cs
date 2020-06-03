using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Missiles : MonoBehaviour
{
    public bool IceMissiles;
    public AOEDamageScript AOEDamage;
    public float Speed;

    public float time;

    [HideInInspector]
    public float FireballMultiplier;

    [HideInInspector]
    public float BlizzardDamage;

    public static EnemyCombat CurrTarger;

    bool collided = false;


    void OnEnable()
    {
        //test=0;
        if (IceMissiles)
            Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);
        else Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), false);
        time = 0;
        collided = false;
    }

    void Update()
    {
        if (!IceMissiles && CurrTarger)
        {
            if (!CurrTarger.enemyData.Alive)
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

    void OnCollisionEnter(Collision collision)
    {
        if (IceMissiles && collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")) && !collided)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            collided = true;

            if (AOEDamage)
                AOEDamage.AOE_Damage(BlizzardDamage);
            StartCoroutine(Explode());
        }

        else if (!IceMissiles && collision.gameObject.tag.Equals("Enemy"))
        {
            //CHANGE TO ATTACKDMG
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);

            CharacterSelection.ChosenCharacter.Character.GetComponent<PlayerCombat>().DealDamage(CurrTarger, FireballMultiplier);

            GetComponent<Collider>().enabled = false;
            StartCoroutine(Explode());
        }

    }


    IEnumerator Explode()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.position = GameObject.FindGameObjectWithTag("Mage").gameObject.transform.position;

        MageCombatSystem.activeMissiles--;
        this.enabled = false;

    }


    Vector3 CalculateBezierPoint(float t, Vector3 startPosition, Vector3 endPosition, Vector3 controlPoint)
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
