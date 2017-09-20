using UnityEngine;
using System.Collections;

namespace Aponi
{
    public class SwitchElement : MonoBehaviour
    {
        public SwitchManager Manager;
        public void Active()
        {
            Manager.Interact(this);
        }
    }
}