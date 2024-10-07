using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public virtual float hp { get; set; }
    public virtual float speed { get; set; }
    public virtual float AttackRange { get; set; }

    // 기본 이동 함수
    public virtual void Move(Vector3 direction)
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    public virtual void Damaged(float amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}