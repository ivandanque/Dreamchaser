using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileContainer : MonoBehaviour
{
    public AttackHit attackHit;
    public float radius;
    private float damage;

    private float lifeTime;
    public LayerMask playerLayer;
    private PlayerUnit pu;
    private bool isAlive = false;

    private void Update()
    {
        if (isAlive)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0) Destroy(gameObject);
        }
    }

    public void SetAttack(float damage, float lifetime)
    {
        this.damage = damage;
        //this.attackHit = attackHit;
        isAlive = true;
        lifeTime = lifetime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, radius, playerLayer);
            foreach (Collider col in cols)
            {
                pu = col.GetComponent<PlayerUnit>();
                pu.TakeDamage(damage);
                //pu.InterruptPlayer(attack.interruptValue);
                //pu.GetComponent<Rigidbody>().AddForce(-transform.forward * attack.pushbackForce);
            }
            Destroy(gameObject);
        }
    }
}
