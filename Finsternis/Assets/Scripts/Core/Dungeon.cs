using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class Dungeon : MonoBehaviour
{
    [SerializeField]
    private int seed;

    public int Seed
    {
        get { return seed; }
        set
        {
            seed = value;
        }
    }

    public virtual void Generate()
    {
        if (this.seed != Random.seed)
            Random.seed = this.seed;
    }
}

