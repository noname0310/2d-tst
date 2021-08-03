#nullable enable
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Assets.Scripts.Event;
using Assets.Scripts.Loader;
using Assets.Scripts.Optimizer;
using Assets.Scripts.UI;
using Assets.Scripts.UI.Game;

namespace Assets.Scripts.Controller
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class GameUiController : MonoBehaviour
    { 
        public GameMenuUiController? MenuController { get; private set; }

        public EventSystem? EventSystem;
        public StickUi? StickUi;
        public InteractionUi? InteractionUi;
        public PauseUi? PauseUi;
        public DialogUi? DialogUi;
        public BlurUi? BlurUi;
        public NavigationCursor? NavigationCursor;
        public FadeAnimationController? DialogBranch;
        public FadeAnimationController? DialogCenterImage;
        public FadeAnimationController? GameOverPanel;

        private Animator? _animator;
        private bool _dialoging;
        private bool _dialogBranching;
        private bool _dialogCenterImageShowing;
        private bool _usingTouch;

        private static readonly int ShowStickHash = Animator.StringToHash("ShowStick");
        private static readonly int ShowPauseHash = Animator.StringToHash("ShowPause");
        private static readonly int ShowDialogHash = Animator.StringToHash("ShowDialog");
        private static readonly int ImmediateStickHash = Animator.StringToHash("ImmediateStick");
        private static readonly int ImmediatePauseHash = Animator.StringToHash("ImmediatePause");
        private static readonly int ImmediateDialogHash = Animator.StringToHash("ImmediateDialog");

        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            GameManager.OnInputControlsChanged += GameManager_OnInputControlsChanged;
            MenuController = gameObject.AddComponent<GameMenuUiController>();
            MenuController.Animator = _animator;
            MenuController.OnMenuActiveUpdated += MenuController_OnMenuActiveUpdated;
            DialogUi!.OnDialogShowing += DialogUi_OnDialogShowing;
            DialogUi.OnDialogHiding += DialogUi_OnDialogHiding;
            DialogUi.OnBranchShowing += DialogUi_OnBranchShowing;
            DialogUi.OnBranchHiding += DialogUi_OnBranchHiding;
            DialogUi.OnCenterImageShowing += DialogUi_OnCenterImageShowing;
            DialogUi.OnCenterImageHiding += DialogUi_OnCenterImageHiding;
            _usingTouch = true;
            EventManager.Cutscene!.OnCutsceneStart += Cutscene_OnCutsceneStart;
            EventManager.Cutscene.OnCutsceneComplete += Cutscene_OnCutsceneComplete;
            EventManager.OnGameover += EventManager_OnGameover;
            SceneLoader.OnStageChange += SceneLoader_OnStageChange;
            SceneLoader.OnStageChanged += SceneLoader_OnStageChanged;
            GameManager.Ui!.OnPause += Ui_OnPause;

            _animator.SetBool(ImmediateStickHash, true);
            _animator.SetBool(ImmediatePauseHash, true);
            _animator.SetBool(ImmediateDialogHash, true);
            GameManager_OnInputControlsChanged(GameManager.CurrentControlScheme);
            _animator.SetBool(ImmediateStickHash, false);
            _animator.SetBool(ImmediatePauseHash, false);
            _animator.SetBool(ImmediateDialogHash, false);
            DialogBranch!.DisableImmidate();
            DialogCenterImage!.DisableImmidate();
        }

        private void EventManager_OnGameover()
        {
            GameOverPanel!.Enable(() => StartCoroutine(WaitGoTitle()));

            IEnumerator WaitGoTitle()
            {
                yield return YieldInstructionCache.WaitForSecondsRealtime(5f);
                SceneLoader.LoadTitle();
            }
        }

        private void MenuController_OnMenuActiveUpdated(bool showingMenu)
        {
            GameManager.SetInputMap(showingMenu ? "UI" : "Player");
            if (showingMenu)
            {
                BlurUi!.ShowBlur();
                GameManager.Ui!.BlockingPause = false;
                GameManager.Ui.ControllerLeftStickMove(Vector2.zero);
                GameManager.Ui.BlockingLeftStickMove = true;
                GameManager.Ui.BlockingInteraction = true;
                StickUi!.BlockingInput = true;
                InteractionUi!.BlockingInput = true;
                PauseUi!.BlockingInput = true;

                _animator!.SetBool(ShowDialogHash, false);
                _animator.SetBool(ShowStickHash, false);
                _animator.SetBool(ShowPauseHash, false);

                if (_dialogBranching) return;
                if (NavigationCursor!.TryEnable())
                    NavigationCursor.EventHandleEnable();
            }
            else
            {
                BlurUi!.HideBlur();
                UpdateControllerVisableState();

                if (_dialogBranching) return;
                NavigationCursor!.EventHandleDisable();
                NavigationCursor.Disable(() => EventSystem!.SetSelectedGameObject(null!));
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy()
        {
            if (EventManager.Cutscene is { })
            {
                EventManager.Cutscene.OnCutsceneStart -= Cutscene_OnCutsceneStart;
                EventManager.Cutscene.OnCutsceneComplete -= Cutscene_OnCutsceneComplete;
            }

            EventManager.OnGameover -= EventManager_OnGameover;
            DialogUi!.OnDialogShowing -= DialogUi_OnDialogShowing;
            DialogUi.OnDialogHiding -= DialogUi_OnDialogHiding;
            DialogUi.OnBranchShowing -= DialogUi_OnBranchShowing;
            DialogUi.OnBranchHiding -= DialogUi_OnBranchHiding;
            DialogUi.OnCenterImageShowing -= DialogUi_OnCenterImageShowing;
            DialogUi.OnCenterImageHiding -= DialogUi_OnCenterImageHiding;
            SceneLoader.OnStageChange -= SceneLoader_OnStageChange;
            SceneLoader.OnStageChanged -= SceneLoader_OnStageChanged;
            GameManager.OnInputControlsChanged -= GameManager_OnInputControlsChanged;
            if (GameManager.Ui != null)
                GameManager.Ui.OnPause -= Ui_OnPause;
            GameManager.SetInputMap("Player");
        }

        private void DialogUi_OnDialogShowing()
        {
            _dialoging = true;
            _animator!.SetBool(ShowDialogHash, true);
            UpdateControllerVisableState();
        }

        private void DialogUi_OnDialogHiding()
        {
            _dialoging = false;
            _animator!.SetBool(ShowDialogHash, false);
            UpdateControllerVisableState();
        }
        
        private void DialogUi_OnBranchShowing()
        {
            _dialogBranching = true;
            DialogBranch!.Enable(null);
            UpdateControllerVisableState();

            if (MenuController!.IsMenuOpened) return;
            NavigationCursor!.AllowKeyboard = true;
            if (NavigationCursor.TryEnable())
                NavigationCursor.EventHandleEnable();
            else
                NavigationCursor!.AllowKeyboard = false;
        }

        private void DialogUi_OnBranchHiding()
        {
            _dialogBranching = false;
            DialogBranch!.Disable(null);
            UpdateControllerVisableState();

            if (MenuController!.IsMenuOpened) return;

            NavigationCursor!.EventHandleDisable();
            NavigationCursor.Disable(() => EventSystem!.SetSelectedGameObject(null!));
            NavigationCursor!.AllowKeyboard = false;
        }

        private void DialogUi_OnCenterImageShowing()
        {
            _dialogCenterImageShowing = true;
            DialogCenterImage!.Enable(null);
            UpdateControllerVisableState();
        }

        private void DialogUi_OnCenterImageHiding()
        {
            _dialogCenterImageShowing = false;
            DialogCenterImage!.Disable(null);
            UpdateControllerVisableState();
        }

        private void Cutscene_OnCutsceneStart() => UpdateControllerVisableState();

        private void Cutscene_OnCutsceneComplete() => UpdateControllerVisableState();

        private void SceneLoader_OnStageChange()
        {
            EventManager.Cutscene!.OnCutsceneStart -= Cutscene_OnCutsceneStart;
            EventManager.Cutscene.OnCutsceneComplete -= Cutscene_OnCutsceneComplete;
        }

        private void SceneLoader_OnStageChanged()
        {
            EventManager.Cutscene!.OnCutsceneStart += Cutscene_OnCutsceneStart;
            EventManager.Cutscene.OnCutsceneComplete += Cutscene_OnCutsceneComplete;
        }

        private void GameManager_OnInputControlsChanged(PlayerInput obj) =>
            GameManager_OnInputControlsChanged(obj.currentControlScheme);

        private void GameManager_OnInputControlsChanged(string currentControlScheme)
        {
#if UNITY_EDITOR
            if (currentControlScheme == "Touch" || currentControlScheme == "Keyboard&Mouse")
#else
            if (currentControlScheme == "Touch")
#endif
            {
                _usingTouch = true;
                UpdateControllerVisableState();
            }
            else
            {
                _usingTouch = false;
                UpdateControllerVisableState();
            }
        }

        private void Ui_OnPause()
        {
            if (MenuController!.IsMenuOpened)
                MenuController.GoBack();
            else
                MenuController.OpenMenu();
        }

        private void UpdateControllerVisableState()
        {
            if (_dialoging || _dialogBranching || _dialogCenterImageShowing || EventManager.Cutscene!.PlayingCutScene)
            {
                if (GameManager.Ui != null)
                {
                    GameManager.Ui.ControllerLeftStickMove(Vector2.zero);
                    GameManager.Ui.BlockingLeftStickMove = true;
                    GameManager.Ui.BlockingInteraction = true;
                    GameManager.Ui.BlockingPause = true;
                }
            }
            else
            {
                if (GameManager.Ui != null)
                {
                    GameManager.Ui.BlockingLeftStickMove = false;
                    GameManager.Ui.BlockingInteraction = false;
                    GameManager.Ui.BlockingPause = false;
                }
            }

            if (_dialoging || _dialogBranching || _dialogCenterImageShowing || EventManager.Cutscene!.PlayingCutScene)
            {
                _animator!.SetBool(ShowStickHash, false);
                _animator.SetBool(ShowPauseHash, false);
                StickUi!.BlockingInput = true;
                InteractionUi!.BlockingInput = true;
                PauseUi!.BlockingInput = true;
            }
            else
            {
                if (_usingTouch)
                {
                    _animator!.SetBool(ShowStickHash, true);
                    _animator.SetBool(ShowPauseHash, true);
                    StickUi!.BlockingInput = false;
                    InteractionUi!.BlockingInput = false;
                    PauseUi!.BlockingInput = false;
                }
                else
                {
                    _animator!.SetBool(ShowStickHash, false);
                    _animator!.SetBool(ShowPauseHash, false);
                    StickUi!.BlockingInput = true;
                    InteractionUi!.BlockingInput = true;
                    PauseUi!.BlockingInput = true;
                }
            }
        }
    }
}
