using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PC : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpPower = 5.0f;
    public float gravity = -9.81f;

    private CharacterController controller;
    public float rotationSpeed = 10.0f;

    private Vector3 velocity;
    private bool isGrounded;

    private bool isNearDoor = false;
    private GameObject currentDoor;

    public int maxHp = 100;
    public int cointCount = 0;
    public int requiredCoins = 5;
    private int currentHp;

    private bool hasSword = false;
    private bool isAttacking = false;
    public GameObject attackCollider;

    public GameObject projectilePrefab;
    public Transform firePoint;
    Camera cam;

    public int wolfKillCount = 0;
    public int requiredWolfKills = 1; // ���� �� �Ѿ�� ���� óġ ��
  
    public int hunterKillCount = 0;    // ���� óġ ��
    public int requiredHunterKills = 1; // �ʿ��� ���� óġ �� (1������ 1)

    public int KeyCount = 0;
    public int requiredKeyCount = 1;

    public Slider hpSlider;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentHp = maxHp;
        hpSlider.value = 1f;
        cam = Camera.main;

        // ���� �������� Ȯ��
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        
       
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex >= 3)
        {
            hasSword = true;
            if (projectilePrefab != null)
                projectilePrefab.SetActive(true);
        }
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * z + camRight * x).normalized;
        controller.Move(moveDir * speed * Time.deltaTime);

        if (moveDir.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = jumpPower;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetKey(KeyCode.LeftShift))
            speed = 15f;
        else
            speed = 5f;

       
        if (isNearDoor && Input.GetKeyDown(KeyCode.E))
        {
            TryEnterNextMap();
            
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 15f;
           


        }
        else
        {
            speed = 5f;
            
        }


        if (hasSword && Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        hpSlider.value = (float)currentHp / maxHp;

        if (currentHp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
        Debug.Log("�÷��̾ ����߽��ϴ�.");
    }

    void Attack()
    {
        if (isAttacking) return;
        isAttacking = true;
        Debug.Log("����!");
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint;
        targetPoint = ray.GetPoint(50f);
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));

        

        // �ӵ� �ο�
        proj.GetComponent<Rigidbody>().velocity = direction * 20f;  // �ӵ��� ��Ȳ�� �°� ����



        // 1�� �� �ڵ� �ı�
        Destroy(proj, 1f);


        Invoke(nameof(ResetAttack), 0.5f);
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            cointCount++;
            Destroy(other.gameObject);
            Debug.Log("���� ����: " + cointCount);
        }
        else if (other.CompareTag("Door"))
        {
            isNearDoor = true;
            currentDoor = other.gameObject;
            Debug.Log("[E] Ű�� ���� ���� �� �� �ֽ��ϴ�.");
        }
        else if (other.CompareTag("Sword"))
        {
            hasSword = true;
            Destroy(other.gameObject);
            Debug.Log("���� ȹ���߽��ϴ�!");
        }
        else if(other.CompareTag("Wolf"))
        {
            wolfKillCount++;
            Debug.Log("���� óġ ��: " + wolfKillCount + "/" + requiredWolfKills);
            Destroy(other.gameObject);
        }
        else if(other.CompareTag("Key"))
        {
            KeyCount++;
            Destroy(other.gameObject);
            Debug.Log("���踦 ȹ���߽��ϴ�! ���� ���� ��: " + KeyCount);

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            isNearDoor = false;
            currentDoor = null;
            Debug.Log("������ �־������ϴ�.");
        }
    }

    void TryEnterNextMap()
    {
        if (cointCount >= requiredCoins ||
            wolfKillCount >= requiredWolfKills ||
            hunterKillCount >= requiredHunterKills ||
            KeyCount >= requiredKeyCount)
        {
            Debug.Log("���� ���ϴ�. ���� ������ �̵��մϴ�!");
            LoadNextScene();
        }
        else
        {
            Debug.Log("���� �̴�! ����: " + cointCount + "/" + requiredCoins +
                      ", Wolf óġ: " + wolfKillCount + "/" + requiredWolfKills +
                      ", Hunter óġ: " + hunterKillCount + "/" + requiredHunterKills +
                      "key ȹ��" + KeyCount + "/" + requiredKeyCount);
        }
    }



    void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextSceneIndex);
        else
            Debug.Log("������ ���Դϴ�. �� �̻� �Ѿ ���� �����ϴ�.");
    }
}
