using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.Combat
{
    public class EnemyMover : Mover
    {
        private Transform m_playerTransform;

        public override void StopMoving()
        {
            m_rigidBody.velocity = Vector2.zero;
        }

        public override bool IsMoving()
        {
            return m_rigidBody.velocity != Vector2.zero;
        }

        public override void Init(IAnimationController animationController, float speed)
        {
            base.Init(animationController, speed);
            m_playerTransform = LevelManager.Instance.playerController.mover.transform;
        }

        public void Idle()
        {
            StopMoving();
        }

        public void AddForce(Vector2 force)
        {
            m_rigidBody.velocity = force;
        }

        public void FollowPlayer()
        {
            m_rigidBody.velocity = (m_playerTransform.position - transform.position).normalized * m_speed;

            m_animationController.Run();

            if (m_playerTransform.transform.position.x < transform.position.x)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            
        }
    }
}