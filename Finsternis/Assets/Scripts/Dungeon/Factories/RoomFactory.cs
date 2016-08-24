using System;
using UnityEngine;
using UnityQuery;

namespace Finsternis
{
    public static class RoomFactory
    {
        public static bool CarveRoom(Dungeon dungeon, Corridor corridor, Vector4 brushSizeVariation, Vector2 maxRoomSize, int maxBrushStrokes, out Room room)
        {
            Vector2 startingPosition = corridor ? corridor.Bounds.max : Vector2.zero;
            Vector2 corridorDirection = corridor ? corridor.Direction : Vector2.zero;
            Vector2 minBrushSize = brushSizeVariation.XY();
            Vector2 maxBrushSize = brushSizeVariation.ZW();

            Rect brush = new Rect(startingPosition, Vector2.zero);

            //move the room one up or to the left so it aligns properly to the corridor
            brush.x -= corridorDirection.y;
            brush.y -= corridorDirection.x;

            room = Room.CreateInstance(brush.position, Dungeon.Random);

            if (brush.x < 0 || brush.y < 0
                || !AdjustCoordinate(corridorDirection, minBrushSize, dungeon.Size, ref brush))
            {
                return false;
            }

            bool enoughSpaceForRoom = !dungeon.OverlapsCorridor(brush.position, minBrushSize);

            while (!corridorDirection.IsZero()
                    && !enoughSpaceForRoom //if the room is currently intersecting a corridor
                    && ((corridorDirection.y != 0 && brush.x >= 0 && brush.x + minBrushSize.x - 1 > room.Bounds.x)  //and it can be moved to the left (orUp) 
                    || (corridorDirection.x != 0 && brush.y >= 0 && brush.y + minBrushSize.y - 1 > room.Bounds.y))) //while still being attached to the corridor
            {
                //move the room and check again
                brush.x -= corridorDirection.y;
                brush.y -= corridorDirection.x;
                enoughSpaceForRoom = !dungeon.OverlapsCorridor(brush.position, minBrushSize);//!dungeon.SearchInArea(pos, minSize, CellType.corridor);
            }

            if (!enoughSpaceForRoom) //if a room with the minimum size possible would still intersect a corridor, stop trying to make it
                return false;

            brush.x = Mathf.Clamp(brush.x, 0, dungeon.Width);
            brush.y = Mathf.Clamp(brush.y, 0, dungeon.Height);

            bool roomCarved = false;
            //mark cells at random locations within the room, until the maximum tries is reached
            for (int tries = 0; tries < maxBrushStrokes; tries++)
            {

                if (CreateBrush(
                    minBrushSize,
                    maxBrushSize,
                    startingPosition,
                    corridorDirection,
                    dungeon,
                    room,
                    ref brush))
                {
                    if (!brush.size.IsZero())
                    {
                        if (!dungeon.OverlapsCorridor(brush.position, brush.size))
                        {
                            if (AddCells(brush, dungeon, room) > 0)
                                roomCarved = true;
                        }
                    }
                }
                MoveBrush(dungeon, startingPosition, corridorDirection, room, minBrushSize, maxBrushSize, ref brush);
            }

            if (room.Position.x < 0 || room.Position.y < 0)
                throw new ArgumentOutOfRangeException("Room was carved outside of dungeon!\n" + room);

            return roomCarved;
        }

        static void MoveBrush(
            Dungeon dungeon, 
            Vector2 startingPosition, 
            Vector2 corridorDirection, 
            Room room, 
            Vector2 minBrushSize, 
            Vector2 maxBrushSize, 
            ref Rect brush)
        {
            int modifier = (Dungeon.Random.value() <= 0.75 ? -1 : 1);

            brush.x += Dungeon.Random.Range(1f, brush.width * 0.75f) * modifier; //add some horizontal offset based off of the last calculated width
            brush.y += Dungeon.Random.Range(1f, brush.height * 0.75f) * modifier; //add some vertical offset based off of the last calculated height

            brush.x = Mathf.RoundToInt(brush.x);
            brush.y = Mathf.RoundToInt(brush.y);

            //and that they are not too further left or above what they could 
            if (corridorDirection.y == 0 && brush.x < startingPosition.x)
                brush.x = (int)startingPosition.x;
            if (corridorDirection.x == 0 && brush.y < startingPosition.y)
                brush.y = (int)startingPosition.y;

            //ensure the new coordinates are within the dungeon
            brush.x = Mathf.Clamp(brush.x, 0, dungeon.Width - minBrushSize.x);
            brush.y = Mathf.Clamp(brush.y, 0, dungeon.Height - minBrushSize.y);

            brush.x = Mathf.Clamp(brush.x, room.Bounds.x - maxBrushSize.x, room.Bounds.xMax - 1);
            brush.y = Mathf.Clamp(brush.y, room.Bounds.y - maxBrushSize.y, room.Bounds.yMax - 1);
        }

        private static bool CreateBrush(
            Vector2 minBrushSize,
            Vector2 maxBrushSize,
            Vector2 startingPosition,
            Vector2 corridorDirection,
            Dungeon dungeon,
            Room room,
            ref Rect brush)
        {
            //make sure a segment with the given dimensions won't go over the room bounds
            brush.width = Mathf.RoundToInt(Dungeon.Random.Range(minBrushSize.x, maxBrushSize.x - Mathf.Min(0, brush.x - startingPosition.x)));
            brush.height = Mathf.RoundToInt(Dungeon.Random.Range(minBrushSize.y, maxBrushSize.y - Mathf.Min(0, brush.y - startingPosition.y)));

            Rect brushPerimeter = new Rect(brush);
            Vector2 offset = corridorDirection.YX();
            brushPerimeter.min -= offset;
            brushPerimeter.max += offset;

            bool hadToExpandBrush = !room.Size.IsZero() && !room.Bounds.Overlaps(brushPerimeter);
            //make sure this new part will be connected to the room!
            while (!corridorDirection.IsZero() && hadToExpandBrush && !dungeon.OverlapsCorridor(brush.position, brush.size))
            {
                brush.width += corridorDirection.y;
                brush.height += corridorDirection.x;
                hadToExpandBrush = !room.Bounds.Overlaps(brush);
            }

            //make sure this new part won't go over a corridor!
            while (!corridorDirection.IsZero()
                    && brush.width > minBrushSize.x
                    && brush.height > minBrushSize.y
                    && dungeon.OverlapsCorridor(brush.position, brush.size))
            {
                if (hadToExpandBrush) //there's no point in reducing the brush if it had to be expanded to begin with!
                    return false;
                brush.width -= corridorDirection.y;
                brush.height -= corridorDirection.x;
            }

            brush.width = Mathf.Min(brush.width, maxBrushSize.x);
            brush.height = Mathf.Min(brush.height, maxBrushSize.y);

            return true;

        }

        private static int AddCells(Rect brush, Dungeon dungeon, Room room)
        {
            return AddCells(brush.position, brush.size, dungeon, room);
        }

        private static int AddCells(Vector2 pos, Vector2 size, Dungeon dungeon, Room room)
        {
            int cellsAdded = 0;
            for (int y = (int)pos.y; y < pos.y + size.y; y++)
            {
                for (int x = (int)pos.x; x < pos.x + size.x; x++)
                {
                    if (x < dungeon.Width && y < dungeon.Height)
                    {
                        int roomCells = room.CellCount;
                        room.AddCell(x, y);
                        if (roomCells < room.CellCount)
                            cellsAdded++;
                    }
                }
            }
            return cellsAdded;
        }

        private static bool AdjustCoordinate(Vector2 corridorDirection, Vector2 minBrushSize, Vector2 dungeonSize, ref Rect brush)
        {
            //if the corridor is to the left of the room
            if (corridorDirection.x != 0)
            {
                if (brush.xMax > dungeonSize.x) //and the brush is outside the dungeon, horizontally
                {
                    if (brush.position.x + minBrushSize.x > dungeonSize.x) //try to move it to the left a little
                        return false;
                    brush.x -= brush.xMax - dungeonSize.x;
                }

                if (brush.yMax > dungeonSize.y) //and if the brush is outside the dungeon vertically
                {
                    brush.y -= brush.yMax - dungeonSize.y; //move it up as much as needed
                }
            }
            else if (corridorDirection.y != 0) //same as abovem but for when the corridor is above the room (checks things vertivally)
            {
                if (brush.yMax > dungeonSize.y)
                {
                    if (brush.position.y + minBrushSize.y > dungeonSize.y)
                        return false;
                    brush.y -= brush.yMax - dungeonSize.y;
                }
                if (brush.xMax > dungeonSize.x)
                {
                    brush.x -= brush.xMax - dungeonSize.x;
                }
            }
            return true;
        }
    }
}