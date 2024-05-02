using System;
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
    public LayerMask enemyLayer;
    private bool isAlive = false;

    public static event Action<float> OnPlayerHit;
    public static event Action<float> OnEnemyHit;

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
        isAlive = true;
        lifeTime = lifetime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, radius, playerLayer);
            for (int i = 0; i < cols.Length; i++) OnPlayerHit?.Invoke(damage);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, radius, enemyLayer);
            for (int i = 0; i < cols.Length; i++) OnEnemyHit?.Invoke(damage);
            Destroy(gameObject);
        }
    }
}
