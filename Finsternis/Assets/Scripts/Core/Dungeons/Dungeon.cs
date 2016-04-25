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

    public Vector2 Entrance { get { return entrance; } }
    public Vector2 Exit { get { return exit; } }

    public virtual void Awake()
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

