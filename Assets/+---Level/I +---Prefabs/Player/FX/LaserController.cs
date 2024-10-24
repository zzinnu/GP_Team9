using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public PlayerController playerController;
    public PlayerMovementStats MovementStats;
    public GameObject ExplosionPrefab;

    private bool isEnabled;

    private bool _isFacingRight;

    void Start()
    {
        // Destroy the laser after 2.5 seconds
        Destroy(gameObject, 2.5f);

        playerController = FindObjectOfType<PlayerController>();
        MovementStats = playerController.MovementStats;

        _isFacingRight = playerController._isFacingRight;

        if (!playerController._isFacingRight)
        {
            Vector3 localeScale = transform.localScale;
            localeScale.x *= -1f;
            transform.localScale = localeScale;
        }

    }

    void Update()
    {
        if (_isFacingRight)
            transform.Translate(MovementStats.LaserSpeed * Time.deltaTime, 0, 0);
        else
            transform.Translate(-1 * MovementStats.LaserSpeed * Time.deltaTime, 0, 0);
    }

    // Is Trigger 옵션 사용 시 사용

    /* private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
            return;

        else if (collider.name == "Ground")
        {
            // Hit the ground
        }

        else if (collider.tag == "Monster")
        {
            collider.gameObject.SendMessage("Damaged", MovementStats.LaserDamage);
        }

        Explode(collider.transform.position);
    } */

    // Is Trigger 옵션 해제 시 사용

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
            return;

        else if (collision.collider.name == "Ground")
        {
            // Hit the ground
        }

        else if (collision.collider.tag == "Monster")
        {
            if (!isEnabled)
            {
                Debug.Log("Laser hit: " + collision.collider.gameObject + " with tag: " + collision.collider.tag);
                MonsterController monsterController = collision.collider.gameObject.GetComponent<MonsterController>();
                monsterController.Damaged(MovementStats.LaserDamage);
                StartCoroutine(LaserRoutine(0.1f));
            }

        }

        Explode(collision.contacts[0].point);
    }

    private void Explode(Vector2 position)
    {
        // Is Trigger 옵션 사용 시 사용
        /* Destroy(gameObject);
        Vector3 newPosition;
        if (_isFacingRight)
            newPosition = new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z);
        else
            newPosition = new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z);
        // Change to ExplosionPrefab
        Instantiate(ExplosionPrefab, newPosition, Quaternion.identity); */

        // Is Trigger 옵션 해제 시 사용
        Destroy(gameObject);
        Instantiate(ExplosionPrefab, position, Quaternion.identity);
    }

    public IEnumerator LaserRoutine(float time)
    {
        this.isEnabled = true;
        yield return new WaitForSeconds(time);
        this.isEnabled = false;
    }
}
