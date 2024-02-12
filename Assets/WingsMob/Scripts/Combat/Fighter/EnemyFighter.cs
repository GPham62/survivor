using UnityEngine;
using UnityEngine.Events;
using WingsMob.Survival.Combat.State;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;
using WingsMob.Survival.Stats;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Combat
{
    public class EnemyFighter : Fighter, IAttacker
    {
        [SerializeField] private Collider2D m_triggerCollider;
        [SerializeField] private Collider2D m_blockCollider;
        private EnemyController m_enemyController;
        public UnityEvent dieCallback; 

        public void InitEnemy(EnemyController enemyController)
        {
            m_enemyController = enemyController;
            base.Init(m_enemyController.animationController, m_enemyController.baseStats);
        }
        
        public override void Reset(BaseStats stats)
        {
            base.Reset(stats);
            m_triggerCollider.enabled = true;
            m_blockCollider.enabled = true;
        }

        protected override void DisplayDamage(int damage)
        {
            GameAssets.Instance.CreateDamagePopup(transform.position, damage);
        }

        public override void Die()
        {
            base.Die();
            m_enemyController.HandleInput(State.EnemyInput.Death);
            m_triggerCollider.enabled = false;
            m_blockCollider.enabled = false;
            LevelManager.Instance.gameStats.IncreaseGameStats(GamePlayStats.KillCount);
            //SoundManager.Instance.PlaySound(MusicNameDefine.GetCombatEnemyDead());
            dieCallback?.Invoke();
        }

        public float GetDamage()
        {
            if (m_health > 0)
            {
                if (m_critRate > 0)
                    if (Random.Range(1, 101) < m_critRate * 100)
                        if (m_critDamage > 0)
                            return m_damage * m_critDamage;
                return m_damage;
            }
            else return 0;
        }

        public override void KnockBack(Vector2 direction, float strength, float duration)
        {
            var prevState = m_enemyController.currentState;
            m_enemyController.HandleInput(EnemyInput.Stun);
            GetComponent<Rigidbody2D>().AddForce(direction * strength, ForceMode2D.Impulse);
            StartCoroutine(CoroutineUtils.DelayCallback(duration, () => {
                if (!isDead)
                    m_enemyController.HandleState(prevState);
            }));
        }

        public void OnInteractWithTarget()
        {
        }
    }
}