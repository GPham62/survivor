using DarkTonic.PoolBoss;
using DG.Tweening;
using UnityEngine;

namespace WingsMob.Survival.Boss.Skill
{
    public class MolestBombMultiplePattern : MolestBombPattern
    {
        [SerializeField] private BombMultiplePath[] paths;
        public override float ThrowBomb(Transform bombPrefab, float damage)
        {
            float totalThrowTime = 0f;
            for (int i = 0; i < paths.Length; i++)
            {
                for (int j = 0; j < paths[i].destinations.Length; j++)
                {
                    Transform newBomb = PoolBoss.SpawnInPool(bombPrefab, transform.position, Quaternion.identity);
                    newBomb.localScale = Vector2.zero; newBomb.DOScale(0.4f, m_bombScaleTime).SetDelay(totalThrowTime);
                    newBomb.DOMoveX(paths[i].destinations[j].destination.position.x, m_tweenTime).SetEase(paths[i].destinations[j].easeX).SetDelay(totalThrowTime);
                    newBomb.DOMoveY(paths[i].destinations[j].destination.position.y, m_tweenTime).SetEase(paths[i].destinations[j].easeY).SetDelay(totalThrowTime)
                        .OnComplete(() =>
                        {
                            MolestBomb molestBomb = newBomb.gameObject.GetComponent<MolestBomb>();
                            molestBomb.SetDamage(damage);
                            molestBomb.TriggerExplosion();
                        });
                }
                totalThrowTime += m_delayBetweenThrow;
            }
            return totalThrowTime;
        }

        [System.Serializable]
        public struct BombMultiplePath
        {
            public BombPath[] destinations;
        }
    }
}
