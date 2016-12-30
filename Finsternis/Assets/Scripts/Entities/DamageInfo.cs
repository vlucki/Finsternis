namespace Finsternis
{
    public sealed class DamageInfo
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

        public DamageInfo(DamageType type, float amount, Entity source)
        {
            this.type = type;
            this.amount = amount;
            this.source = source;
        }
    }
}
