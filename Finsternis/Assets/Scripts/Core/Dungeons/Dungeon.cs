using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public abstract class Dungeon : MonoBehaviour
{
    [SerializeField]
    private int _seed;

    public bool customSeed = true;

    public UnityEvent onGenerationBegin;
    public UnityEvent onGenerationEnd;

    public int Seed
    {
        get { return _seed; }
        set
        {
            if (customSeed)
            {
                Random.seed = this._seed;
                _seed = value;
            }
        }
    }

    public void Awake()
    {
        if (customSeed)
        {
            Random.seed = this._seed;
        }
    }

    public virtual void Generate()
    {
        onGenerationBegin.Invoke();
    }
}

