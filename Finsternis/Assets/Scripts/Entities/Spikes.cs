namespace Finsternis
{
    public class Spikes : TrapBehaviour
    {
        /// <summary>
        /// Spikes can't be attacked and will damage any entity that interacts with it, provided said entity can be damaged
        /// </summary>
        public override void Interact(EntityAction action)
        {
            attack.Execute(action.Agent);
        }
    }
}