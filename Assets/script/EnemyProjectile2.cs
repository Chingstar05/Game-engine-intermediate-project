using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile2 : MonoBehaviour
{
    public int damage = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PC player = other.GetComponent<PC>();
            if (player != null)
            {
                player.TakeDamage(damage); // ������ ��Ȯ�� ����
            }
            Destroy(gameObject); // �浹 �� �ı�
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}