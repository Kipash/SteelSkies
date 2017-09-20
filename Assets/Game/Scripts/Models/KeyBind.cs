using UnityEngine;
using System.Collections;
using System;

namespace Aponi
{
    public class KeyBind
    {
        public string Name;
        public RegisteredKeys Key;
        public KeyCode[] KeyCodes;
        public RegisteredKeys JamKey;
        public KeyCallBack CallBackOnPass;
        public Action[] CallBackOnFail;
    }
}