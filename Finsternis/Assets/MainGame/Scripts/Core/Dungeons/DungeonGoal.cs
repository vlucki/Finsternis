using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

public abstract class DungeonGoal : MonoBehaviour
{
    [Serializable]
    public class GoalReachedEvent : UnityEvent<DungeonGoal>{ }

    public GoalReachedEvent onGoalReached;

    protected bool goalReached;

    public bool GoalReached { get { return goalReached; } }

    public abstract void Check();
}
