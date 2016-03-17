using System;

public class DungeonRandom : Random
{
    private int seed;

    public int Seed { get { return seed; } }

    public DungeonRandom(int seed) : base(seed)
    {
        this.seed = seed;
    }
}
