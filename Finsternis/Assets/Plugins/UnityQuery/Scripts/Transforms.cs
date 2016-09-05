namespace UnityQuery
{
    using System;
    using UnityEngine;

    public static class Transforms
    {

        public static Transform FindDescendent(this Transform t, string name)
        {
            Transform found = SearchChild(t, name);

            return found;
        }

        private static Transform SearchChild(Transform parent, string name)
        {
            Transform child = parent.Find(name);
            if (!child)
            {
                foreach (Transform t in parent)
                {
                    child = SearchChild(t, name);
                    if (child)
                        break;
                }
            }

            return child;
        }
    }
}
