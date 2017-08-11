using UnityEngine;
using System.Collections;
using App.Player.Services;
using App.Player.Models;
using System;

public class PlayerComponent : MonoBehaviour
{
    [SerializeField] PlayerMotor motor;
    [SerializeField] WeaponManager weaponManager;

    private void Start()
    {
        RegisterCallBacks();

        weaponManager.Start();
    }
    private void Update()
    {
        //motor.Update();
    }
    private void FixedUpdate()
    {
        motor.FixedUpdate();
    }


    void RegisterCallBacks()
    {
        #region PlayerMotor

        AppInput.Instance.OnCallbacksDone += motor.PlayerMotor_OnCallbacksDone;

        AppInput.Instance.AddBind(new KeyBind()
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

        AppInput.Instance.AddBind(new KeyBind()
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

        AppInput.Instance.AddBind(new KeyBind()
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

        AppInput.Instance.AddBind(new KeyBind()
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

        AppInput.Instance.AddBind(new KeyBind()
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



}
