using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using Assets.Scripts.Loader;

namespace Assets.Scripts.Event
{
    [Serializable]
    public struct Event
    {
        public EventTriggerType TriggerType;
        public EventPlayType EventType;
        public string ExecuteDialogId;
        public TimelineAsset TimelineAsset;
        public List<string> DialogIdSet;
        public List<ImageInfo> ImageSprites;
        public string StageName;
        public string EntryPoint;
        public SceneLoader.ShadowTransitionDirection ShadowTransitionDirection;

        [Serializable]
        public struct ImageInfo
        {
            public string ImageId;
            public Sprite Sprite;
        }

        public enum EventTriggerType
        {
            Region,
            Raycast,
            Start,
            Extern
        }

        public enum EventPlayType
        {
            Dialog,
            CutScene,
            ChangeStage
        }
    }
}
