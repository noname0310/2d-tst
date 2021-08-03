#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Assets.Scripts.Event;

namespace Assets.Scripts.CutScene
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayableDirector), typeof(TransfromScheduler))]
    public class CutsceneManager : MonoBehaviour
    {
        public bool PlayingCutScene { get; private set; }
        public event Action? OnCutsceneStart;
        public event Action? OnCutsceneDirectorStart;
        public event Action? OnCutsceneComplete;
        private PlayableDirector? _playableDirector;
        private TransfromScheduler? _transfromScheduler;
        private List<string>? _playingDialogIdSet;
        private List<Event.Event.ImageInfo>? _imageInfos;
        private int _nextDialogIdIndex;
        private Action<int>? _onComplete;

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            PlayingCutScene = false;
            _playableDirector = GetComponent<PlayableDirector>();
            _transfromScheduler = GetComponent<TransfromScheduler>();
            _playableDirector.stopped += PlayableDirector_stopped;
            EventManager.Cutscene = this;
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy()
        {
            _playableDirector!.stopped -= PlayableDirector_stopped;
            EventManager.Cutscene = null;
        }

        private void PlayableDirector_stopped(PlayableDirector obj)
        {
            _playingDialogIdSet = null;
            _imageInfos = null;
            _onComplete?.Invoke(0);
            PlayingCutScene = false;
            OnCutsceneComplete?.Invoke();
        }

        public void Play(TimelineAsset timelineAsset, List<string> dialogIdSet, List<Event.Event.ImageInfo> imageInfos, GameObject eventObject, Action<int> onComplete)
        {
            _playableDirector!.playableAsset = timelineAsset;
            _playingDialogIdSet = dialogIdSet;
            _imageInfos = imageInfos;
            _nextDialogIdIndex = 0;
            _onComplete = onComplete;
            PlayingCutScene = true;
            OnCutsceneStart?.Invoke();
            _transfromScheduler!.StartPositioning(eventObject, () =>
            {
                OnCutsceneDirectorStart?.Invoke();
                _playableDirector.Play();
            });
        }

        public void ShowNextDialog()
        {
            if (_playingDialogIdSet!.Count - 1 < _nextDialogIdIndex)
                return;
            _playableDirector!.playableGraph.GetRootPlayable(0).Pause();
            EventManager.Dialog!.PlayText(_playingDialogIdSet![_nextDialogIdIndex], _imageInfos!, DialogComplete);
            _nextDialogIdIndex++;
        }

        private void DialogComplete(int exitResult)
        {
            if (PlayingCutScene)
                _playableDirector!.playableGraph.GetRootPlayable(0).Play();
        }
    }
}
