using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.Environment
{
    public class CameraZoomer : MonoBehaviour
    {
        [SerializeField] private float m_cameraNormalSize = 9f;
        [SerializeField] private float m_cameraBossSize = 12f;
        private Camera m_cam;
        private void Awake()
        {
            m_cam = GetComponent<Camera>();
        }

        private void OnEnable()
        {
            this.RegisterListener(MethodNameDefine.OnHyperModeStart, ChangeCameraHyperSize);
            this.RegisterListener(MethodNameDefine.OnHyperModeEnd, ChangeCameraNormalSize);
        }

        private void OnDisable()
        {
            this.RemoveListener(MethodNameDefine.OnHyperModeStart, ChangeCameraHyperSize);
            this.RemoveListener(MethodNameDefine.OnHyperModeEnd, ChangeCameraNormalSize);
        }

        private void ChangeCameraHyperSize(object obj)
        {
            m_cam.DOOrthoSize(m_cameraBossSize, 0.75f).OnComplete(() => this.PostEvent(MethodNameDefine.OnCameraOrthoSizeChanged));
        }

        private void ChangeCameraNormalSize(object obj)
        {
            m_cam.DOOrthoSize(m_cameraNormalSize, 0.75f).OnComplete(() => this.PostEvent(MethodNameDefine.OnCameraOrthoSizeChanged));
        }
    }
}
