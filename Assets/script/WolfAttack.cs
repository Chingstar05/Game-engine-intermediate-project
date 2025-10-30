using UnityEngine;

public class WolfAttack : MonoBehaviour
{
    [Header("���� ����")]
    public int damage = 20;                  // ���ݷ�
    public float attackRange = 2f;           // ���� ���� �Ÿ�
    public float attackCooldown = 1f;        // ���� ��Ÿ��
    public LayerMask playerLayer;            // �÷��̾� ���̾�

    private bool isAttacking = false;

    [Header("Optional")]
    public Animator anim;                    // ���� �ִϸ��̼�

    void Update()
    {
        // ����: �÷��̾ ���� �ȿ� ������ �ڵ� ����
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

        // ���� �ִϸ��̼� ��� (���û���)
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }

        Debug.Log("���� ����!");

        

        // ���� ��Ÿ�� �� ����
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    // Scene���� ���� ���� �ð�ȭ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
