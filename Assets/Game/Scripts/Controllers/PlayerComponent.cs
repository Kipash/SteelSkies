using UnityEngine;
using System.Collections;
using System;
using MovementEffects;

namespace Aponi
{
    public class PlayerComponent : Entity
    {
        [SerializeField] PlayerMotor motor;
        [SerializeField] WeaponManager weaponManager;
        [SerializeField] PlayerEffects playerEffects;

        [Header("Player")]
        [SerializeField]
        int defaultHP;
        [SerializeField] int damageOnCollision;

        bool isAlive;
        public bool IsAlive
        {
            get { return isAlive; }
            set
            {
                isAlive = value;
                motor.Disabled = !value;
                weaponManager.Disabled = !value;
                playerEffects.Disabled = !value;
            }
        }

        private void Start()
        {
            Timing.Instance.AddTag(GetType().ToString(), false);

            RegisterCallBacks();

            weaponManager.Start();
            playerEffects.Start();

            SetDefaultHP(0);

            GameServices.Instance.GameManager.OnGameStart += Spawn;

            DisablePlayer(true);
            GameServices.Instance.GameUIManager.PlayerHealth.SetImageDial(Health);

            motor.colCallback += OnCrash;
        }
        private void Update()
        {
            if (IsAlive)
            {
                //playerEffects.SetTransition(weaponManager.GetCurrentWarmUp);
                playerEffects.Update();
                motor.Update();
            }
        }

        private void FixedUpdate()
        {
            if (IsAlive)
            {
                motor.FixedUpdate();
            }
        }

        private void OnDestroy()
        {
            UnRegisterCallbacks();
        }

        void RegisterCallBacks()
        {
            #region PlayerMotor

            AppServices.Instance.AppInput.OnCallbacksDone += motor.PlayerMotor_OnCallbacksDone;

            AppServices.Instance.AppInput.AddBind(new KeyBind()
            {
                Name = "Move Left",
                KeyCodes = CurrentBinds.BindedKeys[RegisteredKeys.MoveLeft],

                Key = RegisteredKeys.MoveLeft,
                JamKey = RegisteredKeys.MoveRight,

                CallBackOnPass = new KeyCallBack()
                {
                    OnHold = new Action[] { motor.MoveLeft }
                },
            },
            typeof(PlayerComponent));

            AppServices.Instance.AppInput.AddBind(new KeyBind()
            {
                Name = "Move Right",
                KeyCodes = CurrentBinds.BindedKeys[RegisteredKeys.MoveRight],

                Key = RegisteredKeys.MoveRight,
                JamKey = RegisteredKeys.MoveLeft,

                CallBackOnPass = new KeyCallBack()
                {
                    OnHold = new Action[] { motor.MoveRight }
                },
            },
            typeof(PlayerComponent));

            AppServices.Instance.AppInput.AddBind(new KeyBind()
            {
                Name = "Move Up",
                KeyCodes = CurrentBinds.BindedKeys[RegisteredKeys.MoveUp],

                Key = RegisteredKeys.MoveUp,
                JamKey = RegisteredKeys.MoveDown,

                CallBackOnPass = new KeyCallBack()
                {
                    OnHold = new Action[] { motor.MoveUp }
                },
            },
            typeof(PlayerComponent));

            AppServices.Instance.AppInput.AddBind(new KeyBind()
            {
                Name = "Move Down",
                KeyCodes = CurrentBinds.BindedKeys[RegisteredKeys.MoveDown],

                Key = RegisteredKeys.MoveDown,
                JamKey = RegisteredKeys.MoveUp,

                CallBackOnPass = new KeyCallBack()
                {
                    OnHold = new Action[] { motor.MoveDown }
                },
            },
            typeof(PlayerComponent));

            #endregion
        }

        void UnRegisterCallbacks()
        {
            if (AppServices.Instance != null)
                AppServices.Instance.AppInput.RemoveBind(typeof(PlayerComponent));
        }

        void Spawn()
        {
            IsAlive = true;
            SetDefaultHP(defaultHP);
            gameObject.SetActive(true);
            GameServices.Instance.GameUIManager.PlayerHealth.SetImageDial(Health);

            //~~
            Camera.main.ResetProjectionMatrix();
            Camera.main.ResetWorldToCameraMatrix();
        }

        public override void Die()
        {
            if (IsAlive)
                DisablePlayer();
        }

        void DisablePlayer(bool withoutEffects = false)
        {
            if(withoutEffects)
                playerEffects.DieEffect();
            IsAlive = false;
            gameObject.SetActive(false);

            GameServices.Instance.GameManager.GameOver();
        }

        public override void Hit(int damage)
        {
            if (IsAlive)
            {
                playerEffects.HitEffect();
                base.Hit(damage);
                GameServices.Instance.GameUIManager.PlayerHealth.SetImageDial(Health);
            }
        }
        void OnCrash(GameObject go)
        {
            if (!Invisible)
            {
                if (go.CompareTag("Enemy"))
                {
                    var c1 = go.GetComponentInChildren<EnemyComponent>();
                    var c2 = go.GetComponentInParent<EnemyComponent>();
                    var c = (c1 == null ? c2 : c1);
                    if (c != null)
                    {
                        c.Crash(damageOnCollision);
                        Crash(c.DamageOnCollision);
                    }
                    else
                    {
                        UnityEngine.Debug.LogErrorFormat("GameObject '{0}'(parent:{1}, active:{2}) should have a IHitteble in it's hierarchy!",
                            go.name, go.transform.parent, go.activeInHierarchy);
                    }
                }
            }
        }
    }
}