using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityQuery;

namespace Finsternis
{
    [DisallowMultipleComponent]
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
        private int dungeonWidth = 20;

        [SerializeField]
        [Range(10, 1000)]
        private int dungeonHeight = 20;

        [SerializeField]
        [Range(2, 1000)] //at least one starting point and one exit
        private int totalRooms = 5;

        [Tooltip("How many times should it try to carve a room before stoping")]
        [SerializeField]
        [Range(1, 100)]
        private int maximumTries = 2;

        [Header("Maximum room size")]
        [SerializeField]
        [Range(2, 1000)]
        private int maximumRoomWidth;

        [SerializeField]
        [Range(2, 1000)]
        private int maximumRoomHeight;

        [SerializeField]
        private BrushSizeVariation brushSizeVariation = new BrushSizeVariation(1,1, 10, 10);

        [Header("Corridors")]
        [SerializeField]
        [Range(1, 100)]
        private int minimumCorridorLength = 1;

        [SerializeField]
        [Range(1, 100)]
        private int maximumCorridorLength = 5;

        [SerializeField]
        private bool allowDeadEnds = false;

        [Space]
        [SerializeField]
        private RoomTheme[] roomThemes;

        [SerializeField]
        private CorridorTheme[] corridorThemes;

        #endregion
        
        public const string SEED_KEY = "LastSeed";

        /// <summary>
        /// Initializes every field, ensuring they have consistent values.
        /// </summary>
        /// <param name="dungeon">Reference to the dungeon.</param>
        public void Init(Dungeon dungeon)
        {
            dungeon.Init(this.dungeonWidth, this.dungeonHeight);
            this.brushSizeVariation.Validate(this.dungeonWidth, this.dungeonHeight);
            this.maximumCorridorLength = Mathf.Clamp(this.maximumCorridorLength, 0, Mathf.Min(dungeon.Height, dungeon.Width));
            this.minimumCorridorLength = Mathf.Clamp(this.minimumCorridorLength, 0, this.maximumCorridorLength);
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

            Vector2 maxRoomSize = new Vector2(this.maximumRoomWidth, this.maximumRoomHeight);

            Room room;
            if (RoomFactory.CarveRoom(dungeon, null, brushSizeVariation, maxRoomSize, this.maximumTries, out room))
            {
                dungeon.MarkCells(room);
                hangingRooms.Enqueue(room);
                dungeon.Rooms.Add(room);
                dungeon.Entrance = room.GetRandomCell();
            }
            else
                throw new InvalidOperationException("Could not create a single room within the dungeon. Maybe it is too small and the room too big?");

            int roomCount = 1;
            while (roomCount < this.totalRooms  //keep going until the desired number of rooms was generated
                    && (hangingRooms.Count > 0 || hangingCorridors.Count > 0)) //or until no more rooms or corridors can be generated
            {
                hangingCorridors = GenerateCorridors(dungeon, hangingRooms);

                roomCount = GenerateRooms(dungeon, hangingRooms, hangingCorridors, maxRoomSize, roomCount);
            }

            if (!this.allowDeadEnds)
                ConnectLeftoverCorridors(dungeon, hangingCorridors);

            CleanUp(dungeon);

            DefineThemes(dungeon);

            AddFeatures(dungeon);

            Room r = GetFarthestRoom(dungeon);

            dungeon.Exit = GetFarthestCell(dungeon, r);

            r.AddFeature(r.GetTheme<RoomTheme>().GetRandomExit(), dungeon.Exit);

            PlayerPrefs.SetInt(SEED_KEY, dungeon.Seed);

            if (onGenerationEnd)
                onGenerationEnd.Invoke(dungeon);
        }

        private Vector2 GetFarthestCell(Dungeon dungeon, Room r, int tries = 20)
        {
            Vector2 farthest = r.GetRandomCell();

            while(--tries > 0)
            {
                Vector2 candidate = r.GetRandomCell();
                if (dungeon.Entrance.Distance(candidate) > dungeon.Entrance.Distance(farthest))
                    farthest = candidate;
            }
            return farthest;
        }

        private Room GetFarthestRoom(Dungeon dungeon)
        {
            Room farthest = dungeon.Rooms[0];
            for(int i = 1; i < dungeon.Rooms.Count; i++)
            {
                if (dungeon.Entrance.Distance(farthest.Bounds.center) < dungeon.Entrance.Distance(dungeon.Rooms[i].Bounds.center))
                    farthest = dungeon.Rooms[i];
            }
            return farthest;
        }

        void DefineThemes(Dungeon dungeon)
        {
            foreach (var room in dungeon.Rooms)
                room.SetTheme(this.roomThemes.GetRandom(Dungeon.Random.IntRange));

            foreach (var corridor in dungeon.Corridors)
                corridor.SetTheme(this.corridorThemes.GetRandom(Dungeon.Random.IntRange));
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
                AddDoors(dungeon, corridor);
            }

            foreach (Room room in dungeon.Rooms)
            {
                Decorate(room);
            }
        }

        private void Decorate(Room room)
        {
            var theme = room.GetTheme<RoomTheme>();
            if (!theme.HasDecorations())
                return;

            int amount = Dungeon.Random.IntRange(0, room.CellCount/2);
            while (--amount >= 0)
            {
                room.AddFeature(theme.GetRandomDecoration(), room.GetRandomCell());
            }
        }

        /// <summary>
        /// Randomly adds a trap to a corridor.
        /// </summary>
        /// <param name="corridor">The corridor to be trapped.</param>
        private void AddTrap(Corridor corridor)
        {
            if (corridor.Length > 2 && Dungeon.Random.value() <= 0.3f)
            {
                int pos = 0;
                int count = 0, maxTries = 10;
                do
                    pos = Dungeon.Random.IntRange(1, corridor.Length - 1); //-1 because we don't want to count the last cell either (it should never have a trap)
                while ((++count) <= maxTries && !corridor.AddTrap(corridor[pos]));
            }
        }

        /// <summary>
        /// Add doors at the junction between rooms and corridors.
        /// </summary>
        /// <param name="dungeon">Reference to the dungeon.</param>
        /// <param name="corridor">The corridor that will have doors added to it.</param>
        private void AddDoors(Dungeon dungeon, Corridor corridor)
        {
            //Add door at the start
            var cell = corridor[0];
            corridor.AddDoor(cell, -corridor.Direction * 0.75f);

            //Add door at the end
            cell = corridor.End;
            corridor.AddDoor(cell, corridor.Direction * 0.75f);
        }

        /// <summary>
        /// Removes corridors that don't really look like corridors (eg. have walls only on one side) 
        /// and merge rooms that overlap or don't have walls between one or more cells
        /// </summary>
        /// <param name="dungeon">Reference to the dungeon.</param>
        private void CleanUp(Dungeon dungeon)
        {
            FixCorridors(dungeon);
            MergeRooms(dungeon);
            RemoveUnnecessaryCorridors(dungeon);

            foreach (var corridor in dungeon.Corridors)
                dungeon.MarkCells(corridor);

            foreach (var room in dungeon.Rooms)
                dungeon.MarkCells(room);
        }

        /// <summary>
        /// Incorporates corridors that have only 1 of length and connect a room to itself with said room. 
        /// </summary>
        /// <param name="dungeon">Reference to the dungeon.</param>
        void RemoveUnnecessaryCorridors(Dungeon dungeon)
        {
            for (int i = dungeon.Corridors.Count - 1; i >= 0; i--)
            {
                Corridor corridor = dungeon.Corridors[i];

                if (corridor.Length != 1 || corridor.Connections.Count > 1)
                    continue;

                Room connectedRoom = dungeon[corridor[0] - corridor.Direction] as Room;
                if (connectedRoom)
                {
                    connectedRoom.AddCell(corridor[0]);
                    connectedRoom.RemoveConnection(corridor);
                    dungeon.Corridors.RemoveAt(i);
                }
            }
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
                if (ReduceCorridor(dungeon, corridor))
                {
                    i = dungeon.Corridors.Count;
                    continue;
                }
                if (!ExtendCorridor(dungeon, corridor))
                    TrimCorridor(dungeon, corridor);
            }
        }

        private bool CheckCell<T>(Dungeon dungeon, Vector2 cell) where T : DungeonSection
        {
            return (dungeon.IsWithinDungeon(cell) && dungeon[cell] && !(dungeon[cell] is T));
        }

        private Corridor[] RemoveCell(Dungeon dungeon, int index, Corridor corridor, Vector2 offsetCell)
        {
            Corridor[] halves = null;
            Vector2 cell = corridor[index];
            dungeon[cell] = dungeon[offsetCell];
            dungeon[offsetCell].AddCell(cell);
            if (index < corridor.Length - 1)
                halves = corridor.RemoveAt(index);
            else
                corridor.Length--;
            return halves;
        }

        /// <summary>
        /// Splits or shorten corridors that have cells without walls on at least two sides.
        /// </summary>
        /// <param name="dungeon">Reference to the dungeon.</param>
        /// <param name="corridor">Corridor to be split.</param>
        /// <returns>True is the corridor was split.</returns>
        private bool ReduceCorridor(Dungeon dungeon, Corridor corridor)
        {
            Corridor[] halves = null;
            Vector2 offset = corridor.Direction.YX();

            bool corridorChanged = false;
            int index = corridor.Length - 1;

            while (index >= 0)
            {
                Vector2 cell = corridor[index];
                Vector2 offsetCellA = cell + offset;
                Vector2 offsetCellB = cell - offset;
                if (CheckCell<Corridor>(dungeon, offsetCellA))
                {
                    corridorChanged = true;
                    halves = RemoveCell(dungeon, index, corridor, offsetCellA);
                    if (halves != null)
                        break;

                }
                else if (CheckCell<Corridor>(dungeon, offsetCellB))
                {
                    corridorChanged = true;
                    halves = RemoveCell(dungeon, index, corridor, offsetCellB);

                    if (halves != null)
                        break;
                }
                index--;

            }

            if (halves != null)
            {
                foreach (var half in halves)
                {
                    if (half)
                    {
                        dungeon.Corridors.Add(half);
                        UpdateConnections(dungeon, half);
                    }
                }
                dungeon.Corridors.Remove(corridor);
            } else if (corridor.Length == 0)
                dungeon.Corridors.Remove(corridor);
            return corridorChanged;
        }

        private void UpdateConnections(Dungeon dungeon, Corridor corridor)
        {
            foreach (var cell in corridor)
            {
                foreach (var neighbour in GetNeighbourhood(dungeon, cell))
                {
                    if (neighbour && neighbour != corridor)
                    {
                        neighbour.AddConnection(corridor, true);
                    }
                }
            }
        }

        private List<DungeonSection> GetNeighbourhood(Dungeon dungeon, Vector2 cell)
        {
            var neighbourhood = new List<DungeonSection>(4);
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (Mathf.Abs(i) == Mathf.Abs(j)) //ignore diagonals and the cell itself
                        continue;
                    Vector2 neighbour = cell + new Vector2(i, j);
                    if (dungeon.IsWithinDungeon(neighbour))
                        neighbourhood.Add(dungeon[neighbour]);
                }
            }

            return neighbourhood;
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
                    if (!this.allowDeadEnds)
                        corridorsStillHanging.Enqueue(corridor);
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
                && dungeon[corridor.End + corridor.Direction] == null)
            {
                corridor.Length++;
            }

            if (oldLength != corridor.Length
                && dungeon[corridor.End + corridor.Direction] != null)
            {
                dungeon.MarkCells(corridor);
                corridor.AddConnection(dungeon[corridor.End + corridor.Direction], true);
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
                intersectionFound = (dungeon.SearchAround(corridor.End, 2, false, typeof(Corridor), typeof(Room)) >= 2);

                if (!intersectionFound)
                {
                    //Remove "excess cells" from the corridor
                    dungeon[corridor.End] = null;
                    corridor.Length--;
                }
            }

            if (corridor.Length != originalLength)
            {
                if (corridor.Length < this.minimumCorridorLength)
                    dungeon.Corridors.Remove(corridor);
            }
        }

        public T GetRandomTheme<T>() where T : DungeonSectionTheme
        {
            if (typeof(RoomTheme).Equals(typeof(T)))
                return roomThemes.GetRandom(Dungeon.Random.IntRange) as T;
            else if (typeof(CorridorTheme).Equals(typeof(T)))
                return corridorThemes.GetRandom(Dungeon.Random.IntRange) as T;

            return default(T);
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
        public int GenerateRooms(Dungeon dungeon, Queue<Room> generatedRooms, Queue<Corridor> hangingCorridors, Vector2 maxRoomSize, int roomCount)
        {
            //until there are no hanging corridors (that is, corridors with rooms only at their start) 
            while (hangingCorridors.Count > 0 && roomCount < this.totalRooms)
            {
                //make a room at the end of a corridor and add it to the queue of rooms without corridors
                Room room;
                Corridor corridor = hangingCorridors.Dequeue();

                if (!corridor)
                {
                    throw new InvalidOperationException("Corridor should not be null!\n" + corridor);
                }
                if (RoomFactory.CarveRoom(dungeon, corridor, brushSizeVariation, maxRoomSize, this.maximumTries, out room))
                {
                    generatedRooms.Enqueue(room);
                    dungeon.MarkCells(room);
                    dungeon.Rooms.Add(room);
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
                    if (CorridorFactory.CarveCorridor(dungeon, room, (i == 0 ? Vector2.right : Vector2.up), new Vector2(this.minimumCorridorLength, this.maximumCorridorLength), this.brushSizeVariation.Min, out corridor))
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