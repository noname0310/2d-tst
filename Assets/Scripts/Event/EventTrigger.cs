#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Render;
using Assets.Scripts.Loader;

namespace Assets.Scripts.Event
{
    public class EventTrigger : MonoBehaviour
    {
        public List<InvokeState>? InvokeConditions;
        public Event Event;
        public List<ChangeStateBranch>? ChangeStates;
        private Collider2D? _collider;
        private StateManager.State[]? _cachedInvokeStates;
        private StateManager.State[]? _cachedChangeStates;
        private static List<ChangeStateBranch>? _sceneChangeStates;
        private static StateManager.State[]? _sceneChangeCachedStates;
        private bool _stageChangeInvoked;

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            _stageChangeInvoked = false;
            _collider = GetComponent<Collider2D>();
            if ((SceneLoader.IsStageLoading || SceneLoader.IsStageChanging) && _collider != null)
                _collider.enabled = false;

            _cachedInvokeStates = new StateManager.State[InvokeConditions!.Count];
            for (var index = 0; index < InvokeConditions.Count; index++)
            {
                var item = InvokeConditions[index];
                if (item.GlobalState)
                    _cachedInvokeStates[index] = GameManager.Storage!.GameData!.GlobalState[item.StateName];
                else
                    _cachedInvokeStates[index] = GameManager.Storage!.GameData!.LocalStates[GameManager.Storage.GameData.CurrentSceneName][item.StateName];
            }
            _cachedChangeStates = new StateManager.State[ChangeStates!.Count];
            for (var index = 0; index < ChangeStates.Count; index++)
            {
                var item = ChangeStates[index];
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var changeStateInfoIndex = 0; changeStateInfoIndex < item.ChangeStateInfos.Count; changeStateInfoIndex++)
                {
                    var changeStateInfo = item.ChangeStateInfos[changeStateInfoIndex];
                    if (changeStateInfo.GlobalState)
                        _cachedChangeStates[index] = GameManager.Storage!.GameData!.GlobalState[changeStateInfo.StateName];
                    else
                        _cachedChangeStates[index] = GameManager.Storage!.GameData!.LocalStates[GameManager.Storage.GameData.CurrentSceneName][changeStateInfo.StateName];
                }
            }

            SceneLoader.OnStageLoaded += SceneLoader_OnStageLoaded;
        }

        private void SceneLoader_OnStageLoaded()
        {
            if (_collider != null)
                _collider!.enabled = true;
            if (Event.TriggerType == Event.EventTriggerType.Start) InvokeTriggerInternal();
            SceneLoader.OnStageLoaded -= SceneLoader_OnStageLoaded;
        }

        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once SuggestBaseTypeForParameter
        private void OnTriggerEnter2D(Collider2D component)
        {
            if (Event.TriggerType == Event.EventTriggerType.Region)
                InvokeTrigger(component.gameObject);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDrawGizmosSelected()
        {
            if (Event.EventType != Event.EventPlayType.CutScene) return;
            foreach (Transform child in transform)
                GizmoDrawer.DrawWireDisk(child, 0.3f);
        }

        public void InvokeTrigger()
        {
            if (Event.TriggerType == Event.EventTriggerType.Extern)
                InvokeTriggerInternal();
            else
                throw new Exception($"{name} is not extern trigger");
        }
        
        public void InvokeTrigger(GameObject targetGameObject)
        {
            if (targetGameObject != EventManager.Player) return;
            InvokeTriggerInternal();
        }

        private void InvokeTriggerInternal()
        {
            if (EventManager.Cutscene!.PlayingCutScene)
                return;

            var invoke = true;
            for (var index = 0; index < InvokeConditions!.Count; index++)
            {
                var item = InvokeConditions[index];
                switch (item.Operator)
                {
                    case InvokeState.InvokeStateOperator.Equals:
                        if (_cachedInvokeStates![index].Value == item.Value) break;
                        invoke = false;
                        goto EndCheckingConditions;
                    case InvokeState.InvokeStateOperator.GraterThan:
                        if (item.Value < _cachedInvokeStates![index].Value) break;
                        invoke = false;
                        goto EndCheckingConditions;
                    case InvokeState.InvokeStateOperator.LowerThan:
                        if (_cachedInvokeStates![index].Value < item.Value) break;
                        invoke = false;
                        goto EndCheckingConditions;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            EndCheckingConditions:

            if (!invoke) return;

            switch (Event.EventType)
            {
                case Event.EventPlayType.Dialog:
                    EventManager.Dialog!.PlayText(Event.ExecuteDialogId,Event.ImageSprites, ApplyChangeState);
                    break;
                case Event.EventPlayType.CutScene:
                    if (Event.TriggerType == Event.EventTriggerType.Region)
                        _collider!.enabled = false;
                    EventManager.Cutscene!.Play(Event.TimelineAsset, Event.DialogIdSet, Event.ImageSprites, gameObject, ApplyChangeState);
                    break;
                case Event.EventPlayType.ChangeStage:
                    //if (SceneLoader.IsStageChanging || SceneLoader.IsStageLoading)
                    //    break;
                    if (Event.EventType == Event.EventPlayType.ChangeStage)
                    {
                        if (_stageChangeInvoked)
                            break;
                        _stageChangeInvoked = true;
                    }
                    ComponentReferenceHolder.EntryPointName = Event.EntryPoint;
                    SceneLoader.ChangeStage(Event.StageName, Event.ShadowTransitionDirection);
                    _sceneChangeStates = ChangeStates;
                    _sceneChangeCachedStates = _cachedChangeStates;
                    SceneLoader.OnStageChanged += SceneLoader_OnStageChanged;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void SceneLoader_OnStageChanged()
        {
            List<ChangeStateInfo>? changeStateInfos = null;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _sceneChangeStates!.Count; i++)
            {
                var item = _sceneChangeStates[i];
                if (item.ExpectReturnValue != 0) continue;
                changeStateInfos = item.ChangeStateInfos;
                break;
            }

            if (changeStateInfos == null)
            {
                SceneLoader.OnStageChanged -= SceneLoader_OnStageChanged;
                return;
            }

            for (var index = 0; index < changeStateInfos.Count; index++)
            {
                var item = changeStateInfos[index];
                
                switch (item.Operator)
                {
                    case ChangeStateInfo.ChangeStateOperator.Set:
                        _sceneChangeCachedStates![index].Value = item.Value;
                        break;
                    case ChangeStateInfo.ChangeStateOperator.Add:
                        _sceneChangeCachedStates![index].Value += item.Value;
                        break;
                    case ChangeStateInfo.ChangeStateOperator.Subtract:
                        _sceneChangeCachedStates![index].Value -= item.Value;
                        break;
                    case ChangeStateInfo.ChangeStateOperator.Multiply:
                        _sceneChangeCachedStates![index].Value *= item.Value;
                        break;
                    case ChangeStateInfo.ChangeStateOperator.Division:
                        _sceneChangeCachedStates![index].Value /= item.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            _sceneChangeStates = null;
            _sceneChangeCachedStates = null;
            SceneLoader.OnStageChanged -= SceneLoader_OnStageChanged;
        }

        private void ApplyChangeState(int dialogingResult)
        {
            List<ChangeStateInfo>? changeStateInfos = null;
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < ChangeStates!.Count; i++)
            {
                var item = ChangeStates[i];
                if (item.ExpectReturnValue != dialogingResult) continue;
                changeStateInfos = item.ChangeStateInfos;
                break;
            }
            if (changeStateInfos == null)
                return;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var changeStateInfoIndex = 0; changeStateInfoIndex < changeStateInfos.Count; changeStateInfoIndex++)
            {
                var item = changeStateInfos[changeStateInfoIndex];
                for (var index = 0; index < _cachedChangeStates!.Length; index++)
                {
                    if (ChangeStates[index].ExpectReturnValue != dialogingResult)
                        continue;
                    switch (item.Operator)
                    {
                        case ChangeStateInfo.ChangeStateOperator.Set:
                            _cachedChangeStates![index].Value = item.Value;
                            break;
                        case ChangeStateInfo.ChangeStateOperator.Add:
                            _cachedChangeStates![index].Value += item.Value;
                            break;
                        case ChangeStateInfo.ChangeStateOperator.Subtract:
                            _cachedChangeStates![index].Value -= item.Value;
                            break;
                        case ChangeStateInfo.ChangeStateOperator.Multiply:
                            _cachedChangeStates![index].Value *= item.Value;
                            break;
                        case ChangeStateInfo.ChangeStateOperator.Division:
                            _cachedChangeStates![index].Value /= item.Value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            if (_collider != null) _collider.enabled = true;
        }

        [Serializable]
        public struct ChangeStateBranch
        {
            public int ExpectReturnValue;
            public List<ChangeStateInfo> ChangeStateInfos;
        }

        [Serializable]
        public struct ChangeStateInfo
        {
            public string StateName;
            public bool GlobalState;
            public ChangeStateOperator Operator;
            public int Value;

            public enum ChangeStateOperator
            {
                Set,
                Add,
                Subtract,
                Multiply,
                Division
            }
        }
        
        [Serializable]
        public struct InvokeState
        {
            public string StateName;
            public bool GlobalState;
            public InvokeStateOperator Operator;
            public int Value;

            public enum InvokeStateOperator
            {
                Equals,
                GraterThan,
                LowerThan
            }
        }
    }
}
