using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public abstract class Dungeon : MonoBehaviour
{
    public UnityEvent onGenerationBegin;
    public UnityEvent onGenerationEnd;

    [SerializeField]
    private int _seed;

    public bool customSeed = true;

    [SerializeField]
    protected Vector2 entrance;

    [SerializeField]
    protected Vector2 exit;

    protected MTRandom random;

    public MTRandom Random { get { return random; } }

    public int Seed
    {
        get { return _seed; }
        set
        {
            if (customSeed)
            {
                //Random.seed = this._seed;
                random = new MTRandom(value);
                _seed = value;
            }
        }
    }

    public Vector2 Entrance { get { return entrance; } }
    public Vector2 Exit { get { return exit; } }

    public virtual void Awake()
    {
        if (customSeed)
        {
            random = new MTRandom(this._seed);
            //Random.seed = this._seed;
        }
    }

    public virtual void Generate()
    {
        onGenerationBegin.Invoke();
    }
}

