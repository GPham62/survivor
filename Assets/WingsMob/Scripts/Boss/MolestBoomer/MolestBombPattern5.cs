using DarkTonic.PoolBoss;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Controller;

namespace WingsMob.Survival.Boss.Skill
{
    public class MolestBombPattern5 : MolestBombPattern
    {
        [SerializeField] private List<BombPath> m_paths;

        public override float ThrowBomb(Transform bombPrefab, float damage)
        {
            Vector3 playerPosition = LevelManager.Instance.playerController.mover.transform.position;
            Transform nearestDestination = m_paths[0].destination;
            float nearestDistance = float.MaxValue;

            for (int i = 0; i < m_paths.Count; i++)
            {
                float distance = (m_paths[i].destination.transform.position - playerPosition).sqrMagnitude;

                if (distance < nearestDistance)
                {
                    nearestDestination = m_paths[i].destination;
                    nearestDistance = distance;
                }
            }

            float totalThrowTime = 0f;
            foreach (var path in m_paths)
            {
                if (path.destination != nearestDestination)
                {
                    Transform newBomb = PoolBoss.SpawnInPool(bombPrefab, transform.position, Quaternion.identity);
                    newBomb.localScale = Vector2.zero;
                    newBomb.DOScale(0.4f, m_bombScaleTime);
                    newBomb.DOMoveX(path.destination.position.x, m_tweenTime).SetEase(path.easeX);
                    newBomb.DOMoveY(path.destination.position.y, m_tweenTime).SetEase(path.easeY).OnComplete(() =>
                    {
                        MolestBomb molestBomb = newBomb.gameObject.GetComponent<MolestBomb>();
                        molestBomb.SetDamage(damage);
                        molestBomb.TriggerExplosion();
                    });
                }
            }
            return totalThrowTime;
        }
    }
}
