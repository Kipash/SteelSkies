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

    private void Start()
    {
        RegisterCallBacks();

        weaponManager.Start();
        playerEffects.Start();

        SetDefaultHP(defaultHP);
    }
    private void Update()
    {
        playerEffects.SetTransition(weaponManager.GetCurrentWarmUp);
        playerEffects.Update();
    }

    private void FixedUpdate()
    {
        motor.FixedUpdate();
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

        #region WeaponManager

        AppServices.Instance.AppInput.AddBind(new KeyBind()
        {
            Name = "Shoot",
            KeyCodes = CurrentBinds.BindedKeys[RegisteredKeys.Fire],

            Key = RegisteredKeys.Fire,
            JamKey = RegisteredKeys.none,

            CallBackOnPass = new KeyCallBack()
            {
                OnPress = new Action[] { weaponManager.PrewarmShot },
                OnRelase = new Action[] { weaponManager.Shoot }
            },
        },
        typeof(PlayerComponent));

        #endregion
    }

    void UnRegisterCallbacks()
    {
        AppServices.Instance.AppInput.RemoveBind(typeof(PlayerComponent));
    }

    public override void Die()
    {
        motor.Disabled = true;
        playerEffects.DieEffect();
        GameServices.Instance.GameManager.ResetLevel(0.5f);
        gameObject.SetActive(false);
    }
    public override void Hit(int damage)
    {
        playerEffects.HitEffect();
        base.Hit(damage);
    }
}
