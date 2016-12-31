namespace Finsternis.Extensions
{
    using UnityEngine;

    public static class Transforms
    {

        public static Transform FindDescendant(this Transform parent, string name)
        {
            Transform child = parent.Find(name);
            if (!child)
            {
                foreach (Transform t in parent)
                {
                    child = t.FindDescendant(name);
                    if (child)
                        break;
                }
            }

            return child;
        }
    }
}
