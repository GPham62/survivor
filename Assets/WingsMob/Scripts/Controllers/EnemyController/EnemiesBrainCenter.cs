using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.Controller
{
    public class EnemiesBrainCenter : MonoBehaviour
    {
        [Header("Enemies Settings")]
        [SerializeField] private int maxEnemies = 150;
        [ReadOnly] public int spawnCount;
        [ReadOnly] public EnemyController[] enemiesArr;
        [ReadOnly] public Vector3[] enemiesPos;

        public void Init()
        {
            enemiesArr = new EnemyController[maxEnemies];
            enemiesPos = new Vector3[maxEnemies];
            spawnCount = 0;
        }
        private void OnEnable()
        {
            this.RegisterListener(MethodNameDefine.OnBossFightStart, KillAllEnemies);
        }

        private void OnDisable()
        {
            this.RemoveListener(MethodNameDefine.OnBossFightStart, KillAllEnemies);
        }

        private void KillAllEnemies(object obj)
        {
            foreach (var enemy in enemiesArr)
            {
                if (enemy != null)
                    enemy.fighter.TakeLethalDamage();
            }
        }

        public void AddEnemyToArr(EnemyController enemyToAdd)
        {
            int emptyIndex = Array.FindIndex(enemiesArr, i => i == null);
            if (emptyIndex != -1)
            {
                spawnCount++;
                enemiesArr[emptyIndex] = enemyToAdd;
            }
        }

        public void RemoveEnemyFromArr(EnemyController enemyToRemove)
        {
            if (Array.IndexOf(enemiesArr, enemyToRemove) != -1)
            {
                enemiesArr[Array.IndexOf(enemiesArr, enemyToRemove)] = null;
                spawnCount--;
            }
            else
            {
                Common.Log("Something WRONG!");
            }
        }

        public bool IsFull() => Array.FindIndex(enemiesArr, i => i == null) == -1;

        public EnemyController FindEnemyInArr(EnemyController enemy) => enemiesArr.FirstOrDefault(e => e = enemy);

        private void FixedUpdate()
        {
            if (GameStatus.CurrentState != GameState.Playing)
            {
                foreach (var enemy in enemiesArr)
                    if (enemy != null && enemy.mover.IsMoving())
                        enemy.mover.StopMoving();
            }
            else
            {
                for (int i = 0; i < enemiesArr.Length; i++) 
                {
                    if (enemiesArr[i] != null)
                    {
                        enemiesArr[i].Act();
                        enemiesPos[i] = enemiesArr[i].mover.transform.position;
                    }
                    else
                    {
                        enemiesPos[i] = LevelManager.Instance.playerController.mover.transform.position;
                    }
                }
            }
        }
    }
}