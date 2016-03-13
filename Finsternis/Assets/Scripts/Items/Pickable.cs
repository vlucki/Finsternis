using UnityEngine;
using System.Collections;

//Items that are on the floor
public class Pickable : Entity
{
    public Item item;

    // Use this for initialization
    void Start()
    {
        //if there is no item set for this pickable, it should not exist
        if (!item)
        {
            Destroy(gameObject);
        }
    }

    
}
