using UnityEngine;
using System.Collections;

public class Trap : DungeonFeature
{
    private TrapBehaviour associatedBehaviour;

    public TrapBehaviour AssociatedBehaviour
    {
        get { return associatedBehaviour; }
        set { associatedBehaviour = value; }
    }
}
