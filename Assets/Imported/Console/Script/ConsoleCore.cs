using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Console
{
    public class ConsoleCore
    {
        public Dictionary<string, Command> Methods { get; private set; }
        Action<string, bool> writeLine;

        public ConsoleCore(Action<string,bool> writeLine)
        {
            if (writeLine == null)
                throw new ArgumentNullException("writeLine");

            this.writeLine = writeLine;
            Methods = new Dictionary<string, Command>();
        }

        public void AddStatic(Type type)
        {
            var mis = type.GetMethods(BindingFlags.Static | BindingFlags.Public);

            foreach (var mi in mis)
            {
                Add(mi.Name, mi, type, null, false);
            }
        }
        public void AddStatic(string name, Type type, string methodName, bool argsToArray = false)
        {
            var mi = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);

            Add(name, mi, type, null, argsToArray);
        }
        public void Add(string name, object source, string methodName, bool argsToArray = false)
        {
            Type type = source.GetType();

            var mi = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

            Add(name, mi, type, source, argsToArray);
        }
        void Add(string key, MethodInfo mi, Type srcType, object src, bool argsToArray)
        {
            Methods.Add(key.ToLower(), new Command
            {
                Key = key,
                SourceType = srcType,
                Source = src,
                Method = mi,
                ArgsToArray = argsToArray,
            });
        }

        public void Invoke(string commandLine)
        {
            var cmds = commandLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var res = Invoke(cmds.First(), cmds.Skip(1).ToArray());

            if (res != null)
                writeLine(res.ToString(), true);
        }

        public object Invoke(string name, object[] args)
        {
            var m = Methods[name.ToLower()];

            if (m.ArgsToArray)
            {
                return m.Method.Invoke(m.Source, new object[] { args });
            }
            return m.Method.Invoke(m.Source, args);
        }
    }
}
