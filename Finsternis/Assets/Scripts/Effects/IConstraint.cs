public interface IConstraint
{
    bool Validate();
    bool AllowMultiple();
}