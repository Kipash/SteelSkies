using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using App.Player.Models;
using System.Linq;

namespace App.Player.Services
{
    public class AppInput
    {
        public bool Debug;
        List<KeyBind> keyBinds = new List<KeyBind>();

        Dictionary<Type, List<KeyBind>> keyBindsOfType = new Dictionary<Type, List<KeyBind>>();

        List<Call> currInputs = new List<Call>();

        public Action<RegisteredKeys[]> OnCallbacksDone;

        internal class Call
        {
            public Action callBack;
            public KeyBind bind;
        }

        bool jammed;
        public void AddBind(KeyBind bind, Type type)
        {
            keyBinds.Add(bind);
            
            if(keyBindsOfType.ContainsKey(type))
            {
                var currBinds = keyBindsOfType[type];
                keyBindsOfType.Remove(type);

                currBinds.Add(bind);

                keyBindsOfType.Add(type, currBinds);
            }
            else
            {
                keyBindsOfType.Add(type, new List<KeyBind>() { bind });
            }
        }
        public void RemoveBind(Type type)
        {
            var keybindsToDelete = keyBindsOfType[type];

            keyBinds.RemoveAll(x => keybindsToDelete.Contains(x));
            keyBindsOfType.Remove(type);
        }
        public void CheckInput()
        {
            if (keyBinds.Count == 0)
                return;

            foreach (var b in keyBinds)
            {
                foreach (var keycode in b.KeyCodes)
                {
                    if (b.CallBackOnPass.OnHold != null && Input.GetKey(keycode))
                        foreach (var m in b.CallBackOnPass.OnHold)
                            AddToInvoke(m, b);
                    else if (b.CallBackOnPass.OnPress != null && Input.GetKeyDown(keycode))
                        foreach (var m in b.CallBackOnPass.OnPress)
                            AddToInvoke(m, b);
                    else if (b.CallBackOnPass.OnRelase != null && Input.GetKeyUp(keycode))
                        foreach (var m in b.CallBackOnPass.OnRelase)
                            AddToInvoke(m, b);
                    else
                        if(b.CallBackOnFail != null)
                            foreach (var m in b.CallBackOnFail)
                                AddToInvoke(m, b);
                }
            }
            if(currInputs.Count != 0)
                Invoke();
            else
                OnCallbacksDone(new RegisteredKeys[0]);
        }
        void AddToInvoke(Action method, KeyBind bind)
        {
            currInputs.Add(new Call() { bind = bind, callBack = method });
        }
        void Invoke()
        {
            var calledKeys = currInputs
                .GroupBy(x => x.bind.Key)
                .Select(x => x.Key)
                .Distinct();

            /*

            var allKeys = Enum.GetNames(typeof(RegisteredKeys))
                .Select(x => Enum.Parse(typeof(RegisteredKeys), x))
                .Cast<RegisteredKeys>();

            var notPressedKeys = allKeys.Except(calledKeys);

            */

            var tempInputs = currInputs.ToArray();

            foreach (var input in tempInputs)
            {
                if(tempInputs.Any(x => x.bind.Key == input.bind.JamKey))
                {
                    var jammedBinds = tempInputs
                        .Where(x => x.bind.Key == input.bind.JamKey)
                        .Select((x) => { currInputs.Remove(x); return x; })
                        .ToArray();

                    if (jammedBinds != null)
                    {
                        foreach (var jammedBind in jammedBinds)
                        {
                            if (jammedBind.bind.CallBackOnFail != null)
                            {
                                foreach (var m in jammedBind.bind.CallBackOnFail)
                                {
                                    m();
                                }
                            }
                        }
                    }
                    
                    currInputs.RemoveAll(x => jammedBinds.Contains(x));
                }
                else
                {
                    if(input.callBack != null)
                        input.callBack();
                }
            }
            OnCallbacksDone(calledKeys.ToArray());

            currInputs.Clear();
        }
    }
}