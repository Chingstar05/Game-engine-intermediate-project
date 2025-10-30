using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class StationaryEnemy : MonoBehaviour
{
    [Header("���� ����")]
    public GameObject projectilePrefab; // �߻�ü ������
    public Transform firePoint;         // �߻� ��ġ
    public float attackCooldown = 1.5f; // ���� ����
    public float projectileSpeed = 20f; // �߻�ü �ӵ�
    public int damage = 10;             // ���ݷ�

    private Transform player;
    private float lastAttackTime;

    void Start()
    {
        // �÷��̾� ã��
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        lastAttackTime = -attackCooldown; // �ٷ� ���� ����
    }

    void Update()
    {
        if (player == null) return;

        // �÷��̾ �ٶ󺸰� ȸ��
        Vector3 lookDir = (player.position - transform.position).normalized;
        lookDir.y = 0; // Y�� ����, ���Ʒ� ȸ�� ����
        transform.rotation = Quaternion.LookRotation(lookDir);

        // ���� ��Ÿ�� Ȯ��
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        // �߻�ü ����
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Rigidbody�� ������ �ӵ� ����
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * projectileSpeed;
        }

        // ������ ���� ���� (�߻�ü ��ũ��Ʈ�� �°�)
        EnemyProjectile2 projScript = proj.GetComponent<EnemyProjectile2>();
        if (projScript != null)
        {
            projScript.damage = damage;
        }

        // �����ϰ� ���� �ð� �� ����
        Destroy(proj, 5f);
    }
}
