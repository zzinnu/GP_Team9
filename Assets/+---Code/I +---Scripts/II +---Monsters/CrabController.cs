using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabController : MonsterController
{
    protected override void Awake()
    {
        base.Awake(); // �θ� Ŭ������ Awake �޼��� ȣ��
        FlipSprite = false;
        attackAnim = "attackC"; // CrabController�� ���� ����
        idleAnim = "idle";
        runAnim = "run";
    }

}
