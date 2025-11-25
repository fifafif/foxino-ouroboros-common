using System;

namespace Ouroboros.Common.FSM
{
    public class SimpleState
    {
        public Action OnUpdate;
        public Action OnEnter;
        public Action OnExit;

        public SimpleState()
        {
        }

        public SimpleState(Action onUpdate)
        {
            OnUpdate = onUpdate;
        }

        public SimpleState(Action onEnter, Action onUpdate)
        {
            OnUpdate = onUpdate;
            OnEnter = onEnter;
        }

        public SimpleState(Action onEnter, Action onUpdate, Action onExit)
        {
            OnUpdate = onUpdate;
            OnEnter = onEnter;
            OnExit = onExit;
        }
    }
}
