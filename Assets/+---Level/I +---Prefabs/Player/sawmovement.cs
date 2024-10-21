using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawMovement : MonoBehaviour
{
    public float speed = 3f; // 톱의 이동 속도
    private Rigidbody2D playerRigidbody;
    public float pushBackForce = 7f; // 플레이어를 밀어내는 힘
    public float slowDuration = 3f; // 플레이어 느려짐 지속 시간
    public float slowFactor = 5f; // 느려지는 정도
    private PlayerHealth playerHealth;

    void Start()
    {
        // 씬에서 플레이어 GameObject 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        // 왔다 갔다 하는 톱의 움직임 구현 (좌우로 움직임)
        transform.position = new Vector3(Mathf.PingPong(Time.time * speed, 4) - 2, transform.position.y, transform.position.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PushPlayerBack();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10); // 플레이어에게 데미지 주기
                StartCoroutine(SlowPlayer()); // 플레이어 속도 느려짐 적용
            }
        }
    }

    void PushPlayerBack()
    {
        if (playerRigidbody != null)
        {
            // 플레이어를 밀어내기 위한 반대 방향 힘 적용
            Vector2 pushDirection = (playerRigidbody.transform.position - transform.position).normalized;
            playerRigidbody.AddForce(pushDirection * pushBackForce, ForceMode2D.Impulse);
        }
    }

    private System.Collections.IEnumerator SlowPlayer()
    {
        if (playerRigidbody != null)
        {
            float originalSpeed = playerRigidbody.velocity.magnitude;
            playerRigidbody.velocity *= slowFactor; // 플레이어 속도 감소

            yield return new WaitForSeconds(slowDuration); // 느려진 상태 유지

            playerRigidbody.velocity = playerRigidbody.velocity.normalized * originalSpeed; // 원래 속도로 복구
        }
    }
}

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // 플레이어 사망 처리
            Debug.Log("Player is Dead");
        }
        else
        {
            Debug.Log("Player Health: " + health);
        }
    }
}
