using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileContainer : MonoBehaviour
{
    public Attack attack;
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

    public void SetAttack(float damage, Attack attack)
    {
        this.damage = damage;
        this.attack = attack;
        SetLifetime();
    }

    private void SetLifetime()
    {
        isAlive = true;
        lifeTime = attack.activeTime;
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
                pu.InterruptPlayer(attack.interruptValue);
                pu.GetComponent<Rigidbody>().AddForce(-transform.forward * attack.pushbackForce);
            }
            Destroy(gameObject);
        }
    }
}
