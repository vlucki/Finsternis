using UnityEngine;
using System.Collections;

public class DungeonFeature : ScriptableObject
{
    private int id = -1;
    public int Id
    {
        get { return id; }
        set { if (id < 0) id = value; }
    }
}
