using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class Dungeon : MonoBehaviour
{
    [SerializeField]
    private int seed;

    public bool customSeed = true;

    public int Seed
    {
        get { return seed; }
        set
        {
            if (customSeed)
            {
                Random.seed = this.seed;
                seed = value;
            }
        }
    }

    public void Awake()
    {
        if (customSeed)
        {
            Random.seed = this.seed;
        }
    }

    public abstract void Generate();
}

