namespace Finsternis
{
    public class BehaviourEvent_Awake : CustomBehaviour
    {
        public BehaviourEvent onAwake;
        public virtual void Awake()
        {
            onAwake.Invoke(this);
        }
    }
}