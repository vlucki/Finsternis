namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    
    public class Lock
    {

        #region Events
        public delegate void UnlockDelegate(Lock l);
        public event UnlockDelegate onUnlock;
        #endregion

        [SerializeField]
        private bool permanentLock = false;

        private OpeneableEntity lockedEntity;

        private KeyCard key;

       public virtual bool Unlock(KeyCard key)
        {
            if (permanentLock)
                return false;
            
            if (key.UsedUp)
                return false;

            bool correctKey = key.Equals(this.key);

            if (correctKey)
            {
                key.Use();
                onUnlock(this);
            }

            return correctKey;
        }

        public void SetKey(KeyCard c)
        {
            if (!c || key)
                return;
            this.key = c;
        }

        public void RemovePermanentLock()
        {
            this.permanentLock = false;
        }

    }
}