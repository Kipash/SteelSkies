﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aponi
{
    [Serializable]
    public class Link
    {
        public SwitchElement Switcher;
        public SwitchElementTarget Target;
        public string Name;
    }
}
