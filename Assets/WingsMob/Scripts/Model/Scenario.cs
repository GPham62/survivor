using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WingsMob.Survival.Model
{
    [Serializable]
    public class Scenario
    {
        public float startScenarioTime;
        public MapEvent mapEvent;
        [TableList(ShowPaging = true)]
        public List<EnemyInfo> enemyInfos;
    }

    [Serializable]
    public class MapEvent
    {
        [BoxGroup("Gifted Monster")] public bool hasGiftedMonster;
        [BoxGroup("Gifted Monster"), EnableIf("hasGiftedMonster")] public EnemyInfo giftedMonsterInfo;
        [BoxGroup("Elite Monster")] public bool hasEliteMonster;
        [BoxGroup("Elite Monster"), EnableIf("hasEliteMonster")] public EnemyInfo eliteMonsterInfo;
        [BoxGroup("Monster Wave")] public bool hasMonsterWave;
        [BoxGroup("Monster Wave"), EnableIf("hasMonsterWave")] public EnemyInfo monsterWaveInfo;
        [BoxGroup("Hyper Mode")] public bool startHyperMode;
        [BoxGroup("Hyper Mode")] public bool endHyperMode;
        [BoxGroup("Boss")] public bool isBossEvent;
        [BoxGroup("Boss"), EnableIf("isBossEvent")] public EnemyInfo bossInfo;
    }

    [Serializable]
    public class EnemyInfo
    {
        [TableColumnWidth(60)]
        public int enemyId;
        [TableColumnWidth(60)]
        public int numberOfEnemy;
        [TableColumnWidth(60)]
        public float spawnInterval;

        public void CloneData(EnemyInfo info)
        {
            this.enemyId = info.enemyId;
            this.numberOfEnemy = info.numberOfEnemy;
        }
    }
}
