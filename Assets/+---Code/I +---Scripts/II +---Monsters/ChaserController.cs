using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserController : MonsterController
{
    private Transform player;
    public override float hp { get; set; } = 5f;
    public override float speed { get; set; } = 3f;
    public override float AttackRange { get; set; } = 1f;
    public float detectionRange = 10f;
    Animator anim;
    PolygonCollider2D polygonCollider;
    SpriteRenderer spriteRenderer;
    private bool idletofly = false;

    void Awake()
    {
        polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // ��������Ʈ ������ ��������

    }

    void Start()
    {
        // �±� "Player"�� ������Ʈ ã��
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform; // �÷��̾��� Transform�� ������
        }
        else
        {
            Debug.LogError("Player with tag 'Player' not found!");
        }
    }

    // �÷��̾ �����ϴ� �Լ� (ü�̼� ���͸� �ش�)
    void Update()
    {
        DirCheck(); // �÷��̾��� ��ġ�� ���� ��������Ʈ ������

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        //Debug.Log("Player detected at position: " + player.position + ", distanceToPlayer: " + distanceToPlayer);
        
        // �÷��̾ ���� ���� ���� ������ ����
        if (distanceToPlayer <= detectionRange && distanceToPlayer > AttackRange)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Move(directionToPlayer);

            if (!idletofly) // ���� ó�� �ѹ��� ����
            {
                anim.SetTrigger("idletofly");
                UpdateCollider();
                idletofly = true;
            }
        }

        // �÷��̾ ���� ���� �ȿ� ������ ����
        if (distanceToPlayer <= AttackRange)
        {
            anim.SetTrigger("bite");  // ���� �ִϸ��̼� Ʈ����
        }

    }

    void UpdateCollider()
    {
        polygonCollider.pathCount = 0;

        // ��������Ʈ�� ���� ��θ� ������ ����Ʈ ����
        List<Vector2> physicsShape = new List<Vector2>();

        // ���� ��������Ʈ���� ���� ��� ��������
        spriteRenderer.sprite.GetPhysicsShape(0, physicsShape);

        // ���� ��θ� ������ �ݶ��̴��� ����
        polygonCollider.SetPath(0, physicsShape.ToArray());
    }

    void DirCheck() 
    {
        if (player.position.x > transform.position.x)
        {
            // �÷��̾ �����ʿ� ���� �� (������ ����)
            spriteRenderer.flipX = true;
        }
        else
        {
            // �÷��̾ ���ʿ� ���� �� (���� ����)
            spriteRenderer.flipX = false;
        }
    }

    public override void Damaged(float amount)
    {
        anim.SetTrigger("hit");
        base.Damaged(amount); // �θ� Ŭ������ Damaged �޼��� ȣ��
    }

    protected override void Die()
    {
        anim.SetTrigger("death");

        // death �ִϸ��̼��� ������ ������Ʈ ����
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("death") && stateInfo.normalizedTime >= 1.0f)
        {
            base.Die();
        }
    }

}
