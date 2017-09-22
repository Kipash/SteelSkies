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
            a();
        }
        public static IEnumerator CallRepeatedly(Action a, float period)
        {
            WaitForSeconds wfs = new WaitForSeconds(period);
            while (true)
            {
                yield return wfs;
                a();
            }
        }
    }
}