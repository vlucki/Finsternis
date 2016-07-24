using UnityEngine;

[CreateAssetMenu(fileName = "DungeonFeature", menuName = "Finsternis/DungeonFeature/Generic Feature", order = 1)]
public class DoorFeature : DungeonFeature
{
    [SerializeField]
    private bool _closed = true;
    [SerializeField]
    private bool _locked = false;

    public bool Closed {
        get { return _closed; }
        set { _closed = value; }
    }
    public bool Locked {
        get { return _locked; }
        set { _locked = value; }
    }
}
