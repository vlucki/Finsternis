namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using Extensions;

    public class CustomBehaviour : MonoBehaviour
    {
        public void DestroyObject(GameObject target)
        {
            target.DestroyNow();
        }

        public void DestroyObject()
        {
            DestroyObject(this.gameObject);
        }

        public void RemoveBehaviour()
        {
            this.DestroyNow();
        }

        public void RemoveAllBehaviours()
        {
            foreach (var behaviour in this.GetComponents<MonoBehaviour>())
            {
                behaviour.DestroyNow();
            }
        }
    }
}