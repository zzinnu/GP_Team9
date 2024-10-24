using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [SerializeField]
    public float hp;
    [SerializeField]
    public float speed;
    [SerializeField]
    public float AttackRange;
    protected Transform player;
    protected Animator anim;
    protected PolygonCollider2D polygonCollider;
    protected SpriteRenderer spriteRenderer;
    protected Sprite previousSprite;
    public float detectionRange = 10f;
    protected bool FirstAnimation = false;
    protected string FirstAnim;
    protected string attackAnim;
    protected string idleAnim;
    protected string runAnim;
    protected bool FlipSprite = true;


    protected virtual void Awake()
    {
        polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // ��������Ʈ ������ ��������
    }

    protected virtual void Start()
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

    protected virtual void Update()
    {
        DirCheck(); // �÷��̾��� ��ġ�� ���� ��������Ʈ ������

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        //Debug.Log("Player detected at position: " + player.position + ", distanceToPlayer: " + distanceToPlayer);

        // �÷��̾ ���� ���� ���� ������ ����
        if (distanceToPlayer <= detectionRange)
        {
            // �÷��̾ ���� ���� �ȿ� ������ ����
            if (distanceToPlayer <= AttackRange)
            {
                TriggerAnimation(attackAnim);  // ���� �ִϸ��̼�
                Debug.Log(attackAnim);
            }
            else // ���� ���� ��, ���� ���� ��
            {
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                Move(directionToPlayer);

                if (!FirstAnimation) // ���� ó�� �ѹ��� ����
                {
                    if (FirstAnim != null)
                    {
                        TriggerAnimation(FirstAnim); // ó�� �����ϴ� �ִϸ��̼�
                    }
                    FirstAnimation = true;
                }
                TriggerAnimation(runAnim);
            }
        }
        else
        {
            TriggerAnimation(idleAnim);
            Debug.Log("idleAnim");
        }


        // ��������Ʈ ����� ��� �ݶ��̴� ������Ʈ
        if (spriteRenderer.sprite != previousSprite)
        {
            UpdateCollider();
            previousSprite = spriteRenderer.sprite; // ���� ��������Ʈ�� ���� ��������Ʈ�� ����
        }

        // death �ִϸ��̼��� ������ ������Ʈ ����
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("death") && stateInfo.normalizedTime >= 1.0f)
        {
            Destroy(gameObject);
        }

    }

    // Gizmos�� ���� ������ ���� ���� �׸���
    private void OnDrawGizmosSelected()
    {
        // Ž�� ������ �Ķ������� ǥ��
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // ���� ������ ���������� ǥ��
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }

    protected virtual void TriggerAnimation(string Animation)
    {
        anim.SetTrigger(Animation); // ������ ����� �ִϸ��̼� �̸��� ���
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
            if (FlipSprite)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
        else
        {
            // �÷��̾ ���ʿ� ���� �� (���� ����)
            if (FlipSprite)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }
    }

    // �⺻ �̵� �Լ�
    public virtual void Move(Vector3 direction)
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    public virtual void Damaged(float amount)
    {
        anim.SetTrigger("hit");
        hp -= amount;
        if (hp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        anim.SetTrigger("death");
    }
}