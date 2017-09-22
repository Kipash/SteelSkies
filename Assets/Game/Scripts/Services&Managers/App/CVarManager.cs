using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;

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
        object value;
        int intValue;
        bool boolValue;

        PropertyInfo[] props;
        PropertyInfo prop;

        public void SetCvar(string name, string s)
        {
            value = null;

            props = typeof(ConsoleVariables).GetProperties();
            prop = props
                    .Where(x => x.Name.ToLower() == name.ToLower())
                    .FirstOrDefault();

            if (prop != null && !string.IsNullOrEmpty(s))
            {
                Console.Console.WriteLine(string.Format("n:{0} par:({1}) type:{2}", name, s, prop.PropertyType));
                

                if (prop.CanWrite && prop.CanRead)
                {
                    if(prop.PropertyType == typeof(Int32))
                    {
                        if(int.TryParse(s, out intValue))
                        {
                            value = intValue;
                        }
                        else
                        {
                            Console.Console.PrintErrorMessage("(" + s + ") is not a number!");
                            return;
                        }
                    }
                    else if (prop.PropertyType == typeof(Boolean))
                    {
                        //Debug.Log(s.Length);
                        if (s.Length == 1)
                        {
                            if(s[0] == '0' || s[0] == '1')
                            {
                                boolValue = s[0] =='1' ? true : false;
                                value = boolValue;
                            }
                            else
                            {
                                Console.Console.PrintErrorMessage("(" + s + ") is not 1/0!");
                            }
                        }
                        else
                        {
                            if (bool.TryParse(s, out boolValue))
                            {
                                value = boolValue;
                            }
                            else
                            {
                                Console.Console.PrintErrorMessage("(" + s + ") is not a true/false!");
                                return;
                            }
                        }
                    }
                    else if (prop.PropertyType == typeof(String))
                    {
                        value = s;
                    }
                    else
                    {
                        Console.Console.PrintErrorMessage("Unsupported Type!  (" + prop.PropertyType + ")");
                    }

                    prop.SetValue(cvars, value, null);
                    Console.Console.WriteLine("Setting " + name + " to " + prop.GetValue(cvars, null));
                }
                else
                    Console.Console.PrintErrorMessage("Access denied!");
            }
            else
            {
                Console.Console.PrintErrorMessage("Wrong name or parameters!");
            }
        }
        public void GetCvar(string name)
        {
            props = typeof(ConsoleVariables).GetProperties();
            prop = props
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
            props = typeof(ConsoleVariables).GetProperties();

            Console.Console.WriteLine(" - Available Cvars - ");
            Console.Console.WriteLine(string.Format("{0,-20}   {1,-20}   {2}", "Key", "Type","Access"));
            Console.Console.WriteLine(@"---------------------*------------------------*------------------------");
            foreach (var x in props)
            {
                Console.Console.WriteLine(string.Format(
                    "{0,-20}   {1,-20}   {2}",
                    x.Name,
                    x.PropertyType.ToString(),
                    (x.CanRead ? "Read " : "") + (x.CanWrite ? "Write " : "")));
            }
            Console.Console.WriteLine(@"---------------------*------------------------*------------------------");
        }
    }
}