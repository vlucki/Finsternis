using System;
using UnityEngine;

public class RoomBasedDungeon : Dungeon
{

    private static readonly uint MinumumBrushWeight = 2;

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

    //How many sections may be merged together at once
    public int maxSectionMerges = 2;

    [Range(0, 1)]
    [Tooltip("Chance of an attempt to merge sections happening.")]
    public float mergeThreshold;

    [Range(0, 1)]
    [Tooltip("Percentage of each section that must be filled before being considered done")]
    float fillRate = 0.5f;

    private DungeonSection[,] _sections;

    // Use this for initialization
    void Start()
    {
    }

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
    }

    //Assure both the dungeon and the random generator have the same seed (this way the seed may be changed at any time without messing the generation up)
    private void CheckRandomGenerator()
    {
        if (random == null || (random.Seed != Seed))
        {
            if (Seed == null)
            {
                Seed = System.DateTime.Now.Millisecond;
            }
            random = new DungeonRandom((int)Seed);
        }
    }

    private void CreateSections()
    {
        _sections = new DungeonSection[horizontalSectionsCount, verticalSectionsCount]; //iniatlly every area is disconnected and, therefore, contains a single section

        //define the best dimensions for the area sections
        int areaWidht = (width - 2 * horizontalPadding) / horizontalSectionsCount;
        int areaHeight = (height - 2 * verticalPadding) / verticalSectionsCount;

        //iterate through the dungeon, moving from area to area
        for (var row = 0; row < verticalSectionsCount; row++)
        {
            for (var col = 0; col < horizontalSectionsCount; col++)
            {
                //Create a new area and make a section within it
                var area = ScriptableObject.CreateInstance<DungeonSection>();
                area.CreateRoom(fillRate, col, row, areaWidht, areaHeight);
                _sections[row, col] = area;
            }
        }
    }

    private void MergeSections()
    {
        //iterate through the dungeon, moving from area to area
        for (var row = 0; row < verticalSectionsCount; row++)
        {
            for (var col = 0; col < horizontalSectionsCount; col++)
            {
                DungeonSection section = _sections[col, row];

                if (section.RoomsCount < maxSectionMerges && random.NextDouble() < mergeThreshold)
                {
                    DungeonSection toMerge = GetRandomAdjacentSection(_sections, col, row);
                    if (toMerge && toMerge != section)
                    {
                        section += toMerge;
                    }
                }
            }
        }
    }

    private DungeonSection GetRandomAdjacentSection(DungeonSection[,] areas, int col, int row)
    {
        DungeonSection section = default(DungeonSection);
        switch (random.Next(0, 4))
        {
            case 0:
                if (row > 0 && areas[row, col]) //tries to get the section above
                {
                    section = areas[row - 1, col];
                }
                break;
            case 1:

                if (row < verticalSectionsCount) //tries to get the section below
                {
                    section = areas[row + 1, col];
                }
                break;
            case 2:
                if (col > 0) //tries to get the section to the left
                {
                    section = areas[row, col - 1];
                }
                break;
            case 3:
                if (col < horizontalSectionsCount) //tries to get the section to the right
                {
                    section = areas[row, col + 1];
                }
                break;
        }
        return section;
    }

    private void ExpandDungeon()
    {

    }

    private void MakeCorridors()
    {

    }
}
