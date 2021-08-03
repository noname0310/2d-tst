#nullable enable
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using Assets.Scripts.UI;
using Assets.Scripts.Sound;
using Assets.Scripts.Storage;

namespace Assets.Scripts
{
    [DefaultExecutionOrder(-3)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerInput))]
    public class GameManager : MonoBehaviour
    {
        public static event Action<PlayerInput>? OnInputControlsChanged;

        public static UiEventManager? Ui { get; private set; }
        public static SoundManager? Sound { get; private set; }
        public static LocalStorage? Storage { get; private set; }
        public static string CurrentControlScheme => _playerInput!.currentControlScheme;
        public AudioMixer? AudioMixer;
        public GlobalSound? GlobalSound;
        public string StartStageName = "Scenes/GameScene/Stages/Room3/3_(2)Long-hallway";

        private static PlayerInput? _playerInput;

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            Ui = new UiEventManager();
            Sound = new SoundManager(AudioMixer!, GlobalSound!);
            Storage = new LocalStorage(StartStageName);
            Storage.LoadSettings();
            _playerInput = GetComponent<PlayerInput>();

            SettingsBootstraper.Load(Sound, Storage.Settings ?? throw new ArgumentNullException());
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy()
        {
            Ui = null;
            Sound = null;
            Storage = null;
            _playerInput = null;
        }

        // ReSharper disable once UnusedMember.Local
#pragma warning disable IDE0051 // Remove unused private members
        private void OnMove(InputValue value) => Ui?.ControllerLeftStickMove(value.Get<Vector2>());
#pragma warning restore IDE0051 // Remove unused private members

        // ReSharper disable once UnusedMember.Local
#pragma warning disable IDE0051 // Remove unused private members
        private void OnInteraction() => Ui?.InteractionButtonClicked();
#pragma warning restore IDE0051 // Remove unused private members

        // ReSharper disable once UnusedMember.Local
#pragma warning disable IDE0051 // Remove unused private members
        private void OnPause() => Ui?.PauseButtonClicked();
#pragma warning restore IDE0051 // Remove unused private members

        // ReSharper disable once UnusedMember.Local
#pragma warning disable IDE0051 // Remove unused private members
        private void OnDialogClick() => Ui?.DialogButtonClicked();
#pragma warning restore IDE0051 // Remove unused private members

        // ReSharper disable once UnusedMember.Local
#pragma warning disable IDE0051 // Remove unused private members
        private void OnControlsChanged(PlayerInput playerInput) => OnInputControlsChanged?.Invoke(playerInput);
#pragma warning restore IDE0051 // Remove unused private members

        public static void SetInputMap(string mapName) => _playerInput?.SwitchCurrentActionMap(mapName);
    }
}
