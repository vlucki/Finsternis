using UnityEngine;
using System.Collections;

namespace Finsternis {
    //Items that are on the floor
    public class Pickable : MonoBehaviour
    {
        [SerializeField]
        private Card _item;

        // Use this for initialization
        void Start()
        {
            //if there is no item set for this pickable, it should not exist
            if (!_item)
            {
                Destroy(gameObject);
            }
        }
    }    
}
