namespace Finsternis
{
    public class Spikes : TrapBehaviour
    {
        public override void Interact(EntityAction action)
        {
            attack.Execute(action.Agent);
        }
    }
}