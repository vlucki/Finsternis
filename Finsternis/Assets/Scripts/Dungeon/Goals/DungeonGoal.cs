using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

namespace Finsternis
{
    [RequireComponent(typeof(Dungeon))]
    public abstract class DungeonGoal : MonoBehaviour
    {
        [Serializable]
        public class GoalReachedEvent : UnityEvent<DungeonGoal>
        {
            public static implicit operator bool(GoalReachedEvent evt)
            {
                return evt != null;
            }
        }

        [SerializeField]
        public GoalReachedEvent onGoalReached;

        public DungeonGoal()
        {
            if (!onGoalReached)
                onGoalReached = new GoalReachedEvent();
        }

        protected bool goalReached;

        public bool GoalReached { get { return goalReached; } }

        public abstract void Check();
    }
}