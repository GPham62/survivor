using System.Collections;
using UnityEngine;
using WingsMob.Survival.Global;
using WingsMob.Survival.Stats;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Combat
{
    public abstract class Mover : MonoBehaviour
    {
        protected float m_speed;
        protected float m_originalSpeed;
        //private bool m_isSlowing;
        protected Rigidbody2D m_rigidBody;
        protected IAnimationController m_animationController;
        public virtual void Init(IAnimationController animationController, float speed)
        {
            m_speed = m_originalSpeed = speed;
            m_animationController = animationController;
            m_rigidBody = GetComponent<Rigidbody2D>();
        }

        public virtual void StopMoving() { }

        public virtual bool IsMoving() { return false; }
        public virtual void Move() { }

        public void Reset()
        {
            //m_isSlowing = false;
        }

        public void ChangeSpeed(float speed)
        {
            m_speed = speed;
        }
    }
}
