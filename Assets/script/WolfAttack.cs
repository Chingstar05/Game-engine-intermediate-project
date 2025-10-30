using UnityEngine;

public class WolfAttack : MonoBehaviour
{
    [Header("공격 설정")]
    public int damage = 20;                  // 공격력
    public float attackRange = 2f;           // 발톱 공격 거리
    public float attackCooldown = 1f;        // 공격 쿨타임
    public LayerMask playerLayer;            // 플레이어 레이어

    private bool isAttacking = false;

    [Header("Optional")]
    public Animator anim;                    // 공격 애니메이션

    void Update()
    {
        // 예시: 플레이어가 범위 안에 있으면 자동 공격
        Collider[] playersInRange = Physics.OverlapSphere(transform.position, attackRange, playerLayer);
        if (playersInRange.Length > 0)
        {
            Attack(playersInRange);
        }
    }

    void Attack(Collider[] targets)
    {
        if (isAttacking) return;
        isAttacking = true;

        // 공격 애니메이션 재생 (선택사항)
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }

        Debug.Log("발톱 공격!");

        

        // 공격 쿨타임 후 리셋
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    // Scene에서 공격 범위 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
