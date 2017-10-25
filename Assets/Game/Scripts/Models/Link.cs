using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteelSkies
{
    [Serializable]
    public class Link
    {
        public SwitchElement Switcher;
        public SwitchElementTarget Target;
        public string Name;
    }
}
