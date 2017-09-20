using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Console
{
    public static class ConsoleFuncs
    {
        public static string GameObjectOps(string name/*, string varName = null*/)
        {
            var obj = GameObject.Find(name);
            var t = obj.GetType();

            var bflags =
                    BindingFlags.Instance |
                    BindingFlags.Static |
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.GetField |
                    BindingFlags.GetProperty |
                    BindingFlags.IgnoreCase;

            //if (varName != null)
            //{
            //    var f = t.GetField(varName, bflags);
            //    if (f != null)
            //    {
            //        var val = f.GetValue(obj);
            //        return val != null ? val.ToString() : null;
            //    }

            //    var p = t.GetProperty(varName, bflags);
            //    if (p != null)
            //    {
            //        var val = f.GetValue(obj);
            //        return val != null ? val.ToString() : null;
            //    }

            //    throw new Exception(string.Format("{0} doesn't contain '{1}'.", t.Name, varName));
            //}
            //else
            //{
                Func<MemberInfo, KeyValuePair<string, object>> selector = x =>
                {
                    object val;
                    try
                    {
                        val = x.GetType() == typeof(FieldInfo)
                            ? (x as FieldInfo).GetValue(obj)
                            : (x as PropertyInfo).GetValue(obj, null);
                    }
                    catch (Exception e)
                    {
                        val = e.GetBaseException().Message;
                    }

                    return new KeyValuePair<string, object>(x.Name, val);
                };

                var fields = t.GetFields(bflags).Select(selector).ToArray();
                var props = t.GetProperties(bflags).Select(selector).ToArray();

                return
                    string.Join("\n", props.Select(x => x.Key + " = " + x.Value).ToArray()) + "\n" +
                    string.Join("\n", fields.Select(x => x.Key + " = " + x.Value).ToArray());
            //}
        }

        public static object Concat(string s0, string s1)
        {
            var res = s0 + s1;
            //System.IO.File.AppendAllText(@"c:\desk\out.txt", res);
            return res;
        }

        public static void Print(string s)
        {
            Console.WriteLine(s);
        }
    }


    //public class ObjB
    //{
    //    public void Log(string[] lines)
    //    {
    //        //System.IO.File.AppendAllText(@"c:\desk\log.txt", lines);
    //    }
    //}
    //public class ObjA
    //{
    //    public object Concat(string[] vals)
    //    {
    //        ///var res = s0 + s1;
    //        //System.IO.File.AppendAllText(@"c:\desk\out.txt", res);
    //        return string.Concat(vals);
    //    }
    //    public void WriteLine(string s)
    //    {
    //        System.Console.WriteLine(s);
    //    }
    //}

    public class ObjB
    {
        public void Log(string[] lines)
        {
            //System.IO.File.AppendAllLines(@"c:\desk\log.txt", lines);
        }
    }
    public class ObjA
    {
        public object Concat(string[] vals)
        {
            ///var res = s0 + s1;
            //System.IO.File.AppendAllText(@"c:\desk\out.txt", res);
            return string.Concat(vals);
        }
        public object WriteLine(string[] s)
        {
            return s;
            //System.Console.WriteLine(s);
        }
        public object Number(string[] vals)
        {
            string c = null;
            float curr = 0;
            foreach(var x in vals)
            {
                if (float.TryParse(x, out curr))
                    c += " " + curr;
                else
                    c += " Error";
            }
            return c;
        }
    }

}
