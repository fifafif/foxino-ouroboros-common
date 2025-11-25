using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ouroboros.Common.Utils
{
    public class RegisteredCallback
    {
        private Action callback;
        private bool isInvoked;
        private bool isRepeating;

        public RegisteredCallback() : this(false)
        {
        }

        public RegisteredCallback(bool isRepeating)
        {
            this.isRepeating = isRepeating;
        }

        public void Register(Action callback)
        {
            if (isInvoked)
            {
                callback?.Invoke();

                if (isRepeating)
                {
                    if (this.callback != null)
                    {
                        this.callback -= callback;
                    }

                    this.callback += callback;
                }
            }
            else
            {
                if (this.callback != null)
                {
                    this.callback -= callback;
                }
                
                this.callback += callback;
            }
        }

        public void Unregister(Action callback)
        {
            if (this.callback != null)
            {
                this.callback -= callback;
            }
        }

        public void Invoke()
        {
            callback?.Invoke();

            if (!isRepeating)
            {
                callback = null;
            }

            isInvoked = true;
        }
    }
}