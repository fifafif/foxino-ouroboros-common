using System;
using System.Collections.Generic;

namespace Ouroboros.Common.FSM
{
    public class SimpleFSM
    {
        private Dictionary<int, SimpleState> stateMap = new Dictionary<int, SimpleState>();
        private SimpleState currentState;
        private int currentStateId = int.MinValue;

        public void AddState(Enum id, SimpleState state)
        {
            AddState(Convert.ToInt32(id), state);
        }

        public void AddState(int id, SimpleState state)
        {
            stateMap.Add(id, state);
        }

        public void Update()
        {
            currentState?.OnUpdate?.Invoke();
        }

        public void SetState(Enum id)
        {
            SetState(Convert.ToInt32(id));
        }

        public void SetState(int id)
        {
            // if (currentStateId == id) return;

            currentState?.OnExit?.Invoke();

            if (!stateMap.TryGetValue(id, out var state))
            {
                throw new Exception($"[SimpleFSM] Cannot set state! Missing state id={id}");
            }

            currentState = state;
            currentStateId = id;
            currentState?.OnEnter?.Invoke();
        }

        public void InvalidateState()
        {
            currentStateId = int.MinValue;
        }
    }
}
