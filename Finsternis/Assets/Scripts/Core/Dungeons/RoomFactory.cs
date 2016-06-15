using System;
using UnityEngine;

public static class RoomFactory
{
    public static bool CarveRoom(SimpleDungeon dungeon, Corridor corridor, Vector2 minSize, Vector2 maxSize, int _maximumTries, out Room room)
    {
        return CarveRoom(dungeon, corridor.Bounds.max, minSize, maxSize, corridor.Direction, _maximumTries, out room);
    }

    public static bool CarveRoom(SimpleDungeon dungeon, Vector2 startingPosition, Vector2 minSize, Vector2 maxSize, Vector2 offset, int _maximumTries, out Room room)
    {

        Vector2 pos = startingPosition;

        //move the room one up or to the left so it aligns properly to the corridor
        pos.x -= offset.y;
        pos.y -= offset.x;

        room = new Room(pos, dungeon.Random);

        if (pos.x < 0 || pos.y < 0
            || !AdjustCoordinate(offset.x, minSize.x, dungeon.Width, ref pos.x)
            || !AdjustCoordinate(offset.y, minSize.y, dungeon.Height, ref pos.y))
        {
            return false;
        }

        bool enoughSpaceForRoom = !dungeon.OverlapsCorridor(pos, minSize);

        while (!enoughSpaceForRoom //if the room is currently intersecting a corridor
                && ((offset.y != 0 && pos.x >= 0 && pos.x + minSize.x > room.Bounds.x)  //and it can be moved to the left (orUp) 
                || (offset.x != 0 && pos.y >= 0 && pos.y + minSize.y > room.Bounds.y))) //while still being attached to the corridor
        {

            //move the room and check again
            pos.x -= offset.y;
            pos.y -= offset.x;
            enoughSpaceForRoom = !dungeon.OverlapsCorridor(pos, minSize);//!dungeon.SearchInArea(pos, minSize, CellType.corridor);
        }

        if (!enoughSpaceForRoom) //if a room with the minimum size possible would still intersect a corridor, stop trying to make it
            return false;

        pos.x = Mathf.Clamp(pos.x, 0, dungeon.Width);
        pos.y = Mathf.Clamp(pos.y, 0, dungeon.Height);

        bool roomCarved = false;
        //mark cells at random locations within the room, until the maximum tries is reached
        for (int tries = 0; tries < _maximumTries; tries++)
        {
            //make sure a segment with the given dimensions won't go over the room bounds
            Vector2 size = new Vector2();
            size.x = Mathf.RoundToInt(dungeon.Random.Range(minSize.x, maxSize.x - pos.x + startingPosition.x));
            size.y = Mathf.RoundToInt(dungeon.Random.Range(minSize.y, maxSize.y - pos.y + startingPosition.y));

            //make sure this new part will be connected to the room!
            while (room.Size != Vector2.zero && !room.Bounds.Overlaps(new Rect(pos-Vector2.one, size+2*Vector2.one)))
            {
                size.x += offset.y;
                size.y += offset.x;
            }

            //make sure this new part won't go over a corridor!
            while (size.x > minSize.x && size.y > minSize.y && dungeon.OverlapsCorridor(pos, size))
            {
                size.x -= offset.y;
                size.y -= offset.x;
            }

            size.x = Mathf.Min(size.x, maxSize.x);
            size.y = Mathf.Min(size.y, maxSize.y);

            if (!dungeon.OverlapsCorridor(pos, size))
            {
                roomCarved = true;
                AddCells(pos, size, dungeon, room);
            }
            int modifier = (dungeon.Random.value() <= 0.5 ? -1 : 1);

            pos.x += dungeon.Random.Range(1f, size.x * 0.75f) * modifier; //add some horizontal offset based off of the last calculated width
            pos.y += dungeon.Random.Range(1f, size.y * 0.75f) * modifier; //add some vertical offset based off of the last calculated height

            pos.x = Mathf.RoundToInt(pos.x);
            pos.y = Mathf.RoundToInt(pos.y);

            //and that they are not too further left or above what they could 
            if (offset.y == 0 && pos.x < startingPosition.x)
                pos.x = (int)startingPosition.x;
            if (offset.x == 0 && pos.y < startingPosition.y)
                pos.y = (int)startingPosition.y;

            //ensure the new coordinates are within the dungeon
            pos.x = Mathf.Clamp(pos.x, 0, dungeon.Width - minSize.x);
            pos.y = Mathf.Clamp(pos.y, 0, dungeon.Height - minSize.y);
        }

        if (room.Pos.x < 0 || room.Pos.y < 0)
            throw new ArgumentOutOfRangeException("Room was carved outside of dungeon!\n" + room);

        return roomCarved;
    }

    private static void AddCells(Vector2 pos, Vector2 size, SimpleDungeon dungeon, Room room)
    {
        for (int y = (int)pos.y; y < pos.y + size.y; y++)
        {
            for (int x = (int)pos.x; x < pos.x + size.x; x++)
            {
                if (x < dungeon.Width && y < dungeon.Height)
                    room.AddCell(x, y);
            }
        }
    }

    /// <summary>
    /// Tries to adjust a given coordinate so it respects a given limit.
    /// </summary>
    /// <param name="axisOffset">Whether this coordinate can be adjusted or not.</param>
    /// <param name="adjustment">By how much the coordinate may be shifted.</param>
    /// <param name="limit">An upper (exclusive) limit for 'coordinate'.</param>
    /// <param name="coordinate">The X or Y coordinate that is being adjusted.</param>
    /// <returns>True if the adjustment was possible.</returns>
    private static bool AdjustCoordinate(float axisOffset, float adjustment, float limit, ref float coordinate)
    {
        if (coordinate + adjustment > limit)
        {
            if (axisOffset == 0)
                coordinate = limit - adjustment;
            else
                return false;
        }

        return true;
    }

}

