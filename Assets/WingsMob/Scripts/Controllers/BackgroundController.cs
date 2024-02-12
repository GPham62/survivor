using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WingsMob.Survival.Controller
{
    public class BackgroundController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_srBG;
        [SerializeField] private Transform point;
        [SerializeField] private Camera cam;
        [SerializeField] private float m_safePadding = 2f;

        private List<Transform> m_listBG = new List<Transform>();
        private List<Transform> m_listBGOutOfRange = new List<Transform>();

        private float cameraHeight;
        private float cameraWidth;
        private float horizontalRange;
        private float verticalRange;

        private Vector2 m_BGSize;

        private Transform leftPointOfCam;
        private Transform rightPointOfCam;
        private Transform topPointOfCam;
        private Transform botPointOfCam;

        private void Start()
        {
            m_listBG.Add(m_srBG.transform);

            cameraHeight = cam.orthographicSize;
            cameraWidth = cameraHeight * cam.aspect;
            horizontalRange = cameraWidth + m_safePadding;
            verticalRange = cameraHeight + m_safePadding;

            Bounds bounds = m_srBG.bounds;
            m_BGSize = new Vector2(bounds.size.x / 2, bounds.size.y / 2);

            leftPointOfCam = Instantiate(point.gameObject, transform).transform;
            rightPointOfCam = Instantiate(point.gameObject, transform).transform;
            botPointOfCam = Instantiate(point.gameObject, transform).transform;
            topPointOfCam = Instantiate(point.gameObject, transform).transform;
        }

        private void Update()
        {
            UpdateSafePadding();
            CheckCamOutOfBG();
        }

        private void UpdateSafePadding()
        {
            Vector3 camPosition = cam.transform.position;

            leftPointOfCam.position = new Vector3(camPosition.x - horizontalRange, camPosition.y, 0f);
            rightPointOfCam.position = new Vector3(camPosition.x + horizontalRange, camPosition.y, 0f);
            topPointOfCam.position = new Vector3(camPosition.x, camPosition.y + verticalRange, 0f);
            botPointOfCam.position = new Vector3(camPosition.x, camPosition.y - verticalRange, 0f);
        }

        private void CheckCamOutOfBG()
        {
            Transform leftBG = null;
            Transform rightBG = null;
            Transform topBG = null;
            Transform botBG = null;

            foreach(Transform bg in m_listBG)
            {
                if (leftBG == null && !CheckInHorizontal(bg, leftPointOfCam.position))
                    leftBG = bg;

                if (rightBG == null && !CheckInHorizontal(bg, rightPointOfCam.position))
                    rightBG = bg;

                if (topBG == null && !CheckInVertical(bg, topPointOfCam.position))
                    topBG = bg;

                if (botBG == null && !CheckInVertical(bg, botPointOfCam.position))
                    botBG = bg;
            }

            if (leftBG != null)
            {
                Transform newBg = Instantiate(m_srBG.gameObject, new Vector3(leftBG.position.x - m_BGSize.x, leftBG.position.y, leftBG.position.z), Quaternion.identity, transform).transform;
                m_listBG.Add(newBg);
            }
            else if (rightBG != null)
            {
                Transform newBg = Instantiate(m_srBG.gameObject, new Vector3(rightBG.position.x + m_BGSize.x, rightBG.position.y, rightBG.position.z), Quaternion.identity, transform).transform;
                m_listBG.Add(newBg);
            }

            if (topBG != null)
            {
                Transform newBg = Instantiate(m_srBG.gameObject, new Vector3(topBG.position.x, topBG.position.y + m_BGSize.y, topBG.position.z), Quaternion.identity, transform).transform;
                m_listBG.Add(newBg);
            }
            else if (botBG != null)
            {
                Transform newBg = Instantiate(m_srBG.gameObject, new Vector3(botBG.position.x, botBG.position.y - m_BGSize.y, botBG.position.z), Quaternion.identity, transform).transform;
                m_listBG.Add(newBg);
            }
        }

        private bool CheckInHorizontal(Transform bg, Vector3 point)
        {
            if (point.x >= bg.position.x - m_BGSize.x && point.x <= bg.position.x + m_BGSize.x)
                return true;

            return false;
        }

        private bool CheckInVertical(Transform bg, Vector3 point)
        {
            if (point.y >= bg.position.y - m_BGSize.y && point.y <= bg.position.y + m_BGSize.y)
                return true;

            return false;
        }
    }

}