namespace Finsternis
{
    using Extensions;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    [Serializable]
    public abstract class Effect
    {
        protected string name;

        protected List<EffectConstraint> constraints;

        public string Name { get { return this.name; } }

        public ReadOnlyCollection<EffectConstraint> Constraints { get { return this.constraints.AsReadOnly(); } }

        public Effect(string name = null)
        {
            this.constraints = new List<EffectConstraint>();
            if (!string.IsNullOrEmpty(name))
                this.name = name;
        }

        public static implicit operator bool(Effect e)
        {
            return e != null;
        }

        public void AddConstraint(EffectConstraint constraint)
        {
            if (!constraint.IsValid(this))
                return;
            constraints.Add(constraint);
        }

        public bool HasConstraint<T>() where T : IConstraint
        {
            return HasConstraint(typeof(T));
        }

        private bool HasConstraint(Type t)
        {
            return GetConstraint(t) != null;
        }

        public T GetConstraint<T>() where T : EffectConstraint
        {
            return (T)GetConstraint(typeof(T));
        }

        private Constraint GetConstraint(Type t)
        {
            return constraints == null ? null : constraints.Find((constraint) => { return constraint.GetType().Equals(t); });
        }

        /// <summary>
        /// Should this effect be taken in consideation?
        /// </summary>
        /// <returns>True if every constraint is valid</returns>
        public virtual bool ShouldBeActive()
        {
            return constraints.Find((constraint) => { return !constraint.IsValid(this); }) == null;
        }

        public override string ToString()
        {
            return base.ToString() + ((constraints != null && constraints.Count > 0) ? ", constraints: {" + this.constraints.SequenceToString() + "}" : "");
        }
    }
}