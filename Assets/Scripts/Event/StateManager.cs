#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Event
{
    [Serializable]
    public class StateManager
    {
        public State this[string stateName]
        {
            get
            {
                if (_states.TryGetValue(stateName, out var value))
                    return value;
                var state = new State();
                _states.Add(stateName, state);
                return state;
            }
        }

        private readonly Dictionary<string, State> _states;

        public StateManager() => _states = new Dictionary<string, State>();

        public void InitializeState()
        {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var item in _states)
            {
                var state = item.Value;
                state.Value = state.Value;
            }
        }

        public IEnumerator InitializeStateAsync()
        {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var item in _states)
            {
                var state = item.Value;
                state.Value = state.Value;
                yield return null;
            }
        }

        [Serializable]
        public class State
        {
            [field: NonSerialized]
            public event Action? OnChangeState;

            public int Value
            {
                get => _value;
                set
                {
                    _value = value;
                    OnChangeState?.Invoke();
                }
            }

            private int _value;

            public State(int value = 0) => _value = value;
        }
    }
}
