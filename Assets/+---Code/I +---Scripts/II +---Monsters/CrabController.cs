using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabController : MonsterController
{
    protected override void Awake()
    {
        base.Awake(); // 부모 클래스의 Awake 메서드 호출
        FlipSprite = false;
        attackAnim = "attackC"; // CrabController의 공격 설정
        idleAnim = "idle";
        runAnim = "run";
    }

}
