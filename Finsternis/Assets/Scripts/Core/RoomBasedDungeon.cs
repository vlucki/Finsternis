using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomBasedDungeon : Dungeon
{
    [Header("Dungeon Dimensions")]
    //Dungeon dimensions, in cells (each cell represents 1 Unity cube)
    public int width = 32;
    public int height = 32;

    //Cells around the edges of the dungeon that will be used to expand the rooms
    public int horizontalPadding = 4;
    public int verticalPadding = 4;

    [Header("Section settings")]
    //How many areas should be generated
    public int horizontalSectionsCount = 4;
    public int verticalSectionsCount = 4;

    [Tooltip("How many sections may be merged together at once")]
    public int maxSectionMerges = 2;

    [Range(0.0f, 1.0f)]
    [Tooltip("Chance of an attempt to merge sections happening.")]
    public float mergeThreshold = 0.5f;

    [Range(0.5f, 1.0f)]
    [Tooltip("Percentage of each section that must be filled before being considered done")]
    float fillRate = 0.5f;

    private DungeonSection[,] _sections;

    public override void Generate()
    {
        if (width < 0 || height < 0)
            throw new System.ArgumentOutOfRangeException("Neither width nor height can be less than 0");
        if (verticalPadding < 0 || horizontalPadding < 0)
            throw new System.ArgumentOutOfRangeException("Padding can't be less than 0");
        if (verticalPadding > width - 2 || horizontalPadding > height - 2)
            throw new System.ArgumentOutOfRangeException("Not enought space to generate dungeon - reduce padding and room count or increase dungeon dimension.");

        CheckRandomGenerator();
        CreateSections();
        MergeSections();
        ExpandDungeon();
        MakeCorridors();

        //TODO: remove temporary method!
        DisplayDungeon();
    }

    private void DisplayDungeon()
    {
        HashSet<DungeonSection> displayed = new HashSet<DungeonSection>();
        foreach(DungeonSection section in _sections)
        {
            if (displayed.Add(section))
            {
                section.DisplayRooms();
            }
        }
    }

    //Assure both the dungeon and the random generator have the same seed (this way the seed may be changed at any time without messing the generation up)
    private void CheckRandomGenerator()
    {
        if (random == null || (random.Seed != Seed))
        {
            random = new DungeonRandom((int)Seed);
        }
    }

    private void CreateSections()
    {
        _sections = new DungeonSection[verticalSectionsCount, horizontalSectionsCount]; //iniatlly every area is disconnected and, therefore, contains a single section

        //define the best dimensions for the sections
        int sectionWidht = (width - 2 * horizontalPadding) / horizontalSectionsCount;
        int ectionHeight = (height - 2 * verticalPadding) / verticalSectionsCount;

        //iterate through the dungeon, moving from section to section
        for (int row = 0; row < verticalSectionsCount; row++)
        {
            for (int col = 0; col < horizontalSectionsCount; col++)
            {
                //Create a new area and make a section within it
                var section = ScriptableObject.CreateInstance<DungeonSection>();
                section.Init(random);
                section.CreateRoom(fillRate, row, col, sectionWidht, ectionHeight);
                _sections[row, col] = section;
            }
        }
    }

    private void MergeSections()
    {
        //iterate through the dungeon, moving from area to area
        for (int row = 0; row < verticalSectionsCount; row++)
        {
            for (int col = 0; col < horizontalSectionsCount; col++)
            {
                DungeonSection section = _sections[row, col];

                if (section.RoomsCount < maxSectionMerges && random.NextDouble() < mergeThreshold)
                {
                    int randomCellCol = col, randomCellRow = row;
                    DungeonSection toMerge = GetRandomAdjacentSection(_sections, ref randomCellRow, ref randomCellCol);
                    if (toMerge && toMerge != section)
                    {
                        section += toMerge;
                        _sections[randomCellRow, randomCellCol] = section; //replace the random section that was merged for the current section
                    }
                }
            }
        }
    }

    private DungeonSection GetRandomAdjacentSection(DungeonSection[,] sections, ref int row, ref int col)
    {
        DungeonSection section = null;
        float randomValue = random.Next(0, 4);
        int offset = (int)(randomValue / 2) * 2 - 1; //0 and 1 results in -1, 2 and 3 results in 1

        //will be true only if randomValue is 0 or 2
        if((int)(randomValue/2) * 2 == randomValue)
        {
            section = GetSection(row + offset, col); //look above or below the given coordinates
        }
        else
        {
            section = GetSection(row, col + offset); //look to the left or right of the given coordinates
        }

        return section;
    }

    private DungeonSection GetSection(int row, int col)
    {
        if (row >= 0 && row < _sections.GetLength(0) && col >= 0 && col < _sections.GetLength(1))
            return _sections[row, col];

        return null;
    }

    private void ExpandDungeon()
    {
        HashSet<DungeonSection> moved = new HashSet<DungeonSection>(); //stores the sections that were moved already (needed since a section may ocupy more than one spot on the grid after the merges)

        for (int row = 0; row < verticalSectionsCount; row++)
        {
            for (int col = 0; col < horizontalSectionsCount; col++)
            {
                DungeonSection section = _sections[row, col];
                if (moved.Contains(section)) //if this section was already moved, ignore it
                    continue;

                moved.Add(section);
                section.MoveAwayFrom(verticalSectionsCount / 2, horizontalSectionsCount / 2, verticalPadding, horizontalPadding);
            }
        }
    }

    private void MakeCorridors()
    {

    }
}
