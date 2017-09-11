using UnityEngine;
using System.Collections;
using App.Player.Services;
using App.Player.Models;
using System;

public class PlayerComponent : Entity
{
    [SerializeField] PlayerMotor motor; 
    [SerializeField] WeaponManager weaponManager;
    [SerializeField] PlayerEffects playerEffects;

    [Header("Player")]
    [SerializeField] int defaultHP;

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
        RegisterCallBacks();

        weaponManager.Start();
        playerEffects.Start();

        SetDefaultHP(0);

        GameServices.Instance.GameManager.OnGameStart += Spawn;

        DisablePlayer();
        GameServices.Instance.GameUIManager.PlayerHealth.SetImageDial(Health);
    }
    private void Update()
    {
        if (IsAlive)
        {
            //playerEffects.SetTransition(weaponManager.GetCurrentWarmUp);
            playerEffects.Update();
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
        if(AppServices.Instance != null)
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

    void DisablePlayer()
    {
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
}
