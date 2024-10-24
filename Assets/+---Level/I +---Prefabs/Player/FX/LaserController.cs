using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public PlayerController playerController;
    public PlayerMovementStats MovementStats;
    public GameObject ExplosionPrefab;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            return;

        else if (collision.name == "Ground")
        {
            // Hit the ground
        }

        else if (collision.tag == "Monster")
        {
            collision.gameObject.BroadcastMessage("Damaged", MovementStats.LaserDamage);
        }

        Explode();
    }

    private void Explode()
    {
        Destroy(gameObject);
        Vector3 newPosition;
        if (_isFacingRight)
            newPosition = new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z);
        else
            newPosition = new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z);
        // Change to ExplosionPrefab
        Instantiate(ExplosionPrefab, newPosition, Quaternion.identity);
    }
}
