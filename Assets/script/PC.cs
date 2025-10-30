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
    public int requiredWolfKills = 1; // 다음 맵 넘어가기 위한 처치 수
  
    public int hunterKillCount = 0;    // 헌터 처치 수
    public int requiredHunterKills = 1; // 필요한 헌터 처치 수 (1마리면 1)

    public int KeyCount = 0;
    public int requiredKeyCount = 1;

    public Slider hpSlider;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentHp = maxHp;
        hpSlider.value = 1f;
        cam = Camera.main;

        // 현재 스테이지 확인
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
        Debug.Log("플레이어가 사망했습니다.");
    }

    void Attack()
    {
        if (isAttacking) return;
        isAttacking = true;
        Debug.Log("공격!");
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint;
        targetPoint = ray.GetPoint(50f);
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));

        

        // 속도 부여
        proj.GetComponent<Rigidbody>().velocity = direction * 20f;  // 속도는 상황에 맞게 조절



        // 1초 후 자동 파괴
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
            Debug.Log("현재 코인: " + cointCount);
        }
        else if (other.CompareTag("Door"))
        {
            isNearDoor = true;
            currentDoor = other.gameObject;
            Debug.Log("[E] 키를 눌러 문을 열 수 있습니다.");
        }
        else if (other.CompareTag("Sword"))
        {
            hasSword = true;
            Destroy(other.gameObject);
            Debug.Log("검을 획득했습니다!");
        }
        else if(other.CompareTag("Wolf"))
        {
            wolfKillCount++;
            Debug.Log("늑대 처치 수: " + wolfKillCount + "/" + requiredWolfKills);
            Destroy(other.gameObject);
        }
        else if(other.CompareTag("Key"))
        {
            KeyCount++;
            Destroy(other.gameObject);
            Debug.Log("열쇠를 획득했습니다! 현재 열쇠 수: " + KeyCount);

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            isNearDoor = false;
            currentDoor = null;
            Debug.Log("문에서 멀어졌습니다.");
        }
    }

    void TryEnterNextMap()
    {
        if (cointCount >= requiredCoins ||
            wolfKillCount >= requiredWolfKills ||
            hunterKillCount >= requiredHunterKills ||
            KeyCount >= requiredKeyCount)
        {
            Debug.Log("문을 엽니다. 다음 맵으로 이동합니다!");
            LoadNextScene();
        }
        else
        {
            Debug.Log("조건 미달! 코인: " + cointCount + "/" + requiredCoins +
                      ", Wolf 처치: " + wolfKillCount + "/" + requiredWolfKills +
                      ", Hunter 처치: " + hunterKillCount + "/" + requiredHunterKills +
                      "key 획득" + KeyCount + "/" + requiredKeyCount);
        }
    }



    void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextSceneIndex);
        else
            Debug.Log("마지막 씬입니다. 더 이상 넘어갈 씬이 없습니다.");
    }
}
