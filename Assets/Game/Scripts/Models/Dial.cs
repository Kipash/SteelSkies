using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

[Serializable]
public class Dial
{
    [SerializeField] Text[] texts;

    public char DefaultChar = '0';

    public void SetDial(int i)
    {
        i = Mathf.Abs(i);

        if(i == 0)
        {
            for (int x = 0; x < texts.Length; x++)
            {
                SetDefault(x);
            }
        }


        var s = i.ToString();

        if (texts.Length - s.Length < 0)
        {
            s = s.Substring(0, s.Length - 1);
        }

        for (int x = texts.Length - s.Length; x < texts.Length; x++)
        {
            if (texts.Length == x)
                break;

            Set(x, s[x - texts.Length + s.Length]);
        }

        for (int x = 0; x < texts.Length - s.Length; x++)
        {
            SetDefault(x);
        }
    }

    void Set(int i, char c)
    {
        texts[i].text = c.ToString();
    }

    void SetDefault(int i)
    {
        if (texts[i].text.Length != 0)
        {
            if (texts[i].text[0] != DefaultChar)
                texts[i].text = DefaultChar.ToString();
        }
    }
}
