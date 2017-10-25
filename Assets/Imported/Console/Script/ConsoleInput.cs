using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using d = Console;


namespace Console
{
    [Serializable]
    public class ConsoleData
    {
        public string StartString;
        public bool ShowTime;
        public Color TypedCommandColor;
        public Color ErrorCommandColor;
        public Color DefaultTextColor;
        public Color TimeTextColor;
        public Char CommandChar;
        public InputField ConsoleInputField;
        public Text ConsoleLog;
    }
}
namespace SteelSkies
{
    [Serializable]
    public class ConsoleInput
    {
        public d.ConsoleData Data;


        //Console.Console console;
        public Animator ConsoleAnimotor;
        public void Initialize()
        {
            d.Console.Initialize(Data);
            //console.consoleBase.AddStatic();
            //InGameDebug.Console.Add("merge", new ObjA(), "Concat", true);
            //InGameDebug.Console.Add("write", new ObjA(), "WriteLine", true);
            //InGameDebug.Console.Add("Int", new ObjA(), "Number", true);
            //InGameDebug.Console.AddStatic("obj", typeof(ConsoleFuncs), "GameObjectOps");
            d.Console.AddStatic("help", typeof(d.Console), "ListAllCommands");
            d.Console.AddStatic("h", typeof(d.Console), "ListAllCommands");

            d.Console.AddStatic("cls", typeof(d.Console), "ClearLong");
            d.Console.AddStatic("clear", typeof(d.Console), "ClearLong");
        }
        bool active = false;
        List<string> cmdHistory = new List<string>();

        int index;
        int currentCmdIndex
        {
            get
            {
                return index;
            }
            set
            {
                if (cmdHistory.Count > 0)
                    index = Mathf.Clamp(value, 0, cmdHistory.Count - 1);
                else
                    index = 0;
            }
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                cmdHistory.Add(Data.ConsoleInputField.text);
                d.Console.Execute();
                currentCmdIndex = 0;
            }

            if (Input.GetKeyDown(KeyCode.BackQuote) && (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)))
            {
                AppInput.Disable = !active;
                if (active)
                {
                    Data.ConsoleInputField.DeactivateInputField();
                    ConsoleAnimotor.SetTrigger("Pull");
                }
                else
                {
                    ConsoleAnimotor.SetTrigger("Push");
                    Data.ConsoleInputField.ActivateInputField();
                    Data.ConsoleInputField.text = "";
                }
                active = !active;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                SetInputField();
                currentCmdIndex++;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentCmdIndex--;
                SetInputField();
            }
        }
        void SetInputField()
        {
            if (cmdHistory.Count > 0)
            {
                var rev = cmdHistory.ToArray().Reverse();
                var cmd = rev.ElementAt(currentCmdIndex);
                Data.ConsoleInputField.text = cmd;
            }
        }
    }
}