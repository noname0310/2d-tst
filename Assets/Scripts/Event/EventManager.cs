#nullable enable
using System;
using UnityEngine;
using Assets.Scripts.Controller;
using Assets.Scripts.CutScene;
using Cinemachine;

namespace Assets.Scripts.Event
{
    [DefaultExecutionOrder(-2)]
    [DisallowMultipleComponent]
    internal class EventManager : MonoBehaviour
    {
        public static Camera? Camera { get; set; }
        public static CinemachineVirtualCamera? CinemachineVirtualCamera { get; set; }
        public static GameObject? Player { get; set; }
        public static DialogController? Dialog { get; set; }
        public static CutsceneManager? Cutscene { get; set; }
        public static EventManager? Instance { get; private set; }

        public static event Action? OnGameover;

        // ReSharper disable once UnusedMember.Local
        private void Awake() => Instance = this;

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy()
        {
            Camera = null;
            Player = null;
            Dialog = null;
            Cutscene = null;
            Instance = null;
        }

        public void GameOver() => OnGameover?.Invoke();
    }
}
