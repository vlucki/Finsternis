using UnityEngine;
using System.Collections.Generic;

using System;
using System.Collections;
using UnityQuery;
using UnityEngine.Events;

namespace Finsternis
{
    [RequireComponent(typeof(Animator))]
    public class TrapBehaviour : Entity
    {
        public UnityEvent onDeactivated;

        void Deactivate()
        {
            onDeactivated.Invoke();
        }

    }
}