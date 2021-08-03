#nullable enable
using System;
using UnityEditor;
using UnityEngine;
using Assets.Scripts.Event;

namespace Assets.Editor.Event
{
    [CustomEditor(typeof(EventTrigger))]
    [CanEditMultipleObjects]
    internal class EventTriggerEditor : UnityEditor.Editor
    {
        private SerializedProperty? _invokeConditions;
        private SerializedProperty? _eventTriggerType;
        private SerializedProperty? _eventType;
        private SerializedProperty? _eventExecuteDialogId;
        private SerializedProperty? _eventTimelineAsset;
        private SerializedProperty? _eventDialogIdSet;
        private SerializedProperty? _eventImageSprites;
        private SerializedProperty? _eventStageName;
        private SerializedProperty? _eventEntryPoint;
        private SerializedProperty? _eventShadowTransitionDirection;
        private SerializedProperty? _changeState;
        private EventTrigger? _eventTrigger;
        private bool _eventTabExpending;
        
        // ReSharper disable once UnusedMember.Local
        private void OnEnable()
        {
            _eventTrigger = (EventTrigger)target;
            // Fetch the objects from the GameObject script to display in the inspector
            _invokeConditions = serializedObject.FindProperty("InvokeConditions");
            _eventTriggerType = serializedObject.FindProperty("Event").FindPropertyRelative("TriggerType");
            _eventType = serializedObject.FindProperty("Event").FindPropertyRelative("EventType");
            _eventExecuteDialogId = serializedObject.FindProperty("Event").FindPropertyRelative("ExecuteDialogId");
            _eventTimelineAsset = serializedObject.FindProperty("Event").FindPropertyRelative("TimelineAsset");
            _eventDialogIdSet = serializedObject.FindProperty("Event").FindPropertyRelative("DialogIdSet");
            _eventImageSprites = serializedObject.FindProperty("Event").FindPropertyRelative("ImageSprites");
            _eventStageName = serializedObject.FindProperty("Event").FindPropertyRelative("StageName");
            _eventEntryPoint = serializedObject.FindProperty("Event").FindPropertyRelative("EntryPoint");
            _eventShadowTransitionDirection = serializedObject.FindProperty("Event").FindPropertyRelative("ShadowTransitionDirection");
            _changeState = serializedObject.FindProperty("ChangeStates");
            _eventTabExpending = true;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_invokeConditions, new GUIContent("Invoke Conditions"));
            _eventTabExpending = EditorGUILayout.Foldout(_eventTabExpending, new GUIContent("Event"));
            if (_eventTabExpending)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10f);
                GUILayout.BeginVertical();
                EditorGUILayout.PropertyField(_eventTriggerType, new GUIContent("Trigger Type"));
                EditorGUILayout.PropertyField(_eventType, new GUIContent("Event Type"));
                switch (_eventTrigger!.Event.EventType)
                {
                    case Scripts.Event.Event.EventPlayType.Dialog:
                        EditorGUILayout.PropertyField(_eventExecuteDialogId, new GUIContent("Execute Dialog Id"));
                        EditorGUILayout.PropertyField(_eventImageSprites, new GUIContent("Image Sprites"));
                        break;
                    case Scripts.Event.Event.EventPlayType.CutScene:
                        EditorGUILayout.PropertyField(_eventTimelineAsset, new GUIContent("Timeline Asset"));
                        EditorGUILayout.PropertyField(_eventDialogIdSet, new GUIContent("Dialog Id Set"));
                        EditorGUILayout.PropertyField(_eventImageSprites, new GUIContent("Image Sprites"));
                        break;
                    case Scripts.Event.Event.EventPlayType.ChangeStage:
                        EditorGUILayout.PropertyField(_eventStageName, new GUIContent("Stage Name"));
                        EditorGUILayout.PropertyField(_eventEntryPoint, new GUIContent("Entry Point"));
                        EditorGUILayout.PropertyField(_eventShadowTransitionDirection, new GUIContent("Shadow Transition"));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.PropertyField(_changeState, new GUIContent("Change States"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}
