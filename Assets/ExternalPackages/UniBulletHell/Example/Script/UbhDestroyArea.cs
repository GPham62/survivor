using UnityEngine;
using UnityEngine.Serialization;
using WingsMob.Survival.Global;

public class UbhDestroyArea : UbhMonoBehaviour
{
    [SerializeField, FormerlySerializedAs("_ColCenter")]
    private BoxCollider2D m_colCenter = null;

    private void Start()
    {
        if (m_colCenter == null)
        {
            return;
        }

        UbhGameManager manager = FindObjectOfType<UbhGameManager>();
        if (manager != null && manager.m_scaleToFit)
        {
            Vector2 max = Camera.main.ViewportToWorldPoint(UbhUtil.VECTOR2_ONE);
            Vector2 size = max * 2f;
            size.x += 0.5f;
            size.y += 0.5f;
            Vector2 center = UbhUtil.VECTOR2_ZERO;

            m_colCenter.size = size;
        }

        m_colCenter.enabled = true;
    }
    private void OnTriggerExit2D(Collider2D c)
    {
        HitCheck(c.transform);
    }

    private void OnTriggerExit(Collider c)
    {
        HitCheck(c.transform);
    }

    private void HitCheck(Transform colTrans)
    {
        if (colTrans.gameObject.CompareTag(SurvivorConfig.EnemyTag))
        {
            UbhBullet bullet = colTrans.GetComponent<UbhBullet>();
            if (bullet != null && bullet.isActive)
            {
                UbhObjectPool.instance.ReleaseBullet(bullet);
            }
        }
    }
}