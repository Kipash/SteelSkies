using UnityEngine;
using System;
using System.Collections;
using System.Linq;

namespace Aponi
{
    [Serializable]
    public class CVarManager
    {
        ConsoleVariables cvars = new ConsoleVariables();

        public void Initialize()
        {
            Console.Console.Add("Set", this, "SetCvar");
            Console.Console.Add("Get", this, "GetCvar");
            Console.Console.Add("Cvars", this, "ListScvars");
        }

        public void SetCvar(string name, string val)
        {
            var props = typeof(ConsoleVariables).GetProperties();
            var prop = props
                        .Where(x => x.Name.ToLower() == name.ToLower())
                        .FirstOrDefault();

            if (prop != null)
            {
                if (prop.CanWrite && prop.CanRead)
                {
                    prop.SetValue(cvars, val, null);
                    Console.Console.WriteLine("Setting " + name + " to " + prop.GetValue(cvars, null));
                }
                else
                    Console.Console.PrintErrorMessage("Access denied!");
            }
            else
            {
                Console.Console.PrintErrorMessage("Wrong name!");
            }
        }
        public void GetCvar(string name)
        {
            var props = typeof(ConsoleVariables).GetProperties();
            var prop = props
                        .Where(x => x.Name.ToLower() == name.ToLower())
                        .FirstOrDefault();

            if (prop != null)
            {
                if(prop.CanRead)
                    Console.Console.WriteLine(name + " = " + prop.GetValue(cvars, null));
                else
                    Console.Console.PrintErrorMessage("Access denied!");
            }
            else
                Console.Console.PrintErrorMessage("Wrong name!");
        }
        public void ListScvars()
        {
            var props = typeof(ConsoleVariables).GetProperties();

            Console.Console.WriteLine(" - Available Cvars - ");
            Console.Console.WriteLine(string.Format("{0,-20}   {1}", "Key", "Access"));
            Console.Console.WriteLine(@"---------------------*------------------------");
            foreach (var x in props)
            {
                Console.Console.WriteLine(string.Format(
                    "{0,-20}   {1}",
                    x.Name,
                    (x.CanRead ? "Read " : "") + (x.CanWrite ? "Write " : "")));
            }
            Console.Console.WriteLine(@"---------------------*------------------------");
            Console.Console.WriteLine("");
        }
    }
}