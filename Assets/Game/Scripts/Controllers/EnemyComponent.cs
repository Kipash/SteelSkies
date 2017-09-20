using UnityEngine;
using System.Collections;
using MovementEffects;

namespace Aponi
{
    public class EnemyComponent : Entity
    {
        [SerializeField] EnemyMotor motor;
        [SerializeField] EnemyWeaponry weaponry;
        [SerializeField] EnemyEffects effects;


        [Header("Misc")]
        [SerializeField]
        bool Disable;

        [Header("Health")]
        [SerializeField]
        int defaultHP;
        public int DamageOnCollision;

        public PathSettings PathSettings
        {
            get { return motor.pathSettings; }
            set { motor.pathSettings = value; motor.GetPath(); }
        }

        private void OnEnable()
        {
            Timing.Instance.AddTag(GetType().ToString(), false);
            SetDefaultHP(defaultHP);

            motor.OnDisable = null;
            motor.OnDisable += () => { Deactivate(false); };
            motor.Start();

            effects.Start();

            if (GameServices.Instance != null && AppServices.Initiliazed)
                weaponry.Target = GameServices.Instance.GameManager.Player.transform;

            weaponry.Start();
        }
        private void OnDisable()
        {
            effects.Deactivate();
            weaponry.Deactivate();
        }
        private void Update()
        {
            motor.Update();
            weaponry.RotateTowers();
        }
        public override void Die()
        {
            effects.DieEffect(transform.position);
            Deactivate(true);
        }
        void Deactivate(bool natural)
        {
            motor.OnDisable = null;
            GameServices.Instance.ChallengeManager.DeactivateEntity(gameObject, natural);
            SetDefaultHP(defaultHP);

            OnDisable();
        }
        public override void Hit(int damage)
        {
            base.Hit(damage);
            effects.HitEffect();
        }
    }
}