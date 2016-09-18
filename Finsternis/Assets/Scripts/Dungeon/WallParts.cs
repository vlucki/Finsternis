using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName ="WallParts", menuName ="Finsternis/Dungeon/Walls")]
public class WallParts : ScriptableObject
{
    [SerializeField]
    private string[] tags;

    [SerializeField]
    private GameObject wallPrefab;

    public GameObject GetLateral()
    {
        return wallPrefab.transform.FindChild("lateral").gameObject;
    }

    public GameObject GetTop()
    {
        return wallPrefab.transform.FindChild("top").gameObject;
    }

    public bool HasTags(params string[] tags)
    {
        foreach(var tag in this.tags)
        {
            bool tagFound = false;
            foreach(var searchedTag in tags)
            {
                if (tag.Equals(searchedTag))
                    tagFound = true;
            }
            if (!tagFound)
                return false;
        }

        return true;
    }
}
