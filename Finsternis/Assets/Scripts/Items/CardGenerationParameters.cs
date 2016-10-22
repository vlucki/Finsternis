namespace Finsternis
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "CardGenerationParameters", menuName = "Finsternis/Cards/Generation Parameters")]
    public class CardGenerationParameters : ScriptableObject
    {
        [Serializable]
        public struct NameParameters
        {
            public string nameString;
            [Space(10)]
            public List<string> prepositions;
            [Space(10)]
            public List<AttributeModifier> effects;
            [Space(10)]
            public string flavourText;
        }

        [SerializeField]
        private NameParameters[] preNames;

        [Space(20)]
        [SerializeField]
        private NameParameters[] mainNames;

        [Space(20)]
        [SerializeField]
        private NameParameters[] postNames;

        public NameParameters[] PreNames { get { return this.preNames; } }
        public NameParameters[] MainNames { get { return this.mainNames; } }
        public NameParameters[] PostNames { get { return this.postNames; } }

    }
}