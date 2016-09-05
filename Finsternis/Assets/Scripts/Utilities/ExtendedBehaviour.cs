namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityQuery;
    

    public class ExtendedBehaviour : MonoBehaviour
    {

        [SerializeField]
        private bool makeObjectSingleton = false;

        [SerializeField]
        [Tooltip("Recommended to be used only with singleton objects.")]
        private bool dontDestroyOnLoad = false;

        private static List<ExtendedBehaviour> instances;

        protected virtual void Awake()
        {
            if (makeObjectSingleton)
            {
                if (instances == null)
                    instances = new List<ExtendedBehaviour>();
                if (instances.Contains(this))
                {
                    gameObject.DestroyNow();
                    return;
                }

                instances.Add(this);
            }
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(this.gameObject);

        }

        public override int GetHashCode()
        {
            if (!makeObjectSingleton)
                return base.GetHashCode();

            return name.GetHashCode() * 3 + tag.GetHashCode() * 5 + GetComponentsInChildren<MonoBehaviour>().GetHashCode() * 7;
        }

        void OnDestroy()
        {
            if (makeObjectSingleton)
                instances.Remove(this);
        }

        public override bool Equals(object o)
        {
            if (!makeObjectSingleton)
                return base.Equals(o);

            ExtendedBehaviour eBehaviour = o as ExtendedBehaviour;
            if (!eBehaviour)
                return false;

            if (!eBehaviour.name.Equals(this.name) || !eBehaviour.CompareTag(this.tag))
                return false;

            List<GameObject> childrenInThis =  new List<GameObject>(gameObject.GetChildren());
            List<GameObject> childrenInOther = new List<GameObject>(eBehaviour.gameObject.GetChildren());


            if (childrenInThis.Count != childrenInOther.Count)
                return false;

            if (childrenInThis.Count > 0)
            {
                HashSet<GameObject> mathcingChildren = new HashSet<GameObject>();

                foreach (var childOfThis in childrenInThis)
                {
                    foreach (var childOfOther in childrenInOther)
                    {
                        if (mathcingChildren.Contains(childOfOther))
                            continue;

                        if (childOfThis.name.Equals(childOfOther.name) && childOfThis.CompareTag(childOfOther.tag))
                        {
                            mathcingChildren.Add(childOfOther);
                            break;
                        }
                    }
                }

                return mathcingChildren.Count == childrenInThis.Count;
            }


            return true;
        }
    }
}