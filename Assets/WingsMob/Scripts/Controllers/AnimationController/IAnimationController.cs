using System;
using UnityEngine;

public interface IAnimationController
{
    void Reset();
    void Death(Transform caller, Action callback = null, bool isPoolBossObj = true);
    void Idle();
    void Run();
}