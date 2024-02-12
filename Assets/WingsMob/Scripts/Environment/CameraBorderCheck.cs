using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Environment
{
    public class CameraBorderCheck : MonoBehaviour
    {
        [SerializeField] private Camera m_mainCam;
        [SerializeField] private SmoothCamera2D m_smoothCamera;

        private List<GameObject> borders;

        private void Start()
        {
            borders = new List<GameObject>();
            Utilities.FullScreenCollider(m_mainCam, GetComponent<BoxCollider2D>(), 2f);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Border")
            {
                borders.Add(collision.gameObject);
                if (!m_smoothCamera.isStopFollowX)
                    m_smoothCamera.isStopFollowX = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Border")
            {
                borders.Remove(collision.gameObject);
                if (borders.Count < 1)
                    m_smoothCamera.isStopFollowX = false;
            }
        }
    }
}
