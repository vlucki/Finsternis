using UnityEngine;
using System.Collections.Generic;
using CellType = SimpleDungeon.CellType;

public class RoomFactory
{
    public class Room
    {
        private List<Vector2> _cells;
        private Rect _bounds;

        public List<Vector2> Cells { get { return _cells; } }
        public Rect Bounds { get { return _bounds; } }

        public Room(Vector2 position)
        {
            _bounds = new Rect(position, Vector2.zero);
            _cells = new List<Vector2>();
        }

        private void AdjustSize(Vector2 pos)
        {
            if (pos.x < _bounds.x)
                _bounds.x = pos.x;
            else if (pos.x > _bounds.xMax)
                _bounds.xMax = pos.x;

            if (pos.y < _bounds.y)
                _bounds.y = pos.y;
            else if (pos.y > _bounds.yMax)
                _bounds.yMax = pos.y;
        }

        public void AddCell(float x, float y)
        {
            AddCell(new Vector2(x, y));
        }

        public void AddCell(Vector2 pos)
        {
            _cells.Add(pos);
            AdjustSize(pos);
        }

        public Vector2 GetRandomCell()
        {
            return _cells[Random.Range(0, _cells.Count)];
        }

    }

    public static bool CarveRoom(SimpleDungeon dungeon, Vector2 corridorEnd, Vector2 minSize, Vector2 maxSize, Vector2 offset, int _maximumTries, out Room room)
    {
        room = new Room(corridorEnd);

        Vector2 pos = corridorEnd;
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
            size.x = Mathf.RoundToInt(Random.Range(minSize.x, maxSize.x - pos.x + corridorEnd.x + 1));
            size.y = Mathf.RoundToInt(Random.Range(minSize.y, maxSize.y - pos.y + corridorEnd.y + 1));

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

            int modifier = (Random.value <= 0.5 ? -1 : 0);

            pos.x += Random.Range(1f, size.x * 0.75f) * modifier; //add some horizontal offset based off of the last calculated width
            pos.y += Random.Range(1f, size.y * 0.75f) * modifier; //add some vertical offset based off of the last calculated height

            pos.x = Mathf.RoundToInt(pos.x);
            pos.y = Mathf.RoundToInt(pos.y);

            //ensure the new coordinates are within the dungeon
            pos.x = Mathf.Clamp(pos.x, 0, dungeon.Width - size.x);
            pos.y = Mathf.Clamp(pos.y, 0, dungeon.Height - size.y);

            //and that they are not too further left or above what they could 
            if (offset.y == 0 && pos.x < corridorEnd.x)
                pos.x = (int)corridorEnd.x;
            if (offset.x == 0 && pos.y < corridorEnd.y)
                pos.y = (int)corridorEnd.y;
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

