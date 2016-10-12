namespace Finsternis
{
    public interface IOpeneable
    {
        bool IsOpen { get; }
        void Open();
        void Close();
    }
}