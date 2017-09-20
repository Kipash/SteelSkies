using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Console
{
    public class Command
    {
        public string Key { get; set; }
        public object Source { get; set; }
        public Type SourceType { get; set; }

        public MethodInfo Method { get; set; }

        public bool ArgsToArray { get; set; }
    }
}
