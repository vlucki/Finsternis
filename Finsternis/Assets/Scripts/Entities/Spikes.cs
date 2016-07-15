namespace Finsternis
{
    public class Spikes : TrapBehaviour
    {
        public override void Interact(EntityAction action)
        {
            attack.Perform(action.Agent, DamageInfo.DamageType.physical);
        }
    }
}