﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityQuery;

namespace Finsternis
{
    public class DungeonFactory : MonoBehaviour
    {

        [Serializable]
        public class DungeonGenerationEndEvent : UnityEvent<Dungeon>
        {
            public static implicit operator bool(DungeonGenerationEndEvent evt)
            {
                return evt != null;
            }
        }

        public UnityEvent onGenerationBegin;
        public DungeonGenerationEndEvent onGenerationEnd;

        #region Generation Parameters

        public string dungeonName = "Simple Dungeon";
        [Space]

        [Header("Dungeon dimensions")]
        [SerializeField]
        [Range(10, 1000)]
        private int _dungeonWidth = 20;

        [SerializeField]
        [Range(10, 1000)]
        private int _dungeonHeight = 20;

        [SerializeField]
        [Range(2, 1000)] //at least one starting point and one exit
        private int _totalRooms = 5;

        [Tooltip("How many times should it try to carve a room before stoping")]
        [SerializeField]
        [Range(1, 100)]
        private int _maximumTries = 2;

        [Header("Maximum room size")]
        [SerializeField]
        [Range(2, 1000)]
        private int _maximumRoomWidth;

        [SerializeField]
        [Range(2, 1000)]
        private int _maximumRoomHeight;

        [Header("Minimum brush size")]
        [SerializeField]
        [Range(2, 1000)]
        private int _minimumBrushWidth = 3;

        [SerializeField]
        [Range(2, 1000)]
        private int _minimumBrushHeight = 3;

        [Header("Maximum brush size")]
        [SerializeField]
        [Range(2, 100)]
        private int _maximumBrushWidth = 7;

        [SerializeField]
        [Range(2, 100)]
        private int _maximumBrushHeight = 7;

        [Header("Corridors")]
        [SerializeField]
        [Range(1, 100)]
        private int _minimumCorridorLength = 1;

        [SerializeField]
        [Range(1, 100)]
        private int _maximumCorridorLength = 5;

        [SerializeField]
        private bool _allowDeadEnds = false;

        [Space]
        [Header("Features")]
        [SerializeField]
        private DungeonFeature[] _traps;

        [SerializeField]
        private DoorFeature[] _doors;
        #endregion

        private Room _lastRoom;
        public const string SEED_KEY = "LastSeed";

        /// <summary>
        /// Initializes every field, ensuring they have consistent values.
        /// </summary>
        /// <param name="dungeon">Reference to the dungeon.</param>
        public void Init(Dungeon dungeon)
        {
            dungeon.Init(_dungeonWidth, _dungeonHeight);

            _maximumBrushHeight = Mathf.Clamp(_maximumBrushHeight, 0, dungeon.Height);
            _maximumBrushWidth = Mathf.Clamp(_maximumBrushWidth, 0, dungeon.Width);
            _minimumBrushHeight = Mathf.Clamp(_minimumBrushHeight, 0, _maximumBrushHeight);
            _minimumBrushWidth = Mathf.Clamp(_minimumBrushWidth, 0, _maximumBrushWidth);
            _maximumCorridorLength = Mathf.Clamp(_maximumCorridorLength, 0, Mathf.Min(dungeon.Height, dungeon.Width));
            _minimumCorridorLength = Mathf.Clamp(_minimumCorridorLength, 0, _maximumCorridorLength);
        }

        /// <summary>
        /// Core method for dungeon generation.
        /// </summary>
        /// <param name="seed">The seed to be used for the pseudo-random number generator.</param>
        public void Generate(int? seed = null)
        {
            GameObject dungeonGO = new GameObject(dungeonName);

            dungeonGO.tag = "Dungeon";
            Dungeon dungeon = dungeonGO.AddComponent<Dungeon>();
            if (seed != null)
                dungeon.Seed = (int)seed;

            Init(dungeon);

            onGenerationBegin.Invoke();

            Queue<Corridor> hangingCorridors = null;
            Queue<Room> hangingRooms = new Queue<Room>();

            Vector4 brushVariation = new Vector4(_minimumBrushWidth, _minimumBrushHeight, _maximumBrushWidth, _maximumBrushHeight);
            Vector2 maxRoomSize = new Vector2(_minimumBrushWidth, _minimumBrushHeight);

            Room room;
            if (RoomFactory.CarveRoom(dungeon, null, brushVariation, maxRoomSize, _maximumTries, out room))
            {
                dungeon.MarkCells(room);
                hangingRooms.Enqueue(room);
                dungeon.Rooms.Add(room);
                dungeon.Entrance = room.GetRandomCell();
            }
            else
                throw new InvalidOperationException("Could not create a single room within the dungeon. Maybe it is too small and the room too big?");

            int roomCount = 1;
            while (roomCount < _totalRooms  //keep going until the desired number of rooms was generated
                    && (hangingRooms.Count > 0 || hangingCorridors.Count > 0)) //or until no more rooms or corridors can be generated
            {
                hangingCorridors = GenerateCorridors(dungeon, hangingRooms);

                roomCount = GenerateRooms(dungeon, hangingRooms, hangingCorridors, brushVariation, maxRoomSize, roomCount);
            }

            if (!_allowDeadEnds) ConnectLeftoverCorridors(dungeon, hangingCorridors);

            CleanUp(dungeon);

            AddFeatures(dungeon);

            dungeon.Exit = _lastRoom.GetRandomCell();

            PlayerPrefs.SetInt(SEED_KEY, dungeon.Seed);

            if (onGenerationEnd)
                onGenerationEnd.Invoke(dungeon);
        }

        /// <summary>
        /// Add features (doors, traps, chests) to the dungeon.
        /// </summary>
        /// <param name="dungeon">Reference to the dungeon.</param>
        private void AddFeatures(Dungeon dungeon)
        {
            foreach (Corridor corridor in dungeon.Corridors)
            {
                AddTrap(corridor);
                AddDoors(corridor);
            }
        }

        /// <summary>
        /// Randomly adds a trap to a corridor.
        /// </summary>
        /// <param name="corridor">The corridor to be trapped.</param>
        private void AddTrap(Corridor corridor)
        {
            if (_traps == null || _traps.Length == 0)
            {
                Debug.LogWarning("No traps found.");
                return;
            }
            if (corridor.Length > 2 && Dungeon.Random.value() <= 0.3f)
            {
                int pos = Dungeon.Random.Range(1, corridor.Length - 2);
                DungeonFeature feature = (_traps[Dungeon.Random.Range(0, _traps.Length, false)]);
                corridor.AddFeature(feature, corridor[pos]);
            }
        }

        /// <summary>
        /// Add doors at the junction between rooms and corridors.
        /// </summary>
        /// <param name="corridor">The corridor that will have doors added to it.</param>
        /// <param name="index">Is this door being added the the first (0) or last (1) cell of the corridor?</param>
        private void AddDoors(Corridor corridor, int index = 0)
        {
            if (_doors == null || _doors.Length == 0)
            {
                Debug.LogWarning("No doors found.");
                return;
            }

            DoorFeature feature = (_doors[Dungeon.Random.Range(0, _doors.Length, false)]);
            corridor.AddFeature(feature, corridor[index]);

            foreach (DungeonSection section in corridor.Connections)
            {
                if (section is Room && ((Room)section).Locked)
                {
                    feature.Locked = true;
                    break;
                }
            }


            if (corridor.Length > 1 && index == 0)
            {
                feature.Alignment = DungeonFeature.CellAlignment.EDGE;
                AddDoors(corridor, corridor.Length - 1);
            }

        }

        /// <summary>
        /// Removes corridors that don't really look like corridors (eg. have walls only on one side) 
        /// and merge rooms that overlap or don't have walls between one or more cells
        /// </summary>
        private void CleanUp(Dungeon dungeon)
        {
            FixCorridors(dungeon);
            MergeRooms(dungeon);
        }

        /// <summary>
        /// Ensures every corridor within the dungeon "looks right"
        /// </summary>
        /// <param name="dungeon">Reference to the dungeon.</param>
        private void FixCorridors(Dungeon dungeon)
        {
            for (int i = dungeon.Corridors.Count - 1; i >= 0; i--)
            {
                Corridor corridor = dungeon.Corridors[i];
                if (SplitCorridor(dungeon, corridor))
                {
                    i = dungeon.Corridors.Count;
                    continue;
                }
                if (!ExtendCorridor(dungeon, corridor))
                    TrimCorridor(dungeon, corridor);
            }
        }

        /// <summary>
        /// Splits corridors that have cells without walls on at least two sides.
        /// </summary>
        /// <param name="dungeon">Reference to the dungeon.</param>
        /// <param name="corridor">Corridor to be split.</param>
        /// <returns>True is the corridor was split.</returns>
        private bool SplitCorridor(Dungeon dungeon, Corridor corridor)
        {
            Corridor[] halves = null;
            Vector2 offset = corridor.Direction.YX();
            bool hasToSplit = false;
            int index = corridor.Length - 1;

            while (index >= 0)
            {
                Vector2 cell = corridor[index];
                Vector2 offsetCellA = cell + offset;
                Vector2 offsetCellB = cell - offset;
                if (dungeon.IsWithinDungeon(offsetCellA))
                {
                    if (dungeon[offsetCellA] && !(dungeon[offsetCellA] is Corridor))
                    {
                        dungeon[cell] = dungeon[offsetCellA];
                        dungeon[offsetCellA].AddCell(cell);
                        if (hasToSplit)
                        {
                            halves = corridor.RemoveAt(index);
                            break;
                        }
                        else
                        {
                            corridor.Length--;
                        }
                    }
                }
                if (dungeon.IsWithinDungeon(offsetCellB))
                {
                    if (dungeon[offsetCellB] && !(dungeon[offsetCellB] is Corridor))
                    {
                        dungeon[cell] = dungeon[offsetCellB];
                        dungeon[offsetCellB].AddCell(cell);
                        if (hasToSplit)
                        {
                            halves = corridor.RemoveAt(index);
                            break;
                        }
                        else
                        {
                            corridor.Length--;
                        }
                    }
                }
                index--;
                if (index >= corridor.Length)
                    index = corridor.Length - 1;
                hasToSplit = true;
            }
            if (halves != null)
            {
                if (halves[0])
                    dungeon.Corridors.Add(halves[0]);
                if (halves[1])
                    dungeon.Corridors.Add(halves[1]);
                dungeon.Corridors.Remove(corridor);
            }
            if (corridor.Length == 0)
                dungeon.Corridors.Remove(corridor);
            return halves != null;
        }

        /// <summary>
        /// Merge all the rooms in the dungeon that are touching/intersecting each other.
        /// </summary>
        /// <param name="dungeon">Reference to the dungeon.</param>
        void MergeRooms(Dungeon dungeon)
        {
            for (int i = dungeon.Rooms.Count - 1; i > 0; i--)
            {
                Room roomA = dungeon.Rooms[i];
                for (int j = i - 1; j >= 0; j--)
                {
                    Room roomB = dungeon.Rooms[j];

                    if (roomA.IsTouching(roomB))
                    {
                        roomA.Merge(roomB);
                        roomB.Disconnect();
                        dungeon.Rooms.RemoveAt(j);
                        i = dungeon.Rooms.Count;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Tries to connect every corridor that still isn't connected to something.
        /// </summary>
        /// <param name="dungeon">Reference to the dungeons.</param>
        /// <param name="hangingCorridors">The corridors that still are not attached to another room section (ie. have a dead end)</param>
        private void ConnectLeftoverCorridors(Dungeon dungeon, Queue<Corridor> hangingCorridors)
        {
            if (hangingCorridors.Count == 0)
                return;

            Queue<Corridor> corridorsStillHanging = new Queue<Corridor>(hangingCorridors.Count);

            while (hangingCorridors.Count > 0)
            {
                Corridor corridor = hangingCorridors.Dequeue();
                if (!ExtendCorridor(dungeon, corridor)) //first of all, try to extend the corridor untils it intersects something
                {
                    //if it fails, them remove every corridor that is not connected to another one
                    if (!_allowDeadEnds) corridorsStillHanging.Enqueue(corridor);
                }
            }

            //Must only trim corridors after trying to extend them all!
            while (corridorsStillHanging.Count > 0)
            {
                TrimCorridor(dungeon, corridorsStillHanging.Dequeue());
            }
        }

        /// <summary>
        /// Extends a corridor until it reaches a room or another corridor.
        /// </summary>
        /// <param name="dungeon">Reference to the dungeons.</param>
        /// <param name="corridor">Corridor to be extended.</param>
        /// <returns>True if the corridor was succesfully extended.</returns>
        private bool ExtendCorridor(Dungeon dungeon, Corridor corridor)
        {
            int oldLength = corridor.Length;

            while (corridor.Bounds.xMax < dungeon.Width - corridor.Direction.x
                && corridor.Bounds.yMax < dungeon.Height - corridor.Direction.y
                && dungeon[corridor.LastCell + corridor.Direction] == null)
            {
                corridor.Length++;
            }

            if (oldLength != corridor.Length
                && dungeon[corridor.LastCell + corridor.Direction] != null)
            {
                dungeon.MarkCells(corridor);
                return true;
            }
            else
            {
                corridor.Length = oldLength;
                return false;
            }
        }

        /// <summary>
        /// Reduces the length of a corridor in order for it not to be a dead end.
        /// </summary>
        /// <param name="dungeon">Reference to the dungeons.</param>
        /// <param name="corridor">Corridor to be reduced.</param>
        private void TrimCorridor(Dungeon dungeon, Corridor corridor)
        {
            int originalLength = corridor.Length;

            bool intersectionFound = false;

            while (corridor.Length > 0 && !intersectionFound)
            {
                //look around the last cell of the corridor
                intersectionFound = (dungeon.SearchAround(corridor.LastCell, 2, false, typeof(Corridor), typeof(Room)) >= 2);

                if (!intersectionFound)
                {
                    //Remove "excess cells" from the corridor
                    dungeon[corridor.LastCell] = null;
                    corridor.Length--;
                }
            }

            if (corridor.Length != originalLength)
            {
                if (corridor.Length < _minimumCorridorLength)
                    dungeon.Corridors.Remove(corridor);
            }
        }

        /// <summary>
        /// Generate a room for every hanging corridor (corridors with no room attatched to it's end).
        /// </summary>
        /// <param name="dungeon">Reference to the dungeons.</param>
        /// <param name="generatedRooms">Container for the rooms that will be generated.</param>
        /// <param name="hangingCorridors">Corridors that need rooms attatched to their end.</param>
        /// <param name="brushVariation">Min and max sizes of the brush that will carve the rooms.</param>
        /// <param name="maximumRoomSize">Maximum width and height for the rooms.</param>
        /// <returns>How many rooms were created.</returns>
        public int GenerateRooms(Dungeon dungeon, Queue<Room> generatedRooms, Queue<Corridor> hangingCorridors, Vector4 brushVariation, Vector2 maxRoomSize, int roomCount)
        {
            //until there are no hanging corridors (that is, corridors with rooms only at their start) 
            while (hangingCorridors.Count > 0 && roomCount < _totalRooms)
            {
                //make a room at the end of a corridor and add it to the queue of rooms without corridors
                Room room;
                Corridor corridor = hangingCorridors.Dequeue();

                if (!corridor)
                {
                    throw new InvalidOperationException("Corridor should not be null!\n" + corridor);
                }
                if (RoomFactory.CarveRoom(dungeon, corridor, brushVariation, maxRoomSize, _maximumTries, out room))
                {
                    generatedRooms.Enqueue(room);
                    dungeon.MarkCells(room);
                    dungeon.Rooms.Add(room);
                    _lastRoom = room;
                    corridor.AddConnection(room);
                    roomCount++;
                }
                else
                {
                    ExtendCorridor(dungeon, corridor);
                }
            }
            return roomCount;
        }


        /// <summary>
        /// Generate corridors going out of rooms.
        /// </summary>
        /// <param name="dungeon">Reference to the dungeon.</param>
        /// <param name="hangingRooms">Rooms without corridors going out of them.</param>
        /// <returns>Every corridor generated.</returns>
        private Queue<Corridor> GenerateCorridors(Dungeon dungeon, Queue<Room> hangingRooms)
        {
            Queue<Corridor> hangingCorridors = new Queue<Corridor>(hangingRooms.Count); //assume at least one corridor per room at first

            //until every room has a corridor going out of it, make corridors
            while (hangingRooms.Count > 0)
            {
                Room room = hangingRooms.Dequeue();
                if (!room)
                    throw new Exception("Something went wrong. Expected a room, but fount " + room + " instead!");

                Corridor corridor;
                for (int i = 0; i < 2; i++)
                {
                    if (CorridorFactory.CarveCorridor(dungeon, room, (i == 0 ? Vector2.right : Vector2.up), new Vector2(_minimumCorridorLength, _maximumCorridorLength), new Vector2(_minimumBrushWidth, _minimumBrushHeight), out corridor))
                    {
                        hangingCorridors.Enqueue(corridor);
                        dungeon.Corridors.Add(corridor);
                        corridor.AddConnection(room);
                        dungeon.MarkCells(corridor);
                    }
                }
            }

            return hangingCorridors;
        }
    }
}