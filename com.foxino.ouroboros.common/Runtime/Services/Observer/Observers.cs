using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ouroboros.Common.Observer
{
    public class Observers<T> : List<T> where T : class
    {
        private List<T> listenersClone = new List<T>();

        private bool hasInvoked;
        private Action<T> lastInvokedAction;
        
        public void Subscribe(T listener)
        {
            Add(listener);
        }

        public void SubscribeRetroactively(T listener)
        {
            Add(listener);

            if (hasInvoked)
            {
                lastInvokedAction.Invoke(listener);
            }
        }

        public void Subscribe(T[] listeners)
        {
            AddRange(listeners);
        }

        public void Unsubscribe(T listener)
        {
            Remove(listener);
        }

        public void Invoke(Action<T> action)
        {
            listenersClone.Clear();

            for (int i = Count - 1; i >= 0; --i)
            {
                if (this[i] == null
                    || (this[i] is MonoBehaviour component
                        && component == null))
                {
                    RemoveAt(i);
                    continue;
                }

                listenersClone.Add(this[i]);
            }

            for (int i = listenersClone.Count - 1; i >= 0; --i)
            {
                action.Invoke(listenersClone[i]);
            }

            lastInvokedAction = action;

            hasInvoked = true;
        }
    }
}