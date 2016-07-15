using UnityEngine;
using UnityQuery;

namespace Finsternis
{
    public static class CorridorFactory
    {
        private static bool CanFitCorridor(Dungeon dungeon, Vector2 direction, Rect roomBounds, Vector2 minMaxCorridorLength)
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
        public static bool CarveCorridor(Dungeon dungeon, Room room, Vector2 direction, Vector2 minMaxCorridorLength, Vector2 minRoomDimensions, out Corridor corridor)
        {
            corridor = Corridor.CreateInstance(new Rect(), direction);

            if (!CanFitCorridor(dungeon, direction, room.Bounds, minMaxCorridorLength))
            {
                return false;
            }

            Vector2 corridorStart = Vector2.zero;
            int tries = 10;
            bool maySpawnCorridor = false;
            Vector2 sideOffset = direction.YX();
            for (int count = 0; !maySpawnCorridor && count < tries; count++)
            {
                corridorStart = room.GetRandomCell();

                //move the corridor starting point outside the room
                while (corridorStart.x < dungeon.Width && corridorStart.y < dungeon.Height && room.ContainsCell(corridorStart))
                    corridorStart += direction;

                Vector2 offsetA = corridorStart + sideOffset;
                Vector2 offsetB = corridorStart - sideOffset;

                if (dungeon.IsWithinDungeon(offsetA))
                {
                    if (dungeon[offsetA] != null)
                    {
                        maySpawnCorridor = false;
                        continue;
                    }
                    else
                    {
                        maySpawnCorridor = true;
                    }
                }
                if (dungeon.IsWithinDungeon(offsetB))
                {
                    if (dungeon[offsetB] != null)
                    {
                        maySpawnCorridor = false;
                        continue;
                    }
                    else
                    {
                        maySpawnCorridor = true;
                    }
                }
            }

            corridor.Position = corridorStart;

            //if there would be no space for the smallest room after making a corridor with the minimum length, no use creating one
            if ((direction.x != 0 && corridor.X + minRoomDimensions.x >= dungeon.Width)
                || (direction.y != 0 && corridor.Y + minRoomDimensions.y >= dungeon.Height))
                return false;

            //move the end of the corridor to the very edge of the room bounds (on the direction the corridor should go)
            while ((direction.x != 0 && corridor.LastCell.x < room.Bounds.xMax) || (direction.y != 0 && corridor.LastCell.y < room.Bounds.yMax))
                corridor.Length++;

            corridor.Length += Mathf.CeilToInt(Dungeon.Random.Range(minMaxCorridorLength.x, minMaxCorridorLength.y));

            if (corridor.Length == 0)
            {
                Debug.LogWarning("No use creating a corridor with 0 length (" + room.Bounds + ")");
                return false;
            }

            int predefinedSize = corridor.Length;
            int actualSize = 0;
            for (int size = 0; size < predefinedSize; size++)
            {
                Vector2 cell = corridor[size];
                if (!dungeon.IsWithinDungeon(cell) || dungeon[cell] != null)
                {
                    break;
                }
                actualSize = size + 1;
            }
            corridor.Length = actualSize;

            bool success = corridor.Length > 0 && corridor.Length == predefinedSize;

            return success;
        }
    }
}