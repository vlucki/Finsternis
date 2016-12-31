﻿public interface IConstraint
{
    /// <summary>
    /// Validates the conditions imposed by the constraint.
    /// </summary>
    /// <returns>True if the conditions are met.</returns>
    bool Validate();

    /// <summary>
    /// Is it possible to use multiple constraints of this type on the same object? 
    /// Usually false when the constraint makes use of a unique property/situation that could cause a conflict,
    /// such as time, coordinates, specific attribute values, etc.
    /// </summary>
    /// <returns>True if multiple constraints of this type may be used on the same object.</returns>
    bool AllowMultiple();
}

public interface IConstraint<T>
{
    /// <summary>
    /// Validates the conditions imposed by the constraint.
    /// </summary>
    /// <param name="value">Value to be checked.</param>
    /// <returns>True if the conditions are met.</returns>
    bool Validate(T value);

    /// <summary>
    /// Is it possible to use multiple constraints of this type on the same object? 
    /// Usually false when the constraint makes use of a unique property/situation that could cause a conflict,
    /// such as time, coordinates, specific attribute values, etc.
    /// </summary>
    /// <returns>True if multiple constraints of this type may be used on the same object.</returns>
    bool AllowMultiple();


}