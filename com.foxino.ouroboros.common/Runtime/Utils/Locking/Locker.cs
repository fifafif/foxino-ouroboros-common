using System.Collections.Generic;

namespace Ouroboros.Common.Utils.Locking
{
    public class Locker
    {
        private const string defaultName = "default";

        private HashSet<string> lockSet = new HashSet<string>();

        public void Lock(string name)
        {
            lockSet.Add(name.IfNullOrEmptyReturnOther(defaultName));
        }

        public bool Unlock(string name)
        {
            if (!IsLocked())
            {
                return true;
            }

            lockSet.Remove(name.IfNullOrEmptyReturnOther(defaultName));

            return !IsLocked();
        }

        public bool IsLocked()
        {
            return lockSet.Count > 0;
        }

        public void UnlockAll()
        {
            lockSet.Clear();
        }
    }
}