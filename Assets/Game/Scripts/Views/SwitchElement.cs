using UnityEngine;
using System.Collections;

namespace SteelSkies
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