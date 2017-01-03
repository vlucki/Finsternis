namespace Finsternis
{
    public class BehaviourEvent_Start : CustomBehaviour
    {
        public Events.BehaviourEvent onStart;
        public virtual void Start()
        {
            onStart.Invoke(this);
        }
    }
}