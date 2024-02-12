using Sirenix.OdinInspector;
using UnityEngine;
using WingsMob.Survival.GameConfig;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Controller
{
    public class ScenarioHandler : MonoBehaviour
    {
        public EnemySpawnManager spawnManager;
        public EventSpawner eventSpawner;
        [ReadOnly] public int waveCount; 
        [ReadOnly] [SerializeField] private ScenarioLevel m_currentScenarioLevel;

        private Scenario m_currentScenario;

        private int m_currentScenarioIndex;

        public void Init(int levelIndex)
        {
            spawnManager.Init();
            StoreCurrentScenario(levelIndex);
            waveCount = 0;
            m_currentScenarioIndex = -1;
        }

        private void StoreCurrentScenario(int levelIndex)
        {
            m_currentScenarioLevel = Resources.Load<ScenarioLevel>("Scenario/" + (GameManager.Instance.RemoteConfigData.IsEasyMode ? "Easy" : "Hard") + "/level_" + (levelIndex+1));
        }

        private void OnEnable()
        {
            this.RegisterListener(MethodNameDefine.OnGameTimeUpdated, OnGameTimeUpdated);
            this.RegisterListener(MethodNameDefine.OnBossFightEnd, OnBossFightEnd);
        }

        private void OnDisable()
        {
            this.RemoveListener(MethodNameDefine.OnGameTimeUpdated, OnGameTimeUpdated);
            this.RemoveListener(MethodNameDefine.OnBossFightEnd, OnBossFightEnd);
        }

        private float m_timePlayed;

        private void OnGameTimeUpdated(object sender = null)
        {
            m_timePlayed = (float)sender;
            if (m_timePlayed >= GetNextScenarioStartTime())
            {
                waveCount++;
                LoadNextScenario();
                DebugExtension.Log("Next Scenario!", Color.red);
                HandleMapEvent();
            }
        }

        public float GetNextScenarioStartTime()
        {
            return IsLastScenario() ?
                Mathf.Infinity : m_currentScenarioLevel.scenarios[m_currentScenarioIndex + 1].startScenarioTime;
        }

        public bool IsLastScenario() => m_currentScenarioIndex >= m_currentScenarioLevel.scenarios.Count - 1;

        public void LoadNextScenario()
        {
            m_currentScenarioIndex++;
            m_currentScenario = m_currentScenarioLevel.scenarios[m_currentScenarioIndex];
        }


        private void HandleMapEvent()
        {
            spawnManager.StopSpawnCoroutines();

            if (m_currentScenario.mapEvent.hasEliteMonster)
            {
                eventSpawner.SpawnElite(m_currentScenario.mapEvent.eliteMonsterInfo);
            }

            if (m_currentScenario.mapEvent.hasMonsterWave)
            {
                eventSpawner.SpawnMonsterWave(m_currentScenario.mapEvent.monsterWaveInfo);
            }

            if (m_currentScenario.mapEvent.hasGiftedMonster)
            {
                eventSpawner.SpawnGiftMonster(m_currentScenario.mapEvent.giftedMonsterInfo);
            }

            if (m_currentScenario.mapEvent.startHyperMode)
            {
                this.PostEvent(MethodNameDefine.OnHyperModeStart);
            }

            if (m_currentScenario.mapEvent.endHyperMode)
            {
                this.PostEvent(MethodNameDefine.OnHyperModeEnd);
            }

            if (m_currentScenario.mapEvent.isBossEvent)
            {
                eventSpawner.SpawnBoss(m_currentScenario.mapEvent.bossInfo);
            }
            else
            {
                spawnManager.ResumeSpawnCoroutines();
            }
            spawnManager.UpdateSpawnInfo(m_currentScenario);
        }

        private void OnBossFightEnd(object sender = null)
        {
            if (IsLastScenario())
            {
                if (LevelManager.Instance.levelId < SurvivorConfig.MaxGameLevel
                    && ProfileManager.Instance.PlayerUnlockedLevel <= LevelManager.Instance.levelId)
                    ProfileManager.Instance.PlayerUnlockedLevel = LevelManager.Instance.levelId + 1;

                StartCoroutine(CoroutineUtils.DelayCallback(2f, () => LevelManager.Instance.gameStateManager.GameWin()));
            }
            else
            {
                spawnManager.ResumeSpawnCoroutines();
            }
        }
    }
}