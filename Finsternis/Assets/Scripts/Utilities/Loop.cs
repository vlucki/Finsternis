using System;

public static class Loop
{
    /// <summary>
    /// Helper method that provides a deadlock-free while loop
    /// </summary>
    /// <param name="conditionCheck">The conditions that must be met for the loop to keep running.</param>
    /// <param name="callback">What must happen each iteration.</param>
    /// <param name="maxIterations">Max number of iterations before exiting loop (even if the passed condition still holds).</param>
    /// <returns>True if the loop exited normally (that is, the passed condition was no longer valid).</returns>
    public static bool Do(Func<bool> conditionCheck, Action callback, int maxIterations = 999)
    {
        while (conditionCheck() && maxIterations > 0)
        {
            callback();
            maxIterations--;
        }
        return (!conditionCheck());
    }
}
