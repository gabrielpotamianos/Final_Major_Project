using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float DestroyTime;
    public float ProjectileSpeed;
    [HideInInspector]
    public Vector3 directionProjectile;


    void Start()
    {
       StartCoroutine(DestroyProjectile(DestroyTime));
    }

    private void Update()
    {
        transform.position += directionProjectile*ProjectileSpeed;
    }


    IEnumerator DestroyProjectile(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            Destroy(gameObject);
            collision.gameObject.GetComponent<Enemy>().TakeDamage(10);
        }
    }


}
