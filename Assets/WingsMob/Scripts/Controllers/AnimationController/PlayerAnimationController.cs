using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour, IAnimationController
{
    [SpineAnimation] [SerializeField] private string idleAnimationName;
    [SpineAnimation] [SerializeField] private string runAnimationName;

    private SkeletonAnimation m_anim;
    private void Start()
    {
        m_anim = GetComponent<SkeletonAnimation>();
    }

    public void Reset()
    {
        
    }

    public void Death(Transform caller, Action callback = null, bool isPoolBossObj = true)
    {
        
    }

    public void Idle()
    {
        if (m_anim.state.GetCurrent(0).Animation.Name == idleAnimationName) 
            return;
        m_anim.AnimationState.SetAnimation(0, idleAnimationName, true);
    }

    public void Run()
    {
        if (m_anim.state.GetCurrent(0).Animation.Name == runAnimationName)
            return;
        m_anim.AnimationState.SetAnimation(0, runAnimationName, true);
    }
}