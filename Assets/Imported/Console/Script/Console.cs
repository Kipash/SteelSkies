using System;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Console
{
    public static class Console
    {
        /*
        static InputField inputField;
        static Text log;
        static Char commandChar = '/';
        static string startString = "";


        static bool showTime;
        static Color typedCommandColor;
        static Color errorCommandColor;
        static Color defaultTextColor;
        static Color timeTextColor;
        */
        static ConsoleData data;
        static ConsoleCore consoleBase;

        public static void Initialize(ConsoleData data)
        {
            Console.data = data;

            consoleBase = new ConsoleCore(WriteLine) { };
        }

        public static void Add(string name, object source, string methodName, bool argsToArray = false)
        {
            consoleBase.Add(name, source, methodName, argsToArray);
        }
        public static void AddStatic(Type type)
        {
            consoleBase.AddStatic(type);
        }
        public static void AddStatic(string name, Type type, string methodName, bool argsToArray = false)
        {
            consoleBase.AddStatic(name, type, methodName, argsToArray);
        }
        public static void Execute()
        {
            data.ConsoleInputField.ActivateInputField();
            if (string.IsNullOrEmpty(data.ConsoleInputField.text)) { }
            else if (data.ConsoleInputField.text[0] == data.CommandChar && data.ConsoleInputField.text != data.CommandChar.ToString())
            {
                SearchCommand(data.ConsoleInputField.text);
            }
            else
            {
                WriteLine(data.ConsoleInputField.text, true);
            }
            data.ConsoleInputField.text = "";
        }
        static void SearchCommand(string rawCommand)
        {
            WriteLine(" " + RitchTextHelper.Combine(rawCommand, RitchTextHelper.ColorToHex(data.TypedCommandColor), false, true));
            rawCommand = rawCommand.Substring(1, data.ConsoleInputField.text.Length - 1);
            if (string.IsNullOrEmpty(rawCommand))
                WriteLine(RitchTextHelper.Combine("Unrecognized command!", RitchTextHelper.ColorToHex(data.ErrorCommandColor), true, false));
            else
            {
                try
                {
                    consoleBase.Invoke(rawCommand);
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(KeyNotFoundException) || e.GetType() == typeof(KeyNotFoundException))
                    {
                        global::Console.Console.PrintErrorMessage("Unknown command!");
                    }
                    else
                    {
                        var be = e.GetBaseException();
                        WriteLine(RitchTextHelper.Combine("Command error! ", RitchTextHelper.ColorToHex(data.ErrorCommandColor), true, false)
                            + "(Short): " + be.Message);

                        UnityEngine.Debug.LogException(e);
                    }
                }
            }
        }
        public static void PrintErrorMessage(string errorMsg)
        {
            WriteLine(RitchTextHelper.Combine(errorMsg, RitchTextHelper.ColorToHex(data.ErrorCommandColor), true, false));
        }
        public static void PrintErrorMessage(string errorMsg, bool newline)
        {
            WriteLine(RitchTextHelper.Combine(errorMsg, RitchTextHelper.ColorToHex(data.ErrorCommandColor), true, false), newline);
        }
        public static void PrintErrorMessage(string errorMsg, string pasted)
        {
            WriteLine(RitchTextHelper.Combine(errorMsg, RitchTextHelper.ColorToHex(data.ErrorCommandColor), true, false) + ": " + pasted);
        }
        public static void PrintErrorMessage(string errorMsg, string pasted, bool newline)
        {
            WriteLine(RitchTextHelper.Combine(errorMsg, RitchTextHelper.ColorToHex(data.ErrorCommandColor), true, false) + ": " + pasted, newline);
        }
        public static void WriteLine(string line)
        {
            WriteLine(line, false);
        }
        public static void WriteLine(string line, bool Time)
        {
            //(newLine == true ? "\n" : "")
            data.ConsoleLog.text += "\n " +(Time == true ? 
                                             RitchTextHelper.Combine(
                                             "|" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "| ", RitchTextHelper.ColorToHex(data.TimeTextColor), false, true)
                                             : "" )
                                             
                                             + RitchTextHelper.DoColor(data.StartString + line, RitchTextHelper.ColorToHex(data.DefaultTextColor));
        }

        public static void ListAllCommands()
        {
            WriteLine("Available commands:");
            WriteLine(string.Format("{0,-20}   {1}", "Key", "Method"));
            WriteLine(@"---------------------*------------------------");
            foreach (var x in consoleBase.Methods.Values)
            {
                WriteLine(string.Format("{0,-20}   {1}", x.Key, x.Method));
            }
            WriteLine(@"---------------------*------------------------");
        }
        public static void ClearLong()
        {
            data.ConsoleLog.text = "";
        }
    }
}
