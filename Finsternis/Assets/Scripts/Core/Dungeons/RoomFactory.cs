using UnityEngine;
using CellType = SimpleDungeon.CellType;

public static class RoomFactory
{
    public static bool CarveRoom(SimpleDungeon dungeon, Corridor corridor, Vector2 minSize, Vector2 maxSize, int _maximumTries, out Room room)
    {
        return CarveRoom(dungeon, corridor.Bounds.max, minSize, maxSize, corridor.Direction, _maximumTries, out room);
    }

    public static bool CarveRoom(SimpleDungeon dungeon, Vector2 startingPosition, Vector2 minSize, Vector2 maxSize, Vector2 offset, int _maximumTries, out Room room)
    {
        room = new Room(startingPosition);

        Vector2 pos = startingPosition;
        pos.x -= offset.y;
        pos.y -= offset.x;

        if (!VerifyAxis(offset.x, minSize.x, dungeon.Width, ref pos.x) || !VerifyAxis(offset.y, minSize.y, dungeon.Height, ref pos.y))
        {
            return false;
        }

        bool enoughSpaceForRoom = !dungeon.SearchInArea(pos, minSize, CellType.corridor);

        while (!enoughSpaceForRoom //if the room is currently intersecting a corridor
                && ((offset.y != 0 && pos.x + minSize.x >= room.Bounds.x) //and it can be moved to the left (orUp) 
                || (offset.x != 0 && pos.y + minSize.y >= room.Bounds.y)))//while still being attached to the corridor
        {

            //move the room and check again
            pos.x -= offset.y;
            pos.y -= offset.x;
            enoughSpaceForRoom = !dungeon.SearchInArea(pos, minSize, CellType.corridor);
        }

        if (!enoughSpaceForRoom) //if a room with the minimum size possible would still intersect a corridor, stop trying to make it
        {
            return false;
        }

        bool roomCarved = false;

        //mark cells at random locations within the room, until the maximum tries is reached
        for (int tries = 0; tries < _maximumTries; tries++)
        {
            //make sure a segment with the given dimensions won't go over the room bounds
            Vector2 size = new Vector2();
            size.x = Mathf.RoundToInt(Random.Range(minSize.x, maxSize.x - pos.x + startingPosition.x + 1));
            size.y = Mathf.RoundToInt(Random.Range(minSize.y, maxSize.y - pos.y + startingPosition.y + 1));

            if (dungeon.SearchInArea(pos, size, CellType.corridor))
            {
                size = minSize;
            }

            if (!dungeon.SearchInArea(pos, size, CellType.corridor))
            {

                roomCarved = true;

                for(int i = (int)pos.y; i < pos.y + size.y; i++)
                {
                    for (int j = (int)pos.x; j < pos.x + size.x; j++)
                    {
                        if(j < dungeon.Height && i < dungeon.Width)
                            room.AddCell(j, i);
                    }
                }
            }

            int modifier = (Random.value <= 0.5 ? -1 : 1);

            pos.x += Random.Range(1f, size.x * 0.75f) * modifier; //add some horizontal offset based off of the last calculated width
            pos.y += Random.Range(1f, size.y * 0.75f) * modifier; //add some vertical offset based off of the last calculated height

            pos.x = Mathf.RoundToInt(pos.x);
            pos.y = Mathf.RoundToInt(pos.y);

            //and that they are not too further left or above what they could 
            if (offset.y == 0 && pos.x < startingPosition.x)
                pos.x = (int)startingPosition.x;
            if (offset.x == 0 && pos.y < startingPosition.y)
                pos.y = (int)startingPosition.y;

            //ensure the new coordinates are within the dungeon
            pos.x = Mathf.Clamp(pos.x, 0, dungeon.Width - size.x);
            pos.y = Mathf.Clamp(pos.y, 0, dungeon.Height - size.y);
        }
        

        return roomCarved;
    }

    private static bool VerifyAxis(float offset, float minSize, float limit, ref float axis)
    {
        if (axis + minSize > limit)
        {
            if (offset == 0) //and the corridor it's connected to is above it
                axis = limit - minSize; //move the room left
            else
                return false;
        }

        return true;
    }

}

