using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.Utils
{
    public class JoystickMover : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private GameObject m_directionArrow;
        [SerializeField] private GameObject m_joystickBG;
        [SerializeField] private GameObject m_joystick;
        [ReadOnly] public Vector2 dragDirection;

        private Vector2 m_screenTouchPos;
        private Vector2 m_joystickOriginalPos;
        private float m_backgroundRadius;
        private EventTrigger m_eventTrigger;

        void Start()
        {
            m_joystickOriginalPos = m_joystickBG.transform.position;
            m_backgroundRadius = m_joystickBG.GetComponent<RectTransform>().sizeDelta.y / 4;
            m_eventTrigger = GetComponent<EventTrigger>();
            this.RegisterListener(MethodNameDefine.OnPlayerLeveledUp, OnPlayerLeveledUp);
            this.RegisterListener(MethodNameDefine.OnGamePaused, OnGamePaused);
        }

        private void OnDestroy()
        {
            this.RemoveListener(MethodNameDefine.OnPlayerLeveledUp, OnPlayerLeveledUp);
            this.RemoveListener(MethodNameDefine.OnGamePaused, OnGamePaused);
        }
        
        private void OnPlayerLeveledUp(object obj)
        {
            if (!LevelManager.Instance.playerController.CanLevelUp())
                return;
            OnGamePaused(obj);
        }

        private void OnGamePaused(object obj)
        {
            m_eventTrigger.CancelInvoke();
            PointerUp();
        }

        public void PointerDown()
        {
            m_screenTouchPos = Input.mousePosition;
            m_joystick.transform.position = m_screenTouchPos;
            m_joystickBG.transform.position = m_screenTouchPos;
            m_directionArrow.SetActive(true);
            player.HandleInput(Combat.State.PlayerInput.Move);
        }

        private Vector2 m_dragPos;
        private float m_distanceFromDragToTouch;

        public void Drag(BaseEventData baseEventData)
        {
            if (GameStatus.CurrentState != GameState.Playing)
                return;

            m_dragPos = (baseEventData as PointerEventData).position;

            dragDirection = (m_dragPos - m_screenTouchPos).normalized;

            if (dragDirection.magnitude != 0)
            {
                player.Act();
                RepositionJoystickThumb();
                RepositionDirectionArrow();
            }
        }

        private void RepositionJoystickThumb()
        {
            m_distanceFromDragToTouch = Vector2.Distance(m_dragPos, m_screenTouchPos);

            m_joystick.transform.position = m_distanceFromDragToTouch < m_backgroundRadius ?
                m_screenTouchPos + dragDirection * m_distanceFromDragToTouch
                : m_screenTouchPos + dragDirection * m_backgroundRadius;
        }

        private void RepositionDirectionArrow()
        {
            m_directionArrow.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dragDirection.x, dragDirection.y) * 180 / Mathf.PI);
        }

        public void PointerUp()
        {
            player.HandleInput(Combat.State.PlayerInput.Idle);
            player.Act();
            dragDirection = Vector2.zero;
            m_joystick.transform.position = m_joystickOriginalPos;
            m_joystickBG.transform.position = m_joystickOriginalPos;
            m_directionArrow.SetActive(false);
        }
    }
}