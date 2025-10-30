using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;




public class Enemy : MonoBehaviour
{
    public enum EnemyState { Idle, Trace, Attack, RunAway }

    public EnemyState state = EnemyState.Idle;

    [Header("이동 & 공격")]
    public float moveSpeed = 12f;
    public float traceRange = 15f;
    public float attackRange = 12f;
    public float attackCooldown = 1.5f;

    [Header("발사체")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public int damage = 20;

    [Header("상태")]
    private Transform player;
    private float lastAttackTime;

    [Header("체력")]
    public int maxHP = 30;
    private int currentHP;
    public Slider HpSlider;

    [Header("도망")]
    public float safeDistance = 10f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        lastAttackTime = -attackCooldown; // 바로 공격 가능
        currentHP = maxHP;
        if (HpSlider != null)
            HpSlider.value = 1f;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);

        // 체력 20% 이하 → 도망
        if (state != EnemyState.RunAway && currentHP <= maxHP * 0.2f)
        {
            state = EnemyState.RunAway;
        }

        switch (state)
        {
            case EnemyState.Idle:
                if (dist < traceRange) state = EnemyState.Trace;
                break;

            case EnemyState.Trace:
                if (dist < attackRange) state = EnemyState.Attack;
                else if (dist > traceRange) state = EnemyState.Idle;
                else TracePlayer();
                break;

            case EnemyState.Attack:
                if (dist > attackRange) state = EnemyState.Trace;
                else AttackPlayer();
                break;

            case EnemyState.RunAway:
                RunAway();
                break;
        }
    }

    void TracePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.LookAt(player.position);
    }

    void AttackPlayer()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        if (projectilePrefab != null && firePoint != null && player != null)
        {
            transform.LookAt(player.position);

            GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            // Rigidbody로 발사
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (player.position - firePoint.position).normalized;
                rb.velocity = dir * 20f; // 속도
            }

            // 데미지 정보 전달
            EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
            if (ep != null)
            {
                ep.damage = damage; // 여기서 데미지 설정
            }

            Destroy(proj, 3f); // 안전하게 파괴
        }
    }

    void RunAway()
    {
        Vector3 dir = (transform.position - player.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > safeDistance)
            state = EnemyState.Idle;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHP -= damageAmount;
        if (HpSlider != null)
            HpSlider.value = (float)currentHP / maxHP;

        if (currentHP <= 0) Die();
    }

    void Die()
    {
        // Wolf인지 체크
        if (CompareTag("Wolf"))
        {
            PC player = FindObjectOfType<PC>();
            if (player != null)
            {
                player.wolfKillCount++; // 플레이어 스크립트에 wolfKillCount 변수 필요
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, traceRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
