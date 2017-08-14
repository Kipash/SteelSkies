using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCoroutines : MonoBehaviour
{
    List<DelayedCall> currCalls;
    public void Invoke(DelayedCall call)
    {
        StartCoroutine(Coroutine(call));
        call.Canceled = false;
    }
    public void CancelInvoke(DelayedCall call)
    {
        call.Canceled = true;    
    }

    IEnumerator Coroutine(DelayedCall call)
    {
        yield return new WaitForSeconds(call.Delay);
        if (!call.Canceled)
            call.CallBack();
    }
}

public class DelayedCall
{
    public Action CallBack;
    public float Delay;
    public bool Canceled;
}