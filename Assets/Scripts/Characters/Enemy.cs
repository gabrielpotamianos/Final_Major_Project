using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

[System.Serializable]
public class Enemy : CharacterStats
{
    public GameObject HealthBar;
    public GameObject CentrePointPatrol;
    public GameObject player;
    public float rangeSphere = 10.0f;

    NavMeshAgent agent;
    NavMeshPath path;
    Animator anim;
    Vector3 point;
    IEnumerator attackCoroutine;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        anim = GetComponent<Animator>();
        // player = GameObject.FindGameObjectWithTag(SelectCharacter.SelectedGameObject);
        player = GameObject.FindGameObjectWithTag("Warrior");
    }
    private void Start()
    {
        RandomPoint(CentrePointPatrol.transform.position, rangeSphere, out point);
        agent.SetDestination(point);

    }

    // Update is called once per frame
    void Update()
    {
        if (defaultStats.Health <= 0)
        {
           // gameObject.SetActive(false);
            //HealthBar.SetActive(false);
        }
        else
        {
            if (Vector3.Distance(player.transform.position, gameObject.transform.position) <= rangeSphere)
            {
                Debug.DrawLine(player.transform.position, gameObject.transform.position, Color.cyan);
                if (Vector3.Distance(agent.transform.position, player.transform.position) <= agent.stoppingDistance)
                {
                    anim.SetBool("Attack", true);
                    player.GetComponent<PlayerData>().TakeDamage(AttackPower);
                }
                else
                {
                    if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                        agent.SetDestination(player.transform.position);
                    anim.SetBool("Attack", false);
                }
                print(Vector3.Distance(agent.transform.position, player.transform.position) > agent.stoppingDistance);

            }

            else 
            {
                if (point != agent.destination)
                    agent.SetDestination(point);
                Debug.DrawRay(transform.position, transform.forward);

                if ((Vector3.Distance(agent.transform.position, point) <= 1.5f))
                {
                    RandomPoint(CentrePointPatrol.transform.position, rangeSphere, out point);
                    agent.SetDestination(point);
                }

            }
             //gent.isStopped = anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !anim.IsInTransition(0);



        }
        anim.SetFloat("Health", defaultStats.Health);
        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    IEnumerator BasicAttack()
    {
        anim.SetTrigger("AttackTrig");
        yield return new WaitForSeconds(1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(CentrePointPatrol.transform.position, rangeSphere);
    }


    void RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        while (true)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            Debug.DrawRay(randomPoint, Vector3.up, Color.blue, 14.0f);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                break;
            }

        }

    }

    public void TakeDamage(float dmg)
    {
        defaultStats.Health -= dmg;
        UpdateBar(defaultStats.Health);

    }

    public void UpdateBar(float health)
    {
        HealthBar.GetComponent<Slider>().value = health / 100.0f;

    }
}
