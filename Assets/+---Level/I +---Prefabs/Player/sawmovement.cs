using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawMovement : MonoBehaviour
{
    public float speed = 3f; // ���� �̵� �ӵ�
    private Rigidbody2D playerRigidbody;
    public float pushBackForce = 7f; // �÷��̾ �о�� ��
    public float slowDuration = 3f; // �÷��̾� ������ ���� �ð�
    public float slowFactor = 5f; // �������� ����
    private PlayerHealth playerHealth;

    void Start()
    {
        // ������ �÷��̾� GameObject ã��
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        // �Դ� ���� �ϴ� ���� ������ ���� (�¿�� ������)
        transform.position = new Vector3(Mathf.PingPong(Time.time * speed, 4) - 2, transform.position.y, transform.position.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PushPlayerBack();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10); // �÷��̾�� ������ �ֱ�
                StartCoroutine(SlowPlayer()); // �÷��̾� �ӵ� ������ ����
            }
        }
    }

    void PushPlayerBack()
    {
        if (playerRigidbody != null)
        {
            // �÷��̾ �о�� ���� �ݴ� ���� �� ����
            Vector2 pushDirection = (playerRigidbody.transform.position - transform.position).normalized;
            playerRigidbody.AddForce(pushDirection * pushBackForce, ForceMode2D.Impulse);
        }
    }

    private System.Collections.IEnumerator SlowPlayer()
    {
        if (playerRigidbody != null)
        {
            float originalSpeed = playerRigidbody.velocity.magnitude;
            playerRigidbody.velocity *= slowFactor; // �÷��̾� �ӵ� ����

            yield return new WaitForSeconds(slowDuration); // ������ ���� ����

            playerRigidbody.velocity = playerRigidbody.velocity.normalized * originalSpeed; // ���� �ӵ��� ����
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
            // �÷��̾� ��� ó��
            Debug.Log("Player is Dead");
        }
        else
        {
            Debug.Log("Player Health: " + health);
        }
    }
}
