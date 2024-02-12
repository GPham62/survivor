namespace WingsMob.Survival.Controller
{
    public class EnemyShotController : EnemyController
    {
        public UbhShotCtrl shotCtrl;

        public override void Init()
        {
            base.Init();
            shotCtrl.Init(baseStats.GetBaseStat(Global.CharacterStats.AttackBase));
        }

        protected override void OnEnable()
        {
            HandleInput(Combat.State.EnemyInput.ShootPlayer, null);
            animationController.Reset();
            fighter.Reset(baseStats);
        }
    }
}