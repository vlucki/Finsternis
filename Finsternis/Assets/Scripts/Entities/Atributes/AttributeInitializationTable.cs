namespace Finsternis
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Collections;

    [Serializable]
    public struct Influence
    {
        public EntityAttribute template;
        public RangeF range;
    }

    [CreateAssetMenu(fileName = "AttributeInitializationTable", menuName = "Attribute Initialization Table")]
    public class AttributeInitializationTable : ScriptableObject, IEnumerable<Influence>
    {

        [SerializeField]
        private List<Influence> influencedAttributes;

        public Influence this[int i] { get { return this.influencedAttributes[i]; } }

        public Influence GetInfluence(String attributeAlias)
        {
            return this.influencedAttributes.Find(influence => influence.template.Alias.Equals(attributeAlias));
        }

        public IEnumerator<Influence> GetEnumerator()
        {
            return this.influencedAttributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            for(int i = 0; i < this.influencedAttributes.Count; i++)
            {
                for (int j = this.influencedAttributes.Count - 1; j > i; j--)
                {
                    if (this.influencedAttributes[i].template == null || this.influencedAttributes[j].template == null)
                        continue;
                    if(this.influencedAttributes[i].template.Alias.Equals(this.influencedAttributes[j].template.Alias))
                    {
                        this.influencedAttributes.Insert(j, default(Influence));
                        this.influencedAttributes.RemoveAt(j+1);
                    }
                }
            }
            this.influencedAttributes.ForEach(influence =>
                influence.range.max = Mathf.Max(influence.range.max, influence.range.min));
        }
#endif
    }
}