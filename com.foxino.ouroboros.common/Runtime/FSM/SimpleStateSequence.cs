using System.Collections.Generic;

namespace Ouroboros.Common.FSM
{
    public class SimpleStateSequence
    {
        private List<SimpleState> states = new List<SimpleState>();
        private List<float> durations = new List<float>();
        private int index;
        private float currentStateDuration;

        public void AddState(SimpleState state, float duration = 0f)
        {
            states.Add(state);
            durations.Add(duration);
        }

        public void Start()
        {
            if (states.Count <= 0) return;

            index = 0;

            states[index].OnEnter?.Invoke();
            currentStateDuration = 0f;
        }

        public void Update(float deltaTime)
        {
            if (index >= states.Count) return;

            if (currentStateDuration >= durations[index])
            {
                if (!GoToNext())
                {
                    return;
                }
            }

            states[index].OnUpdate?.Invoke();

            currentStateDuration += deltaTime;
        }

        public bool GoToNext()
        {
            if (index >= states.Count)
            {
                return false;
            }

            states[index].OnExit?.Invoke();

            ++index;

            if (index >= states.Count)
            {
                return false;
            }

            states[index].OnEnter?.Invoke();
            currentStateDuration = 0f;

            return true;
        }
    }
}
