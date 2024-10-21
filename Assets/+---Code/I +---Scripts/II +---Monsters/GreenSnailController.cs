using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSnailController : MonsterController
{
    protected override void Awake()
    {
        base.Awake(); // �θ� Ŭ������ Awake �޼��� ȣ��
        FlipSprite = false;
        attackAnim = "attack"; // CrabController�� ���� ����
        idleAnim = "idle";
        runAnim = "run";
    }

}
