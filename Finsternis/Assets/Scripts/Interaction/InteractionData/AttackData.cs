namespace Finsternis
{
    using System;

    public sealed class AttackData : InteractionData
    {
        public enum DamageType
        {
            physical = 0,
            magical = 10,
            absolute = 20
        }

        private DamageType type;
        private float amount;
        private Entity source;

        public DamageType Type { get { return this.type; } }
        public float Amount { get { return this.amount; } }
        public Entity Source { get { return this.source; } }

        public override Type InteractionType
        {
            get
            {
                return typeof(AttackInteraction);
            }
        }

        public AttackData(float amount, Entity source, DamageType type = DamageType.physical) : base(source)
        {
            this.type = type;
            this.amount = amount;
            this.source = source;
        }
    }
}
