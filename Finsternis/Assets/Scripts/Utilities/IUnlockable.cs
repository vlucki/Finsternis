namespace Finsternis
{
    public interface IUnlockable
    {
        void AddLock(KeyCard keyCard);
        void RemoveLock(KeyCard keyCard);
        bool IsLocked { get; }
    }
}