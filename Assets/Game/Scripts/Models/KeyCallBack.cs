using UnityEngine;
using System.Collections;
using System;

namespace App.Player.Models
{
    public class KeyCallBack
    {
        public Action[] OnPress;
        public Action[] OnHold;
        public Action[] OnRelase;
    }
}