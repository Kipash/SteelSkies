using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

[Serializable]
public class Dial
{
    [SerializeField] Text[] texts;
    [SerializeField] GameObject[] images;

    [SerializeField] string positive;
    [SerializeField] string negative;

    public char DefaultChar = '0';

    public void SetNumericDial(int i)
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

    public void SetImageDial(int i)
    {
        i = Mathf.Clamp(Mathf.Abs(i), 0, images.Length);
        
        for (int x = 0; x <  i; x++)
        {
            Set(x, true);
        }

        for (int x = i; x < images.Length; x++)
        {
            Set(x, false);
        }
    }

    void Set(int i, char c)
    {
        texts[i].text = c.ToString();
    }
    void Set(int i, bool b)
    {
        var p = images[i].transform.Find(positive);
        var n = images[i].transform.Find(negative);

        p.gameObject.SetActive(b);
        n.gameObject.SetActive(!b);
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
