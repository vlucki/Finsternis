using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System;

public abstract class Dungeon : MonoBehaviour
{
    [SerializeField]
    private int? seed;

    protected DungeonRandom random;

    public int? Seed
    {
        get { return seed; }
        set
        {
            seed = value;
            random = null;
        }
    }

    public abstract void Generate();
}

