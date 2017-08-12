using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CurrentBinds
{
    public static Dictionary<RegisteredKeys, KeyCode[]> BindedKeys = new Dictionary<RegisteredKeys, KeyCode[]>();

    public static void SetDefaults()
    {
        BindedKeys.Add(RegisteredKeys.Fire, new KeyCode[] { KeyCode.Space, KeyCode.Mouse0 });

        BindedKeys.Add(RegisteredKeys.MoveLeft, new KeyCode[] { KeyCode.A, KeyCode.LeftArrow });
        BindedKeys.Add(RegisteredKeys.MoveRight, new KeyCode[] { KeyCode.D, KeyCode.RightArrow });
        BindedKeys.Add(RegisteredKeys.MoveDown, new KeyCode[] { KeyCode.S, KeyCode.DownArrow });
        BindedKeys.Add(RegisteredKeys.MoveUp, new KeyCode[] { KeyCode.W, KeyCode.UpArrow });

        BindedKeys.Add(RegisteredKeys.Pause, new KeyCode[] { KeyCode.Escape });
    }
}
