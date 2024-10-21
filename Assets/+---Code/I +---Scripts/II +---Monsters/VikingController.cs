using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VikingController : MonsterController
{
    protected override void Awake()
    {
        base.Awake(); // �θ� Ŭ������ Awake �޼��� ȣ��
        FlipSprite = false;
        attackAnim = "attackA"; // CrabController�� ���� ����
        idleAnim = "idle";
        runAnim = "walk";
    }

}
