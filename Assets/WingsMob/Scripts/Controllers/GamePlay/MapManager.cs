using DarkTonic.PoolBoss;
using Sirenix.OdinInspector;
using UnityEngine;
using WingsMob.Survival.Environment.Map;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Controller
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private Transform m_mapHolder;

        public void SpawnMiniPool(int level)
        {
            PoolMiniBoss miniPool = Resources.Load<PoolMiniBoss>("PoolBoss/pool_lvl_" + (level + 1));
            if (miniPool == null)
            {
                DebugExtension.Log("pool_lvl_" + (level + 1) + " doesn't exist!", Color.green);
            }
            else
            {
                Instantiate(miniPool, Vector3.zero, Quaternion.identity);
            }
        }

        public void SpawnMapByLevel(int level)
        {
            m_mapHolder.Clear();
            SurvivorMap map = Resources.Load<SurvivorMap>("Maps/level_" + (level+1));
            if (map == null)
            {
                DebugExtension.Log("level_" + (level+1) + " doesn't exist!", Color.green);
            }
            else
            {
                SurvivorMap newMap = Instantiate(map, map.transform.position, map.transform.rotation, m_mapHolder);
                newMap.SetupMap();
                if (newMap is SurvivorParallaxMap)
                    LevelManager.Instance.isVerticalMap = true;
            }
        }

        public Transform GetMap() => m_mapHolder.GetChild(0);
    }
}