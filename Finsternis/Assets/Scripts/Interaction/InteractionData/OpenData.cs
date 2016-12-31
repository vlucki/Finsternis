namespace Finsternis
{
    using System;
    using System.Collections.Generic;

    public class OpenData : InteractionData
    {
        public override Type InteractionType
        {
            get
            {
                return typeof(OpenInteraction);
            }
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<KeyCard> Keys { get { return keys.AsReadOnly(); } }

        private List<KeyCard> keys;

        public OpenData(Entity e, params KeyCard[] keys) : base(e)
        {
            this.keys = new List<KeyCard>(keys);
            this.keys.ForEach(k => k.onUse += K_onUse);
        }

        private void K_onUse(KeyCard key)
        {
            if (key.UsedUp)
                this.keys.Remove(key);
        }
    }
}