using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RitchTextHelper
{
    static string BoldOpen = "<b>";
    static string BoldClose = "</b>";
    static string ItalicOpen = "<i>";
    static string ItalicClose = "</i>";
    static string ColorOpen = "<color={0}>";
    static string ColorClose = "</color>";
    static string SizeOpen = "<size={0}>";
    static string SizeClose = "</size>";

    public static string DoBold(string text)
    {
        return BoldOpen + text + BoldClose;
    }
    public static string DoItalic(string text)
    {
        return ItalicOpen + text + ItalicClose;
    }
    public static string DoColor(string text, string hexColor)
    {
        return string.Format(ColorOpen + text + ColorClose, hexColor);
    }
    public static string Combine(string text, string hexColor, int size, bool bold, bool italic)
    {
        if (bold && italic)
        {
            return string.Format(ItalicOpen + BoldOpen + string.Format(SizeOpen, "{0}") + string.Format(ColorOpen, "{1}") + text + ColorClose + SizeClose + BoldClose + ItalicClose, size, hexColor);
        }
        else if (italic)
        {
            return string.Format(ItalicOpen + string.Format(SizeOpen, "{0}") + string.Format(ColorOpen, "{1}") + text + ColorClose + SizeClose + ItalicClose, size, hexColor);
        }
        else if (bold)
        {
            return string.Format(BoldOpen + string.Format(SizeOpen, "{0}") + string.Format(ColorOpen, "{1}") + text + ColorClose + SizeClose + BoldClose, size, hexColor);
        }
        else
        {
            return string.Format(string.Format(SizeOpen, "{0}") + string.Format(ColorOpen, "{1}") + text + ColorClose + SizeClose, 60, hexColor);
        }
    }
    public static string Combine(string text, string hexColor, bool bold, bool italic)
    {
        if (bold && italic)
        {
            return string.Format(ItalicOpen + BoldOpen + ColorOpen + text + ColorClose + BoldClose + ItalicClose, hexColor);
        }
        else if (italic)
        {
            return string.Format(ItalicOpen + ColorOpen + text + ColorClose + ItalicClose, hexColor);
        }
        else if (bold)
        {
            return string.Format(BoldOpen + ColorOpen + text + ColorClose + BoldClose, hexColor);
        }
        else
        {
            return string.Format(ColorOpen + text + ColorClose, hexColor);
        }
    }
    public static string Combine(string text, int size, bool bold, bool italic)
    {
        if (bold && italic)
        {
            return string.Format(ItalicOpen + BoldOpen + SizeOpen + text + SizeClose + BoldClose + ItalicClose, size);
        }
        else if (italic)
        {
            return string.Format(ItalicOpen + SizeOpen + text + SizeClose + ItalicClose, size);
        }
        else if (bold)
        {
            return string.Format(BoldOpen + SizeOpen + text + SizeClose + BoldClose, size);
        }
        else
        {
            return string.Format(SizeOpen + text + SizeClose, size);
        }
    }
    public static string Combine(string text, bool bold, bool italic)
    {
        if (bold && italic)
        {
            return ItalicOpen + BoldOpen + text + BoldClose + ItalicClose;
        }
        else if (italic)
        {
            return ItalicOpen + text + ItalicClose;
        }
        else if (bold)
        {
            return BoldOpen + text + BoldClose;
        }
        else
        {
            return text;
        }
    }

    static string ConvertRGBAtoHex(string s)
    {
        var s0 = s.Skip(1);


        int.Parse(new string(s.Skip(1).Take(2).ToArray()), System.Globalization.NumberStyles.HexNumber);
        return "s";
    }

    public static string ColorToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return "#" + hex;
    }

    public static Color HexToColor(string hex)
    {
        hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
        hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
        byte a = 255;//assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        //Only use alpha if the string has enough characters
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }
}