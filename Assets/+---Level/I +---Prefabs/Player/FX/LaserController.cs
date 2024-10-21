using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public PlayerController playerController;
    private bool _isFacingRight;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
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
            transform.Translate(10 * Time.deltaTime, 0, 0);
        else
            transform.Translate(-10 * Time.deltaTime, 0, 0);

    }
}
