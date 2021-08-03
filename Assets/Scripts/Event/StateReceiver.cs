#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Event
{
    public class StateReceiver : MonoBehaviour
    {
        public List<EventTrigger.InvokeState>? InvokeConditions;
        public UnityEvent ConditionPassed = new UnityEvent();
        private StateManager.State[]? _cachedStates;

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            _cachedStates = new StateManager.State[InvokeConditions!.Count];
            for (var index = 0; index < InvokeConditions.Count; index++)
            {
                var item = InvokeConditions[index];
                if (item.GlobalState)
                    _cachedStates[index] = GameManager.Storage!.GameData!.GlobalState[item.StateName];
                else
                    _cachedStates[index] =
                        GameManager.Storage!.GameData!.LocalStates[GameManager.Storage.GameData.CurrentSceneName][item.StateName];
            }

            foreach (var item in _cachedStates) item.OnChangeState += Item_OnChangeState;
            Item_OnChangeState();
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy()
        {
            foreach (var item in _cachedStates!) item.OnChangeState -= Item_OnChangeState;
        }

        private void Item_OnChangeState()
        {
            var result = true;
            for (var index = 0; index < InvokeConditions!.Count; index++)
            {
                var condition = InvokeConditions[index];
                var state = _cachedStates![index];
                switch (condition.Operator)
                {
                    case EventTrigger.InvokeState.InvokeStateOperator.Equals:
                        if (state.Value == condition.Value)
                            break;
                        else
                        {
                            result = false;
                            goto endCheckCondition;
                        }
                    case EventTrigger.InvokeState.InvokeStateOperator.GraterThan:
                        if (condition.Value < state.Value)
                            break;
                        else
                        {
                            result = false;
                            goto endCheckCondition;
                        }
                    case EventTrigger.InvokeState.InvokeStateOperator.LowerThan:
                        if (state.Value < condition.Value)
                            break;
                        else
                        {
                            result = false;
                            goto endCheckCondition;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            endCheckCondition:
            if (result) ConditionPassed.Invoke();
        }
    }
}
