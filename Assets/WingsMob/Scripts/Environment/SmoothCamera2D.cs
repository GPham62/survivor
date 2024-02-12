using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WingsMob.Survival.Environment
{
    public class SmoothCamera2D : MonoBehaviour
    { 
        public float dampTime = 0.15f;
        private Vector3 velocity = Vector3.zero;
        public Transform target;
        private Camera cam;
        public bool isStopFollowX;

        private void Start()
        {
            cam = GetComponent<Camera>();
            isStopFollowX = false;
        }

        void Update()
        {
            if (target)
            {
                Vector3 point = cam.WorldToViewportPoint(target.position);
                Vector3 delta = target.position - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
                Vector3 destination = transform.position + (isStopFollowX ? new Vector3(0, delta.y, delta.z) : delta);
                transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
            }
        }
    }
}
