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
        spriteRenderer = GetComponent<SpriteRenderer>(); // 스프라이트 렌더러 가져오기

    }

    void Start()
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

    // 플레이어를 추적하는 함수 (체이서 몬스터만 해당)
    void Update()
    {
        DirCheck(); // 플레이어의 위치에 따라 스프라이트 뒤집기

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        //Debug.Log("Player detected at position: " + player.position + ", distanceToPlayer: " + distanceToPlayer);
        
        // 플레이어가 감지 범위 내에 있으면 추적
        if (distanceToPlayer <= detectionRange && distanceToPlayer > AttackRange)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Move(directionToPlayer);

            if (!idletofly) // 감지 처음 한번만 실행
            {
                anim.SetTrigger("idletofly");
                UpdateCollider();
                idletofly = true;
            }
        }

        // 플레이어가 공격 범위 안에 있으면 공격
        if (distanceToPlayer <= AttackRange)
        {
            anim.SetTrigger("bite");  // 공격 애니메이션 트리거
        }

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
            spriteRenderer.flipX = true;
        }
        else
        {
            // 플레이어가 왼쪽에 있을 때 (왼쪽 보게)
            spriteRenderer.flipX = false;
        }
    }

    public override void Damaged(float amount)
    {
        anim.SetTrigger("hit");
        base.Damaged(amount); // 부모 클래스의 Damaged 메서드 호출
    }

    protected override void Die()
    {
        anim.SetTrigger("death");

        // death 애니메이션이 끝나면 오브젝트 삭제
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("death") && stateInfo.normalizedTime >= 1.0f)
        {
            base.Die();
        }
    }

}
