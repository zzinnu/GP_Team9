using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAreaController : MonoBehaviour
{
    public PlayerController playerController;
    public PlayerMovementStats MovementStats;
    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        MovementStats = playerController.MovementStats;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
            return;

        else if (collider.name == "Ground")
        {
            // Hit the ground
        }

        else if (collider.tag == "Monster")
        {
            MonsterController monsterController = collider.gameObject.GetComponent<MonsterController>();
            monsterController.Damaged(MovementStats.AttackDamage);
            // collider.gameObject.BroadcastMessage("Damaged", MovementStats.AttackDamage);
        }
    }

}
