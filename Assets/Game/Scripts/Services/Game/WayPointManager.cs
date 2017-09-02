using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class WayPointManager
{
    [SerializeField] Rail[] paths;

    public Dictionary<Paths, Rail> Paths { get; private set; } = new Dictionary<Paths, Rail>();

    public void Start()
    {
        Paths = paths.ToDictionary(x => x.Type, x => x);
    }
}
