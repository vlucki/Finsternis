namespace UnityQuery
{
    using UnityEngine;

    public static class Components
    {
        public static T GetComponentInSibling<T>(this Component c) where T : Component
        {
            T siblingComponent = c.GetComponentInParent<T>();
            if (!siblingComponent)
                siblingComponent = c.GetComponentInChildren<T>();
            return siblingComponent;
        }
    }
}