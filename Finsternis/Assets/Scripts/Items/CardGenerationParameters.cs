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
            public List<AttributeModifier> effects;
            public string flavourText;
        }

        [SerializeField]
        private NameParameters[] preNames;
        [SerializeField]
        private NameParameters[] mainNames;
        [SerializeField]
        private NameParameters[] postNames;
    }
}