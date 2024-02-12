using DarkTonic.PoolBoss;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace WingsMob.Survival.Boss.Skill
{
    public class MolestBombPattern : MonoBehaviour
    {
        [SerializeField] protected float m_bombScaleTime = 0.05f;
        [SerializeField] private List<BombPath> m_paths;
        [SerializeField] protected float m_tweenTime;
        [SerializeField] protected bool m_isSequenceThrow;
        [EnableIf("m_isSequenceThrow")] [SerializeField] protected float m_delayBetweenThrow;

        public virtual float ThrowBomb(Transform bombPrefab, float damage)
        {
            float totalThrowTime = 0f;
            if (m_isSequenceThrow)
            {
                for (int i = 0; i < m_paths.Count; i++)
                {
                    Transform newBomb = PoolBoss.SpawnInPool(bombPrefab, transform.position, Quaternion.identity);
                    newBomb.localScale = Vector2.zero;
                    newBomb.DOScale(0.4f, m_bombScaleTime).SetDelay(totalThrowTime);
                    newBomb.DOMoveX(m_paths[i].destination.position.x, m_tweenTime).SetEase(m_paths[i].easeX).SetDelay(totalThrowTime);
                    newBomb.DOMoveY(m_paths[i].destination.position.y, m_tweenTime).SetEase(m_paths[i].easeY).SetDelay(totalThrowTime)
                        .OnComplete(() =>
                        {
                            MolestBomb molestBomb = newBomb.gameObject.GetComponent<MolestBomb>();
                            molestBomb.SetDamage(damage);
                            molestBomb.TriggerExplosion();
                        });
                    totalThrowTime += m_delayBetweenThrow;
                }
            }
            else
            {
                foreach (var path in m_paths)
                {
                    Transform newBomb = PoolBoss.SpawnInPool(bombPrefab, transform.position, Quaternion.identity);
                    newBomb.localScale = Vector2.zero;
                    newBomb.DOScale(0.4f, m_bombScaleTime);
                    newBomb.DOMoveX(path.destination.position.x, m_tweenTime).SetEase(path.easeX);
                    newBomb.DOMoveY(path.destination.position.y, m_tweenTime).SetEase(path.easeY).OnComplete(() => {
                        MolestBomb molestBomb = newBomb.gameObject.GetComponent<MolestBomb>();
                        molestBomb.SetDamage(damage);
                        molestBomb.TriggerExplosion();
                    });
                }
            }
            return totalThrowTime;
        }

        [System.Serializable]
        public struct BombPath
        {
            public Transform destination;
            public Ease easeX;
            public Ease easeY;
        }
    }
}
