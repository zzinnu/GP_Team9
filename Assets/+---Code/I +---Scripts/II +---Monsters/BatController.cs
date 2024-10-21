using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonsterController
{
    protected override void Awake()
    {
        base.Awake(); // 부모 클래스의 Awake 메서드 호출
        FirstAnim = "idletofly";
        attackAnim = "bite"; // BatController의 공격 애니메이션 이름 설정
        idleAnim = "fly";
        runAnim = "fly";
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
