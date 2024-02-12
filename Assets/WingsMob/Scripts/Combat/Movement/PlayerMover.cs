using System;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Combat
{
    public class PlayerMover : Mover
    {
        public float baseSpeed { get; private set; }
        public float Speed
        {
            get { return m_speed; }
            set { m_speed = value; }
        }

        [SerializeField] private JoystickMover m_joystickMover;
        [SerializeField] private Transform playerBody;

        public override void Init(IAnimationController animationController, float speed)
        {
            base.Init(animationController, speed);
            baseSpeed = m_speed;
            m_prevDirection = Vector2.right;
        }

        public void FreezeRigidbody()
        {
            m_rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        public void UnfreezeRigidbody()
        {
            m_rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        public override void StopMoving()
        {
            m_rigidBody.velocity = Vector2.zero;
        }

        private Vector2 tempVelocity;

        public override void Move()
        {
            tempVelocity = new Vector2(m_joystickMover.dragDirection.x * m_speed, m_joystickMover.dragDirection.y * m_speed);
            m_rigidBody.velocity = m_joystickMover.dragDirection.y != 0 ? tempVelocity : Vector2.zero;
            m_animationController.Run();

            if (m_joystickMover.dragDirection.x < 0)
            {
                playerBody.transform.eulerAngles = new Vector3(0, -180, 0);
            }
            else
                playerBody.transform.eulerAngles = new Vector3(0, 0, 0);

            if (m_joystickMover.dragDirection != Vector2.zero)
            {
                m_prevDirection = m_joystickMover.dragDirection;
            }
        }

        private Vector2 m_prevDirection;

        public Vector2 GetMoveDirection() => m_prevDirection;
    }
}

