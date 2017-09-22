using UnityEngine;
using System.Collections;
using System;
//using MovementEffects;

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

            CheckInput();
        }

        void CheckInput()
        {
            if(AppServices.Instance.AppInput.GetKey(KeyActions.MoveLeft, KeyState.Press))
            {
                motor.MoveLeft();
            }
            else if (AppServices.Instance.AppInput.GetKey(KeyActions.MoveRight, KeyState.Press))
            {
                motor.MoveRight();
            }
            else
            {
                motor.ResetHorizontal();
            }

            if (AppServices.Instance.AppInput.GetKey(KeyActions.MoveUp, KeyState.Press))
            {
                motor.MoveUp();
            }
            else if (AppServices.Instance.AppInput.GetKey(KeyActions.MoveDown, KeyState.Press))
            {
                motor.MoveDown();
            }
            else
            {
                motor.ResetVertical();
            }

               
        }

        private void FixedUpdate()
        {
            if (IsAlive)
            {
                motor.FixedUpdate();
            }
        }

        /*
        void RegisterCallBacks()
        {
            #region PlayerMotor

            AppServices.Instance.AppInput.OnCallbacksDone += motor.PlayerMotor_OnCallbacksDone;

            AppServices.Instance.AppInput.AddBind(new KeyBind()
            {
                Name = "Move Left",
                KeyCodes = CurrentBinds.BindedKeys[KeyActions.MoveLeft],

                Key = KeyActions.MoveLeft,
                JamKey = KeyActions.MoveRight,

                CallBackOnPass = new KeyCallBack()
                {
                    OnHold = new Action[] { motor.MoveLeft }
                },
            },
            typeof(PlayerComponent));

            AppServices.Instance.AppInput.AddBind(new KeyBind()
            {
                Name = "Move Right",
                KeyCodes = CurrentBinds.BindedKeys[KeyActions.MoveRight],

                Key = KeyActions.MoveRight,
                JamKey = KeyActions.MoveLeft,

                CallBackOnPass = new KeyCallBack()
                {
                    OnHold = new Action[] { motor.MoveRight }
                },
            },
            typeof(PlayerComponent));

            AppServices.Instance.AppInput.AddBind(new KeyBind()
            {
                Name = "Move Up",
                KeyCodes = CurrentBinds.BindedKeys[KeyActions.MoveUp],

                Key = KeyActions.MoveUp,
                JamKey = KeyActions.MoveDown,

                CallBackOnPass = new KeyCallBack()
                {
                    OnHold = new Action[] { motor.MoveUp }
                },
            },
            typeof(PlayerComponent));

            AppServices.Instance.AppInput.AddBind(new KeyBind()
            {
                Name = "Move Down",
                KeyCodes = CurrentBinds.BindedKeys[KeyActions.MoveDown],

                Key = KeyActions.MoveDown,
                JamKey = KeyActions.MoveUp,

                CallBackOnPass = new KeyCallBack()
                {
                    OnHold = new Action[] { motor.MoveDown }
                },
            },
            typeof(PlayerComponent));

            #endregion
        }
        */

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
        EnemyComponent e1;
        EnemyComponent e2;
        EnemyComponent e;
        void OnCrash(GameObject go)
        {
            if (!Invisible)
            {
                if (go.CompareTag("Enemy"))
                {
                    e1 = go.GetComponentInChildren<EnemyComponent>();
                    e2 = go.GetComponentInParent<EnemyComponent>();
                    e = (e1 == null ? e2 : e1);
                    if (e != null)
                    {
                        e.Crash(damageOnCollision);
                        Crash(e.DamageOnCollision);
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