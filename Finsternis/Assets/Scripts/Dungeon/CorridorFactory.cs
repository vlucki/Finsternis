using UnityEngine;
using System.Collections.Generic;
using CellType = SimpleDungeon.CellType;

public static class CorridorFactory
{

    private static bool CanFitCorridor(SimpleDungeon dungeon, Vector2 direction, Rect roomBounds, Vector2 minMaxCorridorLength)
    {
        if (direction.y != 0 //if the corridor is vertical
            && roomBounds.yMax >= dungeon.Height - minMaxCorridorLength.x) //but there isn't space for the smallest corridor allowed below the given room
        {
            return false;
        }
        else if (direction.x != 0 //if the corridor is horizontal
            && roomBounds.xMax >= dungeon.Width - minMaxCorridorLength.x) //but there isn't space for the smallest corridor allowed to the right of the given room
        {
            return false;
        }

        return true;
    }
    /// <summary>
    /// Creates a corridor.
    /// </summary>
    /// <param name="roomBounds">Position and dimension of the room where the corridor shall start.</param>
    /// <param name="direction">The direction (right or bottom) of the corridor.</param>
    /// <param name="corridor">Bounds of the corridor created.</param>
    /// <returns>True if a corridor was created without any intersections.</returns>
    public static bool CarveCorridor(SimpleDungeon dungeon, Room room, Vector2 direction, Vector2 minMaxCorridorLength, Vector2 minRoomDimensions, out Corridor corridor)
    {
        corridor = new Corridor(new Rect(), direction);
        Rect bounds = corridor.Bounds;

        if (!CanFitCorridor(dungeon, direction, room.Bounds, minMaxCorridorLength))
        {
            return false;
        }

        Vector2 corridorStart;

        corridorStart = room.GetRandomCell();

        //move the corridor starting point outside the room
        while (corridorStart.x < dungeon.Width && corridorStart.y < dungeon.Height && dungeon[corridorStart] > (int)CellType.wall && dungeon[corridorStart] < (int)CellType.corridor)
            corridorStart += direction;

        bounds.position = corridorStart;

        //if there would be no space for the smallest room after making a corridor with the minimum length, no use creating one
        if ((direction.x != 0 && bounds.x + minRoomDimensions.x >= dungeon.Width)
            || (direction.y != 0 && bounds.y + minRoomDimensions.y >= dungeon.Height))
            return false;

        //move the end of the corridor to the very edge of the room bounds (on the direction the corridor should go)
        while ((direction.x != 0 && bounds.xMax < room.Bounds.xMax) || (direction.y != 0 && bounds.yMax < room.Bounds.yMax))
            bounds.max += direction;

        bounds.max += new Vector2(dungeon.Random.Range(minMaxCorridorLength.x, minMaxCorridorLength.y) * direction.x + direction.y,
                                    dungeon.Random.Range(minMaxCorridorLength.x, minMaxCorridorLength.y) * direction.y + direction.x);

        //reduce the corridor until it is too small or until a room can fit at it's end
        while (bounds.min != bounds.max
                && ((bounds.xMax + minRoomDimensions.x) * direction.x > dungeon.Width
                || (bounds.yMax + minRoomDimensions.y) * direction.y > dungeon.Height))
            bounds.max -= direction;

        if (bounds.size.x == 0 || bounds.size.y == 0)
        {
            Debug.LogWarning("No use creating a corridor with 0 length (" + room.Bounds + ")");
            return false;
        }

        Vector2 predefinedSize = bounds.size;
        predefinedSize.x = Mathf.RoundToInt(predefinedSize.x);
        predefinedSize.y = Mathf.RoundToInt(predefinedSize.y);

        bounds.size = GetFinalSize(corridorStart, direction, predefinedSize, dungeon);
        corridor.Bounds = bounds;

        return corridor.Bounds.size.Equals(predefinedSize);
    }

    private static Vector2 GetFinalSize(Vector2 corridorStart, Vector2 direction, Vector2 predefinedSize, SimpleDungeon dungeon)
    {
        Vector2 actualSize = new Vector2(direction.y, direction.x);
        for (int row = (int)corridorStart.y; row < (int)(corridorStart.y + predefinedSize.y) && row < dungeon.Height; row++)
        {
            for (int col = (int)corridorStart.x; col < (int)(corridorStart.x + predefinedSize.x) && col < dungeon.Width; col++)
            {
                if (dungeon[col, row] != (int)CellType.wall)
                {
                    return actualSize;
                }
                actualSize.x = col - corridorStart.x + 1;
            }
            actualSize.y = row - corridorStart.y + 1;
        }

        return actualSize;
    }
}

