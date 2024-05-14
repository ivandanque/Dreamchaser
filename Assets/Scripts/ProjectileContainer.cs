using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AdaptivePerformance.VisualScripting;

public class ProjectileContainer : MonoBehaviour
{
    public AttackHit attackHit;
    private float damage;

    private float lifeTime;
    public LayerMask playerLayer;
    public LayerMask enemyLayer;
    public ParticleSystem projectileVFX;
    private bool isAlive = false;

    private void Update()
    {
        if (isAlive)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0) Destroy(gameObject);
        }
    }

    private void Explosion()
    {
        if (projectileVFX != null) Instantiate(projectileVFX, transform.position, Quaternion.identity).GetComponent<ParticleSystem>().Play();
    }

    public void SetAttack(float damage, float lifetime)
    {
        this.damage = damage;
        isAlive = true;
        lifeTime = lifetime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerUnit>().TakeDamage(damage);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyUnit>().TakeDamage(damage);
            Explosion();
            Destroy(gameObject);
        }
        
    }
}
