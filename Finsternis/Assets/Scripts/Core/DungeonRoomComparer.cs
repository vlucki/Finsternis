using System;
using System.Collections.Generic;

internal class DungeonRoomComparer : IEqualityComparer<DungeonRoom>
{
    public bool Equals(DungeonRoom a, DungeonRoom b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public int GetHashCode(DungeonRoom obj)
    {
        return obj.X * 2 + obj.Y * 997;
    }
}