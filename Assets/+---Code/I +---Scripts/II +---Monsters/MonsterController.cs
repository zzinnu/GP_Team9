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
        spriteRenderer = GetComponent<SpriteRenderer>(); // 스프라이트 렌더러 가져오기
    }

    protected virtual void Start()
    {
        // 태그 "Player"인 오브젝트 찾기
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform; // 플레이어의 Transform을 가져옴
        }
        else
        {
            Debug.LogError("Player with tag 'Player' not found!");
        }
    }

    protected virtual void Update()
    {
        DirCheck(); // 플레이어의 위치에 따라 스프라이트 뒤집기

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        //Debug.Log("Player detected at position: " + player.position + ", distanceToPlayer: " + distanceToPlayer);

        // 플레이어가 감지 범위 내에 있으면 추적
        if (distanceToPlayer <= detectionRange)
        {
            // 플레이어가 공격 범위 안에 있으면 공격
            if (distanceToPlayer <= AttackRange)
            {
                TriggerAnimation(attackAnim);  // 공격 애니메이션
                Debug.Log(attackAnim);
            }
            else // 감지 범위 안, 공격 범위 밖
            {
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                Move(directionToPlayer);

                if (!FirstAnimation) // 감지 처음 한번만 실행
                {
                    if (FirstAnim != null)
                    {
                        TriggerAnimation(FirstAnim); // 처음 실행하는 애니메이션
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


        // 스프라이트 변경된 경우 콜라이더 업데이트
        if (spriteRenderer.sprite != previousSprite)
        {
            UpdateCollider();
            previousSprite = spriteRenderer.sprite; // 이전 스프라이트를 현재 스프라이트로 갱신
        }

        // death 애니메이션이 끝나면 오브젝트 삭제
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("death") && stateInfo.normalizedTime >= 1.0f)
        {
            Destroy(gameObject);
        }

    }

    // Gizmos로 감지 범위와 공격 범위 그리기
    private void OnDrawGizmosSelected()
    {
        // 탐지 범위는 파란색으로 표시
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 공격 범위는 빨간색으로 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }

    protected virtual void TriggerAnimation(string Animation)
    {
        anim.SetTrigger(Animation); // 변수로 저장된 애니메이션 이름을 사용
    }

    void UpdateCollider()
    {
        polygonCollider.pathCount = 0;

        // 스프라이트의 물리 경로를 저장할 리스트 생성
        List<Vector2> physicsShape = new List<Vector2>();

        // 현재 스프라이트에서 물리 경로 가져오기
        spriteRenderer.sprite.GetPhysicsShape(0, physicsShape);

        // 물리 경로를 폴리곤 콜라이더에 설정
        polygonCollider.SetPath(0, physicsShape.ToArray());
    }

    void DirCheck()
    {
        if (player.position.x > transform.position.x)
        {
            // 플레이어가 오른쪽에 있을 때 (오른쪽 보게)
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
            // 플레이어가 왼쪽에 있을 때 (왼쪽 보게)
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

    // 기본 이동 함수
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