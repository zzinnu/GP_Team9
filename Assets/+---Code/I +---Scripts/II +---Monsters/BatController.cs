using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonsterController
{
    protected override void Awake()
    {
        base.Awake(); // �θ� Ŭ������ Awake �޼��� ȣ��
        FirstAnim = "idletofly";
        attackAnim = "bite"; // BatController�� ���� �ִϸ��̼� �̸� ����
        idleAnim = "fly";
        runAnim = "fly";
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
