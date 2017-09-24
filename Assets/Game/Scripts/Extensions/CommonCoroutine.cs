using UnityEngine;
using System.Collections;
using System;

namespace Aponi
{
    public class CommonCoroutine
    {
        public static IEnumerator CallDelay(Action a, float t)
        {
            yield return new WaitForSeconds(t);
            //Debug.LogFormat("Calling {0} with delay of {1} sec", a.Method.Name, t);
            a();
        }
        public static IEnumerator CallRepeatedly(Action a, float period)
        {
            WaitForSeconds wfs = new WaitForSeconds(period);
            //Debug.LogFormat("Calling {0} with every {1} sec", a.Method.Name, period);
            while (true)
            {
                yield return wfs;
                a();
            }
        }
    }
}