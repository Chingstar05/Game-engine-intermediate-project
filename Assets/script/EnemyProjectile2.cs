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
                player.TakeDamage(damage); // 데미지 정확히 적용
            }
            Destroy(gameObject); // 충돌 후 파괴
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}