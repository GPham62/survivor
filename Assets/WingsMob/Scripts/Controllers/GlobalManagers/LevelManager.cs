using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using WingsMob.Survival.Environment;
using WingsMob.Survival.Environment.Map;
using WingsMob.Survival.Global;
using WingsMob.Survival.Stats;

namespace WingsMob.Survival.Controller
{
    public class LevelManager : MonoBehaviour
    {
        [Title("Level Settings")]
        public int levelId;

        [Title("Spawners Reference")]
        public MapManager mapManager;
        public CollectableSpawner collectableSpawner;

        [TitleGroup("Managers References", BoldTitle = true)]
        [BoxGroup("Managers References/GamePlay")] public PlayerController playerController;
        [BoxGroup("Managers References/GamePlay")] public EnemiesBrainCenter brainCenter;
        [BoxGroup("Managers References/GamePlay")] public ScenarioHandler scenarioHandler;
        [BoxGroup("Managers References/GamePlay")] public GameStateManager gameStateManager;
        [BoxGroup("Managers References/GamePlay")] public GameStats gameStats;
        [BoxGroup("Managers References/GamePlay")] public SkillCollection skillCollection;
        [BoxGroup("Managers References/UI")] public LevelUIManager levelUIManager;

        [Title("Environment")]
        public EnemyDetector enemyDetector;
        public bool isVerticalMap;

        [Title("GMTool")]
        [SerializeField] GameObject m_prefabGMTool;

        [HideInInspector]
        public Camera mainCam;

        private static LevelManager m_instance = null;
        public static LevelManager Instance => m_instance;

        private void Awake()
        {
            m_instance = this;
        }

        private void Start()
        {
            levelId = ProfileManager.Instance.CurrentLevel;
            Setup();
            TrackingManager.Instance.LogEvent(TrackingEventName.LEVEL_START, levelId.ToString());

            if (WMGameConfig.Instance.IsDebugMode)
            {
                Time.timeScale = GameManager.Instance.RemoteConfigData.GameSpeed;
            }
        }

        private void Setup()
        {
            AssignVariables();
            InitManagers();
            InitEnvironment();
        }

        private void AssignVariables()
        {
            DOTween.SetTweensCapacity(500, 125);
            mainCam = Camera.main;
        }

        private void InitManagers()
        {
            levelUIManager.Init();
            brainCenter.Init();
            playerController.Init();
            skillCollection.Init();
            scenarioHandler.Init(levelId);
            gameStats.Init();
        }

        private void InitEnvironment()
        {
            //SoundManager.Instance.PlaySoundInstance(MusicNameDefine.GetBackgroundSound());
            mapManager.SpawnMiniPool(levelId);
            mapManager.SpawnMapByLevel(levelId);
        }

        public float? GetRightBorderMap()
        {
            if (!isVerticalMap) return null;
            return mapManager.GetMap().GetChild(0).GetComponent<SurvivorTile>().GetSize() / 2;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (WMGameConfig.Instance.IsDebugMode)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    gameStats.IncreaseGameStats(GamePlayStats.Exp, gameStats.expToLevelUp - gameStats.exp);
                }
                Time.timeScale = GameManager.Instance.RemoteConfigData.GameSpeed;
            }
#endif
        }
    }
}
