using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SteelSkies
{
    [Serializable]
    public class AppInput
    {
        public Dictionary<int, int[]> Keys = new Dictionary<int, int[]>();

        public static bool Disable;

        public Action AnyKeyDown;

        KeyActions key;
        string keyCodeName;
        KeyCode keyCode;
        KeyCode[] kCodes;
        string keyName;
        int[] keys;
        int i;


        public void Initialize()
        {
            //Debug.Console.Add("bind", this, "EditBind");

            
            Add(KeyActions.MoveUp, new KeyCode[] { KeyCode.UpArrow, KeyCode.W });
            Add(KeyActions.MoveDown, new KeyCode[] { KeyCode.DownArrow, KeyCode.S });
            Add(KeyActions.MoveLeft, new KeyCode[] { KeyCode.LeftArrow, KeyCode.A });
            Add(KeyActions.MoveRight, new KeyCode[] { KeyCode.RightArrow, KeyCode.D });
        }

        public void Add(KeyActions k, KeyCode[] codes)
        {
            Keys.Add((int)k, codes.Cast<int>().ToArray());
        }

        public void EditBind(string rawKey, string rawKeyCode)
        {
            key = IsKey(rawKey);
            keyCodeName = Enum.GetNames(typeof(KeyCode))
                                    .FirstOrDefault(x => x.ToLower() == rawKeyCode.ToLower());
            if (!string.IsNullOrWhiteSpace(keyCodeName))
            {
                keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), keyCodeName);

                kCodes = Keys[(int)key].Cast<KeyCode>().ToArray();
                Keys.Remove((int)key);
                Add(key, (new KeyCode[] { keyCode }).Concat(kCodes).ToArray());
            }
        }
        KeyActions IsKey(string rawKey)
        {
            keyName = Enum.GetNames(typeof(KeyActions))
                        .FirstOrDefault(x => x.ToLower() == rawKey.ToLower());
            if (!string.IsNullOrWhiteSpace(keyName))
            {
                return (KeyActions)Enum.Parse(typeof(KeyActions), keyName);
            }
            else
                Console.Console.PrintErrorMessage("No such key as " + rawKey + " is found!");

            return KeyActions.none;
        }

        public bool GetKey(KeyActions k, KeyState s)
        {
            if (Disable)
                return false;

            keys = HaveBinding((int)k);
            for (i = 0; i < keys.Length; i++)
            {
                switch (s)
                {
                    case KeyState.Initial:
                        if (Input.GetKeyDown((KeyCode)keys[i]))
                            return true;
                        break;
                    case KeyState.Press:
                        if (Input.GetKey((KeyCode)keys[i]))
                            return true;
                        break;
                    case KeyState.End:
                        if (Input.GetKeyUp((KeyCode)keys[i]))
                            return true;
                        break;
                    default:
                        UnityEngine.Debug.LogError("Keystate == none!");
                        break;
                }
            }
            return false;
        }

        private int[] HaveBinding(int pButton)
        {
            if (Keys.TryGetValue(pButton, out keys))
            {
                return keys;
            }
            UnityEngine.Debug.LogError(string.Format("Empty field of KeyCodes!"));
            return null;
        }

        public void CheckAnyKey()
        {
            if (AnyKeyDown == null)
                return;
            if (Input.anyKeyDown)
                AnyKeyDown?.Invoke();
        }
    }
}

/*
namespace Aponi
{
    public class AppInput
    {
        public bool Debug;
        List<KeyBind> keyBinds = new List<KeyBind>();

        Dictionary<Type, List<KeyBind>> keyBindsOfType = new Dictionary<Type, List<KeyBind>>();

        List<Call> currInputs = new List<Call>();

        public Action<RegisteredKeys[]> OnCallbacksDone;
        public Action AnyKey;
        public Action AnyKeyDown;

        public static bool Disable;

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
            if (Disable)
                return;

            if (Input.anyKeyDown)
                AnyKeyDown?.Invoke();

            if (Input.anyKey)
                AnyKey?.Invoke();

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

            

            //var allKeys = Enum.GetNames(typeof(RegisteredKeys))
            //    .Select(x => Enum.Parse(typeof(RegisteredKeys), x))
            //    .Cast<RegisteredKeys>();
            //
            //var notPressedKeys = allKeys.Except(calledKeys);

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
*/
