using System;

namespace Finsternis
{
    public abstract class InteractionData
    {
        public Entity Entity { get; private set; }

        public abstract Type InteractionType { get; }

        public InteractionData(Entity e)
        {
            this.Entity = e;
        }

        public static implicit operator bool(InteractionData data)
        {
            return data != null;
        }
    }

    public class InteractionData<T> : InteractionData where T : Interaction
    {

        public override Type InteractionType { get { return typeof(T); } }
        public InteractionData(Entity e) : base(e)
        {
        }
    }
}