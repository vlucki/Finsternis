namespace Finsternis
{

    public class KeyCard : Card
    {
        private int uses = 1; //how many times this keycard may be used (-1 for infinitely usable)

        public bool UsedUp { get { return uses == 0; } }

        public void SetUses(int uses)
        {
            this.uses = UnityEngine.Mathf.Max(uses, -1);
        }

        public void Use()
        {
            if (uses > 0)
                uses--;
        }
    }
}