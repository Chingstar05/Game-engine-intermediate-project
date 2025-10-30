using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class StationaryEnemy : MonoBehaviour
{
    [Header("공격 설정")]
    public GameObject projectilePrefab; // 발사체 프리팹
    public Transform firePoint;         // 발사 위치
    public float attackCooldown = 1.5f; // 공격 간격
    public float projectileSpeed = 20f; // 발사체 속도
    public int damage = 10;             // 공격력

    private Transform player;
    private float lastAttackTime;

    void Start()
    {
        // 플레이어 찾기
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        lastAttackTime = -attackCooldown; // 바로 공격 가능
    }

    void Update()
    {
        if (player == null) return;

        // 플레이어를 바라보게 회전
        Vector3 lookDir = (player.position - transform.position).normalized;
        lookDir.y = 0; // Y축 고정, 위아래 회전 안함
        transform.rotation = Quaternion.LookRotation(lookDir);

        // 공격 쿨타임 확인
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        // 발사체 생성
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Rigidbody가 있으면 속도 적용
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * projectileSpeed;
        }

        // 데미지 정보 전달 (발사체 스크립트에 맞게)
        EnemyProjectile2 projScript = proj.GetComponent<EnemyProjectile2>();
        if (projScript != null)
        {
            projScript.damage = damage;
        }

        // 안전하게 일정 시간 후 삭제
        Destroy(proj, 5f);
    }
}
